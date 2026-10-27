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

public record CloseCaseRequest(int EvidenceCount);

public record CaseResponse(
    Guid Id,
    string Title,
    string AssignedTo,
    DateTime DueDate,
    string CaseStatus,
    string Synopsis,
    int EvidenceCount);

public record CaseNoteResponse(
    Guid Id,
    Guid CaseId,
    string AuthorName,
    string Content,
    DateTime CreatedAtUtc);

public record AddCaseNoteRequest(
    string AuthorName,
    string Content);//CaseId comes from the route, not the request body.
