using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common.Messaging;

namespace CaseManagement.Application.Cases.Commands.DeleteCase;

public record DeleteCaseCommand(Guid Id) : IRequest<bool>;
