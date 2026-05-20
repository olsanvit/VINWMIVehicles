using FluentAssertions;
using SharedServices.Models.VINWMIVehicles.Manufacturers;
using SharedServices.Models.VINWMIVehicles.Vin;

namespace VINWMIVehicles.Tests;

/// <summary>
/// Tests WMI (World Manufacturer Identifier) rules:
/// WMI = first 3 characters of a VIN; manufacturer lookup by WMI prefix.
/// </summary>
public class WmiTests
{
    // ── WMI extraction ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("1HGCM82633A004352", "1HG")]
    [InlineData("WAUZZZ8K8AA123456", "WAU")]
    [InlineData("JN1AZ4EH8FM402369", "JN1")]
    public void Wmi_IsFirstThreeCharsOfVin(string vin, string expectedWmi)
    {
        var wmi = vin[..3];
        wmi.Should().Be(expectedWmi);
    }

    [Fact]
    public void VinRecord_WmiField_MatchesFirst3CharsOfVin()
    {
        var vin = "WAUZZZ8K8AA123456";
        var record = new VinRecord
        {
            Vin = vin,
            Wmi = vin[..3],
            Vds = vin[3..9],
            Vis = vin[9..]
        };

        record.Wmi.Should().Be("WAU");
        record.Wmi.Length.Should().Be(3);
    }

    // ── WmiAssignment lookup ──────────────────────────────────────────────────

    [Fact]
    public void WmiAssignment_Wmi_HasLengthOfThree()
    {
        var assignment = new WmiAssignment { Wmi = "1HG" };
        assignment.Wmi.Length.Should().Be(3);
    }

    [Fact]
    public void WmiAssignment_CanBeLinkedToManufacturer()
    {
        var manufacturerId = Guid.NewGuid();
        var assignment = new WmiAssignment
        {
            Wmi = "1HG",
            ManufacturerId = manufacturerId,
            IsActive = true
        };

        assignment.ManufacturerId.Should().Be(manufacturerId);
        assignment.IsActive.Should().BeTrue();
    }

    [Fact]
    public void WmiAssignment_IsActiveDefault_IsTrue()
    {
        var assignment = new WmiAssignment { Wmi = "WAU" };
        assignment.IsActive.Should().BeTrue();
    }

    [Fact]
    public void WmiAssignment_YearFrom_IsOptional()
    {
        var assignment = new WmiAssignment { Wmi = "JN1", YearFrom = null };
        assignment.YearFrom.Should().BeNull();
    }

    [Fact]
    public void WmiAssignment_YearRange_YearFromLessOrEqualYearTo()
    {
        var assignment = new WmiAssignment { Wmi = "1HG", YearFrom = 1990, YearTo = 2005 };
        assignment.YearFrom!.Value.Should().BeLessThanOrEqualTo(assignment.YearTo!.Value);
    }

    // ── Manufacturer lookup by WMI prefix ─────────────────────────────────────

    [Fact]
    public void MultipleWmiAssignments_CanBelongToSameManufacturer()
    {
        var manufacturerId = Guid.NewGuid();
        var assignments = new List<WmiAssignment>
        {
            new() { Wmi = "1HG", ManufacturerId = manufacturerId },
            new() { Wmi = "2HG", ManufacturerId = manufacturerId },
            new() { Wmi = "3HG", ManufacturerId = manufacturerId }
        };

        assignments.Should().AllSatisfy(a => a.ManufacturerId.Should().Be(manufacturerId));
        assignments.Should().HaveCount(3);
    }

    [Fact]
    public void FindManufacturerByWmi_ReturnsCorrectAssignment()
    {
        var assignments = new List<WmiAssignment>
        {
            new() { Wmi = "1HG", ManufacturerId = Guid.NewGuid() },
            new() { Wmi = "WAU", ManufacturerId = Guid.NewGuid() },
            new() { Wmi = "JN1", ManufacturerId = Guid.NewGuid() }
        };

        var found = assignments.FirstOrDefault(a => a.Wmi == "WAU");
        found.Should().NotBeNull();
        found!.Wmi.Should().Be("WAU");
    }
}
