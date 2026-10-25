using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Domain.Cases;

namespace CaseManagement.Application.Cases;

internal static class CaseMappings
{
    public static CaseResponse ToResponse(this Case caseEntity) => new(
       caseEntity.Id,
       caseEntity.Title,
       caseEntity.AssignedTo,
       caseEntity.DueDate,
       caseEntity.CaseStatus.ToString(),
       caseEntity.Synopsis,
       caseEntity.EvidenceCount);

    public static CaseNoteResponse ToResponse(this CaseNote caseNote) => new(
        caseNote.Id,
        caseNote.CaseId,
        caseNote.AuthorName,
        caseNote.Content,
        caseNote.CreatedAtUtc);
}
