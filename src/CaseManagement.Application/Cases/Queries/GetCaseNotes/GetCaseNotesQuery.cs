using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CaseManagement.Application.Cases.Queries.GetCaseNotes;

public record GetCaseNotesQuery(Guid CaseId) : IRequest<IReadOnlyList<CaseNoteResponse>>;

