using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using CaseManagement.Api.Endpoints;
using CaseManagement.Api.Infrastructure;
using CaseManagement.Application;
using CaseManagement.Infrastructure;
using CaseManagement.Infrastructure.Identity;
using CaseManagement.Infrastructure.Identity.Entities;
using CaseManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Aspire: OpenTelemetry, health checks, service discovery, resilient HTTP.
builder.AddServiceDefaults();

//Aspire:Aspire reads the "casesdb" connection string it injected and registers
//ApplicationDbContext with the Npgsql provider - in a single call.
builder.AddNpgsqlDbContext<ApplicationDbContext>("casesdb");

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

//Read the "JwtSettings" section into  a strongly-typed object.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options => options.User.RequireUniqueEmail = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
//Tell ASP.NET Core to authenticate requests using JWT bearer tokens.
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        //Keep the claim names exactly as they appear in the token(no surprise remapping).
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            //No grace period on expiry - the token dies exactly when it says it does.
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtRegisteredClaimNames.Name,
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddOpenApi();

//Accept and return enums as their string names ("Action") instead of numbers.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

//Turn domain rule violations into 400 ProblemDetails responses.
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

var app = builder.Build();

//DEMO ONLY:create and migrate the database, then seed sample data, so the app
//runs with a single command. Never auto-migrate or auto-speed in production.

if(app.Environment.IsDevelopment())
{
    await app.Services.InitializeDatabaseAsync();
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapAuthEndpoints();
app.MapSecuredEndpoints();
app.MapCaseEndpoints();

app.Run();

// Makes the implicitly-generated Program class public so the integration
// test project (a separate assembly) can use WebApplicationFactory<Program>.
public partial class Program { }
//app.MapGet("/", () => "Hello World!");

//app.Run();
