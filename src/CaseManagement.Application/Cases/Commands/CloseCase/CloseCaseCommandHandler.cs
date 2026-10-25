using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseManagement.Application.Cases.Commands.CloseCase;

public class CloseCaseCommandHandler(IApplicationDbContext applicationDbContext):IRequestHandler<CloseCaseCommand,bool>
{
    public async Task<bool> Handle(CloseCaseCommand request,CancellationToken cancellationToken)
    {
        var caseEntity = await applicationDbContext.Cases.FirstOrDefaultAsync(m => m.Id == request.Id,cancellationToken);
        if(caseEntity is null)
        {
            return false;
        }
        caseEntity.CloseCase(request.EvidenceCount);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
