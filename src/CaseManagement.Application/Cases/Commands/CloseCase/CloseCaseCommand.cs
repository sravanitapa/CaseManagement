using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CaseManagement.Application.Cases.Commands.CloseCase;

public record CloseCaseCommand(Guid Id, int EvidenceCount) : IRequest<bool>;

