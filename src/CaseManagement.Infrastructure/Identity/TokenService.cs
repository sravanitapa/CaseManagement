using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CaseManagement.Infrastructure.Identity;

public class TokenService(IOptions<JwtSettings> jwtSettings):ITokenService
{
    private readonly JwtSettings _settings = jwtSettings.Value;

    public (string Token, DateTime ExpiresAt) CreateToken(Entities.ApplicationUser user, IEnumerable<string> roles)
    {
        // Builds a signed JWT with user claims and role claims.
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);
        //Claims are the pieces of information we store inside the token
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,user.Id),
            new(JwtRegisteredClaimNames.Email,user.Email!),
            new(JwtRegisteredClaimNames.Name,$"{user.FirstName}{user.LastName}"),
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        //One "role" claim per role the user has
        claims.AddRange(roles.Select(role => new Claim("role", role)));

        //The signing key proves the token came from us and was not tampered with.
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials=new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256);

        var descriptor=new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            SigningCredentials = credentials,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience
        };

        //JsonWebTokenHandler is the modern,faster handler that replaces JwtSecurityTokenHandler
        var handler=new JsonWebTokenHandler();

        var token = handler.CreateToken(descriptor);
        return (token, expiresAt);
    }

    public string CreateRefreshToken()
    {
        // A refresh token is just a large cryptographically-random value.
        // RandomNumberGenerator replaces the obselete RNGCryptoServiceProvider.
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}
