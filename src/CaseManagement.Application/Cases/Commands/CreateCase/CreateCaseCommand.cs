using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MediatR;
using CaseManagement.Domain.Cases;

namespace CaseManagement.Application.Cases.Commands.CreateCase;

public record CreateCaseCommand(string Title, string AssignedTo, DateTime DueDate, CaseStatus CaseStatus, string Synopsis) : IRequest<CaseResponse>;

