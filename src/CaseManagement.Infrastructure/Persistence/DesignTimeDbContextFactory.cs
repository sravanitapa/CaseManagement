using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CaseManagement.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory:IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=casesdb;Username=postgres;Password=postgres")
            .Options;
        return new ApplicationDbContext(options);
    }
}
