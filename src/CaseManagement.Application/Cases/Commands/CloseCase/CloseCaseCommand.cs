using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common.Messaging;

namespace CaseManagement.Application.Cases.Commands.CloseCase;

public record CloseCaseCommand(Guid Id, int EvidenceCount) : IRequest<bool>;

