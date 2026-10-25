using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseManagement.Application.Cases.Queries.Get;

public class GetCaseQueryHandler(IApplicationDbContext applicationDbContext):IRequestHandler<GetCaseQuery,CaseResponse?>
{
    public async Task<CaseResponse?> Handle(GetCaseQuery request,CancellationToken cancellationToken)
    {
        var caseEntity = await applicationDbContext.Cases
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if(caseEntity is null)
        {
            return null;
        }

        return new CaseResponse(caseEntity.Id, caseEntity.Title, caseEntity.AssignedTo, caseEntity.DueDate, caseEntity.CaseStatus.ToString(),
            caseEntity.Synopsis, caseEntity.EvidenceCount);
    }
}
