using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CaseManagement.Application.Cases.Commands.AddCaseNote;

public record AddCaseNoteCommand(Guid CaseId, string AuthorName, string Content) : IRequest<CaseNoteResponse?>;

