using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Application.Common;
using CaseManagement.Domain.Cases;
using CaseManagement.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CaseManagement.Infrastructure.Persistence;

//public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//    :DbContext(options),IApplicationDbContext

// IdentityDbContext<ApplicationUser> brings in AspNetUsers, AspNetRoles, and
// the other Identity tables alongside our own domain tables.
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<Case> Cases => Set<Case>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<CaseNote> CaseNotes => Set<CaseNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //picks up every IEntityTypeConfiguration in this assembly automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
