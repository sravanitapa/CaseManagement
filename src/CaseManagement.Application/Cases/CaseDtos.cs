using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Domain.Cases;

namespace CaseManagement.Application.Cases;

public record CreateCaseRequest(
    string Title,
    string AssignedTo,
    DateTime DueDate,
    CaseStatus CaseStatus,
    string Synopsis);

public record UpdateCaseRequest(
    string Title,
    string AssignedTo,
    DateTime DueDate,
    CaseStatus CaseStatus,
    string Synopsis);

public record CloseCaseRequest(CaseStatus CaseStatus, int evidenceCount);

public record CaseResponse(
    Guid Id,
    string Title,
    string AssignedTo,
    DateTime DueDate,
    string CaseStatus,
    string Synopsis,
    int evidenceCount);
