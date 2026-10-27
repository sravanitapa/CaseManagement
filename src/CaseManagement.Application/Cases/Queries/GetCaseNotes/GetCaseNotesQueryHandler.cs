using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CaseManagement.Application.Common;

namespace CaseManagement.Application.Cases.Queries.GetCaseNotes;

public class GetCaseNotesQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetCaseNotesQuery, IReadOnlyList<CaseNoteResponse>>
{
    public async Task<IReadOnlyList<CaseNoteResponse>> Handle(GetCaseNotesQuery request, CancellationToken cancellationToken)
    {
        var caseNotes = await applicationDbContext.CaseNotes
            .AsNoTracking()
            .Where(m => m.CaseId == request.CaseId)
            .Select(m => new CaseNoteResponse(m.Id, m.CaseId, m.AuthorName, m.Content, m.CreatedAtUtc))
            .ToListAsync(cancellationToken);
        return caseNotes;
    }
}
