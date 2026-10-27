using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseManagement.Application.Cases.Queries.List;

public class ListCasesQueryHandler(IApplicationDbContext applicationDbContext):IRequestHandler<ListCasesQuery, IReadOnlyList<CaseResponse>>
{
    public async Task<IReadOnlyList<CaseResponse>> Handle(ListCasesQuery request,CancellationToken cancellationToken)
    {
        return await applicationDbContext.Cases
            .AsNoTracking()
            .OrderByDescending(m => m.DueDate)
            .Select(p => new CaseResponse(p.Id, p.Title, p.AssignedTo, p.DueDate,
            p.CaseStatus.ToString(), p.Synopsis, p.EvidenceCount))
            .ToListAsync(cancellationToken);
    }
}
