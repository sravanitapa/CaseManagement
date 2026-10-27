using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseManagement.Application.Cases.Commands.DeleteCase;

public class DeleteCaseCommandHandler(IApplicationDbContext applicationDbContext):IRequestHandler<DeleteCaseCommand,bool>
{
    public async Task<bool> Handle(DeleteCaseCommand request,CancellationToken cancellationToken)
    {
        var caseEntity = await applicationDbContext.Cases.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if(caseEntity is null)
        {
            return false;
        }

        applicationDbContext.Cases.Remove(caseEntity);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
