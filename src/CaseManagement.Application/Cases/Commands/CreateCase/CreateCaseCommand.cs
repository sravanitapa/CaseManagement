using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CaseManagement.Application.Common.Messaging;
using CaseManagement.Domain.Cases;

namespace CaseManagement.Application.Cases.Commands.CreateCase;

public record CreateCaseCommand(string Title, string AssignedTo, DateTime DueDate, CaseStatus CaseStatus, string Synopsis) : IRequest<CaseResponse>;

