using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common.Messaging;

namespace CaseManagement.Application.Cases.Queries.List;

public record ListCasesQuery : IRequest<IReadOnlyList<CaseResponse>>;

