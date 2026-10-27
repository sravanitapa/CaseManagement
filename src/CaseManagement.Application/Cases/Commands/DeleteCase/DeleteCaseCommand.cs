using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CaseManagement.Application.Cases.Commands.DeleteCase;

public record DeleteCaseCommand(Guid Id) : IRequest<bool>;
