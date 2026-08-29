using System.Net;
using System.Net.Http.Json;
using CaseManagement.Api.Auth.DTOs;
using Xunit;

namespace CaseManagement.Api.Tests;

public class AuthEndpointsTests : IClassFixture<CaseManagementApiFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CaseManagementApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static RegisterRequest NewRegisterRequest(string? emailOverride = null) => new(
        FirstName: "Test",
        LastName: "User",
        Email: emailOverride ?? $"{Guid.NewGuid():N}@example.com",
        Password: "Passw0rd!123");

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", NewRegisterRequest());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        var request = NewRegisterRequest();
        await _client.PostAsJsonAsync("/api/auth/register", request);

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens()
    {
        var register = NewRegisterRequest();
        await _client.PostAsJsonAsync("/api/auth/register", register);

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(register.Email, register.Password));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        Assert.False(string.IsNullOrWhiteSpace(body.RefreshToken));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var register = NewRegisterRequest();
        await _client.PostAsJsonAsync("/api/auth/register", register);

        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(register.Email, "WrongPassword!1"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsUnauthorized_NotNotFound()
    {
        // Login intentionally returns the same status for "no such user" and
        // "wrong password" so an attacker can't enumerate registered emails.
        // This test locks that behavior in so it can't regress silently.
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("nobody@example.com", "Passw0rd!123"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
