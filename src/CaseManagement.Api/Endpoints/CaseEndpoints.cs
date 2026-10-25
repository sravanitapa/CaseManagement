using CaseManagement.Api.Infrastructure;
using CaseManagement.Application.Cases;
using CaseManagement.Application.Cases.Commands.AddCaseNote;
using CaseManagement.Application.Cases.Commands.CloseCase;
using CaseManagement.Application.Cases.Commands.CreateCase;
using CaseManagement.Application.Cases.Commands.DeleteCase;
using CaseManagement.Application.Cases.Commands.UpdateCase;
using CaseManagement.Application.Cases.Queries.Get;
using CaseManagement.Application.Cases.Queries.GetCaseNotes;
using CaseManagement.Application.Cases.Queries.List;
using CaseManagement.Infrastructure.Identity.Entities;
using MediatR;

namespace CaseManagement.Api.Endpoints;

public static class CaseEndpoints
{
    public static IEndpointRouteBuilder MapCaseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cases").WithTags("Cases");

        group.MapPost("/", async (CreateCaseCommand command, ISender mediator) =>
        {
            var caseResponse = await mediator.Send(command);
            return Results.Created($"/cases/{caseResponse.Id}", caseResponse);
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapGet("/", async (ISender mediator) =>
        {
            var cases = await mediator.Send(new ListCasesQuery());
            return Results.Ok(cases);
        }).RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
        {
            var caseEntity = await mediator.Send(new GetCaseQuery(id));
            if(caseEntity is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(caseEntity);
        }).RequireAuthorization();

        group.MapPut("/{id:guid}", async (Guid id,UpdateCaseRequest request, ISender mediator) =>
        {
            var command = new UpdateCaseCommand(id, request.Title, request.AssignedTo, request.DueDate, request.Synopsis);
            var updated = await mediator.Send(command);
            return updated ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapPost("/{id:guid}/CloseCaseAsync", async (Guid id,CloseCaseRequest request,ISender mediator) =>
        {
            var command = new CloseCaseCommand(id,request.EvidenceCount);
            var caseEntityStatus = await mediator.Send(command);
            return caseEntityStatus ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapDelete("/{id:guid}", async(Guid id,ISender mediator) =>
        {
            var command = new DeleteCaseCommand(id);
            var deleted = await mediator.Send(command);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapPost("/{id:guid}/notes", async (Guid id, AddCaseNoteRequest request, ISender mediator) =>
        {
            var command = new AddCaseNoteCommand(id, request.AuthorName, request.Content);
            var caseNote = await mediator.Send(command);
            return caseNote is null ? Results.NotFound() :
            Results.Created($"/cases/{id}/notes/{caseNote.Id}", caseNote);
        }).RequireAuthorization(policy => policy.RequireRole(Roles.Admin));

        group.MapGet("/{id:guid}/notes", async (Guid id, ISender mediator) =>
        {
            var caseNotes = await mediator.Send(new GetCaseNotesQuery(id));
            return Results.Ok(caseNotes);
        }).RequireAuthorization();
        return app;
    }
}
