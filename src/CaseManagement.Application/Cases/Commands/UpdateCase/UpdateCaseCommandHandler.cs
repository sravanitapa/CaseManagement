using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaseManagement.Application.Cases.Commands.UpdateCase;

public class UpdateCaseCommandHandler(IApplicationDbContext applicationDbContext):IRequestHandler<UpdateCaseCommand,bool>
{
    public async Task<bool> Handle(UpdateCaseCommand request,CancellationToken cancellationToken)
    {
        var caseEntity = await applicationDbContext.Cases.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if(caseEntity is null)
        {
            return false;
        }

        caseEntity.UpdateDetails(request.Title, request.AssignedTo, request.DueDate, request.Synopsis);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
