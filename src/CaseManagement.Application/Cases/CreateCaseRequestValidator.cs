using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace CaseManagement.Application.Cases;

internal sealed class CreateCaseRequestValidator:AbstractValidator<CreateCaseRequest>
{
    public CreateCaseRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssignedTo).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DueDate).NotEqual(default(DateTime)).WithMessage("Due date is required");
        RuleFor(x => x.CaseStatus).IsInEnum().WithMessage("Case Status must be one of the supported values.");
        RuleFor(x => x.Synopsis).MaximumLength(2000);
    }
}

internal sealed class UpdateCaseRequestValidator : AbstractValidator<UpdateCaseRequest>
{
    public UpdateCaseRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssignedTo).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DueDate).NotEqual(default(DateTime)).WithMessage("Due date is required");
        RuleFor(x => x.CaseStatus).IsInEnum().WithMessage("Case Status must be one of the supported values.");
        RuleFor(x => x.Synopsis).MaximumLength(2000);
    }
}

internal sealed class CaseCloseValidator : AbstractValidator<CloseCaseRequest>
{
    public CaseCloseValidator()
    {
        RuleFor(x => x.EvidenceCount).GreaterThan(0);
    }
}
