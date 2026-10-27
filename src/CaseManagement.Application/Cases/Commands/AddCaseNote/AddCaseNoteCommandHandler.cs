using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using MediatR;

namespace CaseManagement.Application.Cases.Commands.AddCaseNote;

public class AddCaseNoteCommandHandler(IApplicationDbContext applicationDbContext):IRequestHandler<AddCaseNoteCommand,CaseNoteResponse?>
{
    public async Task<CaseNoteResponse?> Handle(AddCaseNoteCommand request, CancellationToken cancellationToken)
    {
        var caseEntity = await applicationDbContext.Cases.FindAsync(new object[] { request.CaseId }, cancellationToken);
        if (caseEntity == null)
        {
            return null;
        }
        var caseNote = caseEntity.AddCaseNote(request.AuthorName, request.Content);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return caseNote.ToResponse();
    }
}
