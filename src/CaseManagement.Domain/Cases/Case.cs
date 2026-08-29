using System;
using System.Collections.Generic;
using System.Text;

namespace CaseManagement.Domain.Cases;

public sealed class Case : Entity
{
    //EF Core needs a parameterless constructor.keeping it private means the
    //rest of the application cannot create a Case in an invalid state.
    private Case()
    {
    }

    private Case(string title, string assignedTo, DateTime dueDate, CaseStatus caseStatus, string synopsis)
    {
        Title = title;
        AssignedTo = assignedTo;
        CreatedAtUtc = DateTime.UtcNow;
        CaseStatus = caseStatus;
        Synopsis = synopsis;
        DueDate = dueDate;
    }
    public string Title { get; private set; } = default!;//I know this property doesn't have a value right now, but don't warn me. I promise I'll initialize it later.
    //public string Description { get; private set; } = default!;
    public CaseStatus CaseStatus { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string AssignedTo { get; private set; } = default!;
    public double? RiskScore { get; private set; }
    public int EvidenceCount { get; private set; }
    public string Synopsis { get; private set; } = default!;
    public DateTime DueDate { get; private set; }

    //A factory method is the only way to build a Case.It enforces the rules
    //that must always be true,so an invalid Case can never exist.
    public static Case Create(string title, string assignedTo, DateTime dueDate, CaseStatus caseStatus, string synopsis)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("A case must have a title.");
        }
        if (string.IsNullOrEmpty(assignedTo))
        {
            throw new DomainException("A case must have AssignedTo to Analyst");
        }

        return new Case(title.Trim(), assignedTo.Trim(), dueDate, caseStatus, synopsis?.Trim() ?? string.Empty);
    }

    public void UpdateDetails(string title, string assignedTo, DateTime dueDate, string synopsis)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("A case must have a title.");
        }

        Title = title;
        AssignedTo = assignedTo;
        DueDate = dueDate;
        Synopsis = synopsis?.Trim() ?? string.Empty;
    }

    public void StartInvestigation()
    {
        if (CaseStatus != CaseStatus.Open)
        {
            throw new DomainException("Only open cases can start investigation");
        }
        CaseStatus = CaseStatus.InProgress;
    }

    public void CloseCase(int evidenceCount)
    {
        if (CaseStatus == CaseStatus.Closed)
        {
            throw new DomainException("Case is already closed.");
        }
        if (evidenceCount <= 0)
        {
            throw new DomainException("Cannot close a case without evidence");
        }
        EvidenceCount = evidenceCount;
        CaseStatus = CaseStatus.Closed;
    }

}

