using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common.Messaging;


namespace CaseManagement.Application.Cases.Commands.UpdateCase;

public record UpdateCaseCommand(Guid Id,string Title, string AssignedTo, DateTime DueDate, string Synopsis) : IRequest<bool>;

