using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace CaseManagement.Application.Cases.Commands.AddCaseNote;

public class AddCaseNoteCommandValidator:AbstractValidator<AddCaseNoteCommand>
{
    public AddCaseNoteCommandValidator()
    {
        RuleFor(x => x.CaseId).NotEmpty().WithMessage("Case Id is required.");
        RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(2000);
    }
}
