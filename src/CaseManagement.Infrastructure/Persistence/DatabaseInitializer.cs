using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Domain.Cases;
using CaseManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CaseManagement.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services,CancellationToken cancellationToken=default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        // On a fresh database EF logs an error reading "_EFMigrationsHistory"
        // before that table exists - it catches it and creates the schema. Expected.
        logger.LogInformation("Applying datbase migrations...");
        await context.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Database migrations applied...");

        await SeedAsync(context, logger, cancellationToken);
    }

    private static async Task SeedAsync(ApplicationDbContext context,ILogger logger,CancellationToken cancellationToken)
    {
        //Idempotent: if the table has already has cases, leave the data untouched.
        if(await context.Cases.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Datbase already seeded, skipping.");
        }

        Case[] cases =
        [
            Case.Create("Customer Complaint - Incorrect Invoice",
            "John Smith.",new DateTime(2026, 7, 2, 17, 0, 0, DateTimeKind.Utc),CaseStatus.Open,
            "Investigate the invoice generation process and verify the billing records."),
            Case.Create("VPN Connection Failure",
            "Emily Johnson.",new DateTime(2026, 7, 2, 17, 0, 0, DateTimeKind.Utc),CaseStatus.InProgress,
            "Analyze VPN server logs and network connectivity to identify the root cause."),
            Case.Create("Employee Access Request",
            "David Wilson.",new DateTime(2026, 7, 2, 17, 0, 0, DateTimeKind.Utc),CaseStatus.InProgress,
            "Verify onboarding documents and provision the required system access.")
        ];
        context.Cases.AddRange(cases);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} cases", cases.Length);
    }
}
