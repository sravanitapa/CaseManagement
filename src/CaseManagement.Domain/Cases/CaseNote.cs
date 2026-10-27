using System;
using System.Collections.Generic;
using System.Text;

namespace CaseManagement.Domain.Cases;

public class CaseNote:Entity
{
    public Guid Id { get; private set; }
    public Guid CaseId { get; private set; }

    public string AuthorName { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public DateTime CreatedAtUtc { get; private set; }
    private CaseNote()
    {
    }
    internal CaseNote(Guid caseId, string authorName, string content)
    {
        Id = Guid.NewGuid();
        CaseId = caseId;
        AuthorName = authorName;
        Content = content;
        CreatedAtUtc = DateTime.UtcNow;
    }

}
