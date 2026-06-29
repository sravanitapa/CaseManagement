using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Domain.Cases;
using Microsoft.EntityFrameworkCore;

namespace CaseManagement.Application.Common;

// This interface is how the Application layer talks to the database without
// depending on the Infrastructure project. It exposes the DbSet directly, so
// services get the full power of EF Core and LINQ - no repository in between.
public interface IApplicationDbContext
{
    DbSet<Case> Cases { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
