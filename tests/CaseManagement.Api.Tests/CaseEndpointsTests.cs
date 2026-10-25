using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using CaseManagement.Api.Auth.DTOs;
using CaseManagement.Domain.Cases;
using CaseManagement.Application.Cases.Commands.CreateCase;
using Xunit;
using System.Net;
using CaseManagement.Application.Cases;

namespace CaseManagement.Api.Tests;

public class CaseEndpointsTests:IClassFixture<CaseManagementApiFactory>
{
    private readonly HttpClient _client;

    public CaseEndpointsTests(CaseManagementApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsAdminAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("admin@casemanagement.local", "Admin123!"));
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", body!.Token);
    }

    private static CreateCaseCommand NewCase() => new("Test Case " + Guid.NewGuid(), "John Smith", DateTime.UtcNow.AddDays(7),
        CaseStatus.Open, "Synopsis");

    [Fact]
    public async Task CreateCase_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/cases", NewCase());
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCase_AsAdmin_ReturnsCreated()
    {
        await AuthenticateAsAdminAsync();

        var response = await _client.PostAsJsonAsync("/cases", NewCase());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCase_WithValidData_ReturnsNoContent()
    {
        await AuthenticateAsAdminAsync();
        var created = await _client.PostAsJsonAsync("/cases", NewCase());
        var caseResponse = await created.Content.ReadFromJsonAsync<CaseResponse>();

        var response = await _client.PutAsJsonAsync($"/cases/{caseResponse!.Id}",
            new UpdateCaseRequest("Update Title", "John Doe", DateTime.UtcNow.AddDays(14), CaseStatus.Open, "Updated"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCase_WithUnknownId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await _client.PutAsJsonAsync($"/cases/{Guid.NewGuid()}",
            new UpdateCaseRequest("Title", "Jane Doe", DateTime.UtcNow.AddDays(14), CaseStatus.Open, "Synopsis"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CloseCase_WithValidEvidenceCount_ReturnsNoContent()
    {
        await AuthenticateAsAdminAsync();
        var created = await _client.PostAsJsonAsync("/cases", NewCase());
        var caseResponse = await created.Content.ReadFromJsonAsync<CaseResponse>();

        var response = await _client.PostAsJsonAsync($"/cases/{caseResponse!.Id}/CloseCaseAsync", new CloseCaseRequest(2));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CloseCase_WithUnknownId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await _client.PostAsJsonAsync($"/cases/{Guid.NewGuid()}/CloseCaseAsync", new CloseCaseRequest(2));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCase_RemovesCase_ReturnsNoContent()
    {
        await AuthenticateAsAdminAsync();
        var created = await _client.PostAsJsonAsync("/cases", NewCase());
        var caseResponse = await created.Content.ReadFromJsonAsync<CaseResponse>();

        var response = await _client.DeleteAsync($"/cases/{caseResponse!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCase_WithUnknownId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await _client.DeleteAsync($"/cases/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddNote_ToExistingCase_ReturnCreated()
    {
        await AuthenticateAsAdminAsync();
        var created = await _client.PostAsJsonAsync("/cases", NewCase());
        var caseResponse=await created.Content.ReadFromJsonAsync<CaseResponse>();

        var response = await _client.PostAsJsonAsync($"/cases/{caseResponse!.Id}/notes",
            new AddCaseNoteRequest("John Smith", "Initial note"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AddNote_WithUnknownCaseId_ReturnsNotFound()
    {
        await AuthenticateAsAdminAsync();

        var response = await _client.PostAsJsonAsync($"/cases/{Guid.NewGuid()}/notes",
            new AddCaseNoteRequest("John Smith", "Note."));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetNotes_ReturnsAllNotesForCase()
    {
        await AuthenticateAsAdminAsync();
        var created= await _client.PostAsJsonAsync("/cases",NewCase()); 
        var caseResponse = await created.Content.ReadFromJsonAsync<CaseResponse>();
        await _client.PostAsJsonAsync($"/cases/{caseResponse!.Id}/notes",
            new AddCaseNoteRequest("John Smith", "First note"));

        await _client.PostAsJsonAsync($"/cases/{caseResponse!.Id}/notes",
           new AddCaseNoteRequest("John Smith", "Second note"));

        var response = await _client.GetAsync($"/cases/{caseResponse.Id}/notes");
        var notes = await response.Content.ReadFromJsonAsync<List<CaseNoteResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, notes!.Count);
    }
}
