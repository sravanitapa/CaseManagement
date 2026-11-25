using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common.Messaging;

namespace CaseManagement.Application.Cases.Queries.GetCaseNotes;

public record GetCaseNotesQuery(Guid CaseId) : IRequest<IReadOnlyList<CaseNoteResponse>>;

