using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using CaseManagement.Domain.Cases;
using Microsoft.EntityFrameworkCore;

namespace CaseManagement.Application.Cases;

public sealed class CaseService(IApplicationDbContext context):ICaseService
{
    public async Task<CaseResponse> CreateAsync(CreateCaseRequest request,CancellationToken cancellationToken)
    {
        var caseEntity = Case.Create(request.Title, request.AssignedTo, request.DueDate, request.CaseStatus, request.Synopsis);
        context.Cases.Add(caseEntity);
        await context.SaveChangesAsync(cancellationToken);

        return caseEntity.ToResponse();
    }

    public async Task<CaseResponse?> GetByIdAsync(Guid id,CancellationToken cancellationToken)
    {
        var caseEntity = await context.Cases
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        return caseEntity?.ToResponse();
    }

    public async Task<IReadOnlyList<CaseResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cases = await context.Cases
            .AsNoTracking()
            .OrderByDescending(m => m.DueDate)
            .ToListAsync(cancellationToken);

        return cases.Select(m => m.ToResponse()).ToList();
    }

    public async Task<bool> UpdateAsync(Guid id,UpdateCaseRequest request,CancellationToken cancellationToken)
    {
        var caseEntity = await context.Cases.FirstOrDefaultAsync(m => m.Id == id,cancellationToken);
        if(caseEntity is null)
        {
            return false;
        }

        caseEntity.UpdateDetails(request.Title, request.AssignedTo, request.DueDate, request.Synopsis);

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CloseCaseAsync(Guid id,CloseCaseRequest closeCaseRequest,CancellationToken cancellationToken)
    {
        var caseEntity = await context.Cases.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if(caseEntity is null)
        {
            return false;
        }

        caseEntity.CloseCase(closeCaseRequest.evidenceCount);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id,CancellationToken cancellationToken)
    {
        var caseEntity = await context.Cases.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if(caseEntity is null)
        {
            return false;
        }

        context.Cases.Remove(caseEntity);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

}
