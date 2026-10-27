using System.Net;
using System.Net.Http.Json;
using CaseManagement.Api.Auth.DTOs;
using Xunit;

namespace CaseManagement.Api.Tests;

public class RefreshTokenTests : IClassFixture<CaseManagementApiFactory>
{
    private readonly HttpClient _client;

    public RefreshTokenTests(CaseManagementApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<AuthResponse> RegisterAndLoginAsync()
    {
        var email = $"{Guid.NewGuid():N}@example.com";
        const string password = "Passw0rd!123";

        await _client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("Test", "User", email, password));

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(email, password));

        var body = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        return body!;
    }

    [Fact]
    public async Task Refresh_WithValidToken_IssuesNewTokenPair()
    {
        var login = await RegisterAndLoginAsync();

        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest(login.RefreshToken));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.NotEqual(login.RefreshToken, body!.RefreshToken);
        Assert.NotEqual(login.Token, body.Token);
    }

    [Fact]
    public async Task Refresh_WithUnknownToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest("this-token-was-never-issued"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_ReusingAnAlreadyRotatedToken_RevokesTheWholeFamily()
    {
        // This is the theft-detection path: a refresh token that's already
        // been rotated gets used a second time (an attacker replaying a
        // stolen token, most likely). Per AuthEndpoints.RefreshAsync, that
        // should burn every active token for the user, not just reject the
        // one reused request - so even the token from the first, legitimate
        // rotation should stop working afterward.
        var login = await RegisterAndLoginAsync();

        var firstRefresh = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest(login.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, firstRefresh.StatusCode);
        var firstBody = await firstRefresh.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(firstBody);

        // Replay the original (now-rotated) token - should be rejected.
        var replay = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest(login.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, replay.StatusCode);

        // The legitimate second-generation token should now be dead too,
        // since the whole family was revoked in response to the replay.
        var secondRefreshAttempt = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest(firstBody!.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, secondRefreshAttempt.StatusCode);
    }

    [Fact]
    public async Task Revoke_ThenRefresh_ReturnsUnauthorized()
    {
        var login = await RegisterAndLoginAsync();

        var revokeResponse = await _client.PostAsJsonAsync("/api/auth/revoke",
            new RevokeRequest(login.RefreshToken));
        Assert.Equal(HttpStatusCode.OK, revokeResponse.StatusCode);

        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequest(login.RefreshToken));
        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }
}
