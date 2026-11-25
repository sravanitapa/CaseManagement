using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common.Messaging;

namespace CaseManagement.Application.Cases.Queries.Get;

public record GetCaseQuery(Guid Id) : IRequest<CaseResponse>;

