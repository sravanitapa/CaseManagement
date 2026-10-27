using System;
using System.Collections.Generic;
using System.Text;

namespace CaseManagement.Domain.Cases;

public enum CaseStatus
{
    Open=1,
    InProgress=2,
    DraftComplete=3,
    TechReview=4,
    AdminReview=5,
    Closed=6
}
