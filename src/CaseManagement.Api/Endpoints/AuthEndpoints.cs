using CaseManagement.Api.Auth.DTOs;
using CaseManagement.Infrastructure.Identity;
using CaseManagement.Infrastructure.Identity.Entities;
using CaseManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CaseManagement.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", RegisterAsync);
        group.MapPost("/login", LoginAsync);

        //Only an existing admin is allowed to hand out roles
        group.MapPost("/add-role", AddRoleAsync)
            .RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        // Refresh and revoke are anonymous on purpose: the access token is usually
        // expired by the time you call them, so the refrsh token IS the credential.
        group.MapPost("/refresh", RefreshAsync);
        group.MapPost("/revoke", RevokeAsync);
    }

    private static async Task<IResult> RegisterAsync(Auth.DTOs.RegisterRequest request,UserManager<ApplicationUser> userManager)
    {
        //Stop duplicate sign-ups with the same email.
        if(await userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Results.BadRequest($"Email '{request.Email}' is already registered.");
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if(!result.Succeeded)
        {
            //Identity tells us exactly why (weak password, etc.).
            return Results.BadRequest(result.Errors.Select(equals => equals.Description));
        }

        //Every new user starts as regular "User"
        await userManager.AddToRoleAsync(user, Roles.User);

        return Results.Ok($"User '{request.Email}' registered successfully.");
    }

    private static async Task<IResult> LoginAsync(
        Auth.DTOs.LoginRequest request,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings,
        ApplicationDbContext applicationDbContext)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        //Same response for "no such user" and "wrong password" so we don't leaks which emails exist.
        if(user is null||!await userManager.CheckPasswordAsync(user,request.Password))
        {
            return Results.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var (token, expiresAt) = tokenService.CreateToken(user, roles);

        //Issue a refresh token and store it so we can rotate or revoke it later.
        var refreshToken = tokenService.CreateRefreshToken();
        var refreshExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenDays);

        applicationDbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Created = DateTime.UtcNow,
            Expires = refreshExpiresAt
        });
        await applicationDbContext.SaveChangesAsync();

        return Results.Ok(new AuthResponse(user.Id, user.Email!, roles, token, expiresAt,refreshToken,refreshExpiresAt));
    }

    private static async Task<IResult> AddRoleAsync(
        AddRoleRequest request,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if(user is null)
        {
            return Results.NotFound($"No user found with email '{request.Email}'.");
        }

        if(!await roleManager.RoleExistsAsync(request.Role))
        {
            return Results.BadRequest($"Role '{request.Role}' does not exist.");
        }

        await userManager.AddToRoleAsync(user, request.Role);
        return Results.Ok($"Role '{request.Role}' added to '{request.Email}'");
    }

    private static async Task<IResult> RefreshAsync(Auth.DTOs.RefreshRequest request,
        ApplicationDbContext applicationDbContext,UserManager<ApplicationUser> userManager,
        ITokenService tokenService,IOptions<JwtSettings> jwtSettings)
    {
        var existing = await applicationDbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        //Token we have never seen - reject.
        if(existing is null)
        {
            return Results.Unauthorized();
        }

        // Token exists but is not usable. If it was already revoked, someone is
        // replaying an old token - assume it was stolen and revoke the whole family.
        if(!existing.IsActive)
        {
            if(existing.Revoked is not null)
            {
                await RevokeAllActiveTokensAsync(applicationDbContext, existing.UserId);
            }
            return Results.Unauthorized();
        }

        var user = await userManager.FindByIdAsync(existing.UserId);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        //Rotation: every refresh kill the old token and issues a brand-new one.
        var newRefreshToken = tokenService.CreateRefreshToken();
        var refreshExpiresAt = DateTime.UtcNow.AddDays(jwtSettings.Value.RefreshTokenDays);

        existing.Revoked = DateTime.UtcNow;
        existing.ReplacedByToken = newRefreshToken;

        applicationDbContext.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            Created = DateTime.UtcNow,
            Expires = refreshExpiresAt
        });
        await applicationDbContext.SaveChangesAsync();

        var roles = await userManager.GetRolesAsync(user);
        var (accessToken, accessExpiresAt) = tokenService.CreateToken(user, roles);

        return Results.Ok(new AuthResponse(
            user.Id, user.Email!, roles,
            accessToken, accessExpiresAt,
            newRefreshToken, refreshExpiresAt));
    }

    private static async Task<IResult> RevokeAsync(RevokeRequest request,ApplicationDbContext applicationDbContext)
    {
        var token = await applicationDbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if(token is null||!token.IsActive)
        {
            return Results.NotFound("Token not found or already inactive.");
        }

        token.Revoked = DateTime.UtcNow;
        await applicationDbContext.SaveChangesAsync();
        return Results.Ok("Refresh token revoked.");
    }

    // when a stolen token is detected, cancel every active token for that user
    // so the attacker (and the real user) both have to login again.
    private static async Task RevokeAllActiveTokensAsync(ApplicationDbContext applicationDbContext,string userId)
    {
        var activeTokens = await applicationDbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.Revoked == null)
            .ToListAsync();

        foreach(var token in activeTokens)
        {
            token.Revoked = DateTime.UtcNow;
        }

        await applicationDbContext.SaveChangesAsync();

    }
}
