using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using CaseManagement.Domain.Cases;
using MediatR;

namespace CaseManagement.Application.Cases.Commands.CreateCase;

public class CreateCaseCommandHandler(IApplicationDbContext applicationDbContext):IRequestHandler<CreateCaseCommand,CaseResponse>
{
    public async Task<CaseResponse> Handle(CreateCaseCommand request,CancellationToken cancellationToken)
    {
        var caseEntity = Case.Create(request.Title, request.AssignedTo, request.DueDate, request.CaseStatus, request.Synopsis);
        applicationDbContext.Cases.Add(caseEntity);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return caseEntity.ToResponse();
    }
}
