using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CaseManagement.Application.Cases.Queries.Get;

public record GetCaseQuery(Guid Id) : IRequest<CaseResponse>;

