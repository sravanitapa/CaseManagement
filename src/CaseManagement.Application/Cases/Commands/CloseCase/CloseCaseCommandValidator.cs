using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace CaseManagement.Application.Cases.Commands.CloseCase;

public class CloseCaseCommandValidator:AbstractValidator<CloseCaseCommand>
{
    public CloseCaseCommandValidator()
    {
        RuleFor(x => x.EvidenceCount).GreaterThan(0);
    }
}
