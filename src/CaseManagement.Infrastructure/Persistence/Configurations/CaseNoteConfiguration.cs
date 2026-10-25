using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CaseManagement.Domain.Cases;

namespace CaseManagement.Infrastructure.Persistence.Configurations;

public sealed class CaseNoteConfiguration:IEntityTypeConfiguration<CaseNote>
{
    public void Configure(EntityTypeBuilder<CaseNote> builder)
    {
        builder.ToTable("case_notes");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedNever();

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.AuthorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<Case>()
            .WithMany(c => c.CaseNotes)
            .HasForeignKey(n => n.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
