using CaseManagement.Api.Infrastructure;
using CaseManagement.Application.Cases;

namespace CaseManagement.Api.Endpoints;

public static class CaseEndpoints
{
    public static IEndpointRouteBuilder MapCaseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/cases").WithTags("Cases");

        group.MapPost("/", async (CreateCaseRequest request, ICaseService service, CancellationToken cancellationToken) =>
        {
            var caseEntity = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/cases/{caseEntity.Id}", caseEntity);
        }).AddEndpointFilter<ValidationFilter<CreateCaseRequest>>();

        group.MapGet("/", async (ICaseService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetAllAsync(cancellationToken)));

        group.MapGet("/{id:guid}", async (Guid id, ICaseService service, CancellationToken cancellationToken) =>
        {
            var caseEntity = await service.GetByIdAsync(id, cancellationToken);
            return caseEntity is null ? Results.NotFound() : Results.Ok(caseEntity);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateCaseRequest request, ICaseService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.UpdateAsync(id, request, cancellationToken);
            return updated ? Results.NoContent() : Results.NotFound();
        }).AddEndpointFilter<ValidationFilter<UpdateCaseRequest>>();

        group.MapPost("/{id:guid}/CloseCaseAsync", async (Guid id, CloseCaseRequest request, ICaseService service, CancellationToken cancellationToken) =>
        {
            var caseEntityStatus = await service.CloseCaseAsync(id, request, cancellationToken);
            return caseEntityStatus ? Results.NoContent() : Results.NotFound();
        }).AddEndpointFilter<ValidationFilter<CloseCaseRequest>>();

        group.MapDelete("/{id:guid}", async (Guid id, ICaseService service, CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
