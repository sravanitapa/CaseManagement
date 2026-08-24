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
        Assert.Equal(Guid.Empty, caseEntity.Id);
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
}
