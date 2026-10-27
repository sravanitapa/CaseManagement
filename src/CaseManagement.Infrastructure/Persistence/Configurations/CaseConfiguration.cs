using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CaseManagement.Domain.Cases;

namespace CaseManagement.Infrastructure.Persistence.Configurations;

public sealed class CaseConfiguration:IEntityTypeConfiguration<Case>
{
    public void Configure(EntityTypeBuilder<Case> builder)
    {
        builder.ToTable("cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.AssignedTo)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Synopsis)
            .HasMaxLength(2000);

        //store the enum as a readable string column instead of an int.
        builder.Property(c => c.CaseStatus).
            HasConversion<string>()
            .HasMaxLength(40);
    }
}
