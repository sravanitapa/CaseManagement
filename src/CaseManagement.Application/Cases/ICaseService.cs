using System;
using System.Collections.Generic;
using System.Text;

namespace CaseManagement.Application.Cases;

public interface ICaseService
{
    Task<CaseResponse> CreateAsync(CreateCaseRequest request, CancellationToken cancellationToken);
    Task<CaseResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CaseResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> UpdateAsync(Guid id, UpdateCaseRequest request, CancellationToken cancellationToken);
    Task<bool> CloseCaseAsync(Guid id, CloseCaseRequest closeCaseRequest, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid guid, CancellationToken cancellationToken);
}
