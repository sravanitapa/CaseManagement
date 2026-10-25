using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CaseManagement.Application.Cases.Queries.List;

public record ListCasesQuery : IRequest<IReadOnlyList<CaseResponse>>;

