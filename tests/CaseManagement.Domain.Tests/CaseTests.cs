using System;
using System.Collections.Generic;
using System.Text;
using CaseManagement.Domain.Cases;

namespace CaseManagement.Domain.Tests;

public class CaseTests
{
    //A small helper so each test starts from a valid case.
    private static Case CreateValidCase() => Case.Create(
        title: "Customer Complaint - Incorrect Invoice",
        assignedTo: "John Smith",
        dueDate: new DateTime(2026, 7, 2, 17, 0, 0, DateTimeKind.Utc),
        caseStatus: CaseStatus.Open,
        synopsis: "Investigate the invoice generation process and verify the billing records.");

    [Fact]
    public void Create_WithValidData_SetsTheProperties()
    {
        var caseEntity = CreateValidCase();

        Assert.Equal("Customer Complaint - Incorrect Invoice", caseEntity.Title);
        Assert.Equal("John Smith", caseEntity.AssignedTo);
        Assert.Equal(CaseStatus.Open, caseEntity.CaseStatus);
        Assert.NotEqual(Guid.Empty, caseEntity.Id);
    }

    [Fact]
    public void Create_TrimsExtraSpacesFromText()
    {
        var caseEntity = Case.Create(" Employee Laptop Replacement ", " John Smith ", DateTime.UtcNow.AddDays(7), CaseStatus.Open,
            " Replacement request for damaged company laptop. ");

        Assert.Equal("Employee Laptop Replacement", caseEntity.Title);
        Assert.Equal("John Smith", caseEntity.AssignedTo);
        Assert.Equal("Replacement request for damaged company laptop.", caseEntity.Synopsis);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_WithEmptyTitle_Throws(string badTitle)
    {
        var error = Assert.Throws<DomainException>(() =>
        Case.Create(badTitle, "Assigned To", DateTime.UtcNow.AddDays(15), CaseStatus.InProgress, "Replacement request."));

        Assert.Equal("A case must have a title.", error.Message);
    }

    [Fact]
    public void Create_WithEmptyDirector_Throws()
    {
        Assert.Throws<DomainException>(() =>
        Case.Create("A Title", "", DateTime.UtcNow.AddDays(15), CaseStatus.InProgress, "Replacement request."));
    }

    [Fact]
    public void CloseCase_WithValidEvidenceCount_ShouldCloseCase()
    {
        var caseEntity = CreateValidCase();

        caseEntity.CloseCase(2);

        Assert.Equal(CaseStatus.Closed, caseEntity.CaseStatus);    
        Assert.Equal(2, caseEntity.EvidenceCount);
    }

    [Fact]
    public void CloseCase_WithEvidenceCountLessThanAreEqualToZero_ShouldThrow()
    {
        var caseEntity = CreateValidCase();
        //caseEntity.CloseCase(0);
        Assert.Throws<DomainException>(() => caseEntity.CloseCase(0));
    }

    [Fact]
    public void CloseCase_WhenAlreadyClosed_Throws()
    {
        var caseEntity = CreateValidCase();
        caseEntity.CloseCase(2);
        var error=Assert.Throws<DomainException>(() => caseEntity.CloseCase(3));

        Assert.Equal("Case is already closed.", error.Message);
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdateProperties()
    {
        var caseEntity = CreateValidCase();
        caseEntity.UpdateDetails("Updated Title", "Jane Doe", new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc), "Updated synopsis.");
        Assert.Equal("Updated Title", caseEntity.Title);
        Assert.Equal("Jane Doe", caseEntity.AssignedTo);
        Assert.Equal("Updated synopsis.", caseEntity.Synopsis);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void UpdateDetails_WithEmptyTitle_Throws(string badTitle)
    {
        var caseEntity = CreateValidCase();

        Assert.Throws<DomainException>(()=>
        caseEntity.UpdateDetails(badTitle, "Jane Doe", new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc), "Synopsis."));
    }

    [Fact]
    public void AddCaseNote_WithValidData_AddsNoteToCollection()
    {
        var caseEntity = CreateValidCase();

        var note = caseEntity.AddCaseNote("John Smith", "Called the customer, awaiting callback.");

        Assert.Single(caseEntity.CaseNotes);
        Assert.Equal("Called the customer, awaiting callback.", note.Content);
        Assert.Equal(caseEntity.Id, note.CaseId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void AddCaseNote_WithEmptyContent_Throws(string badContent)
    {
        var caseEntity = CreateValidCase();

        Assert.Throws<DomainException>(() =>
        caseEntity.AddCaseNote("John Smith", badContent));
    }
}
