using FluentAssertions;
using SharedServices.Models.VINWMIVehicles.Manufacturers;

namespace VINWMIVehicles.Tests;

/// <summary>
/// Tests the Manufacturer model: Name required, CountryId is optional Guid.
/// </summary>
public class ManufacturerTests
{
    [Fact]
    public void Manufacturer_Name_DefaultsToEmptyString()
    {
        var m = new Manufacturer();
        m.Name.Should().NotBeNull();
        m.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void Manufacturer_Name_CanBeSet()
    {
        var m = new Manufacturer { Name = "Volkswagen" };
        m.Name.Should().Be("Volkswagen");
    }

    [Fact]
    public void Manufacturer_Name_Required_ShouldNotBeEmpty_ForValidRecord()
    {
        var m = new Manufacturer { Name = "BMW" };
        m.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Manufacturer_CountryId_IsNullableGuid()
    {
        // By default null
        var m = new Manufacturer();
        m.CountryId.Should().BeNull();

        // Can be set to a valid Guid
        var countryId = Guid.NewGuid();
        m.CountryId = countryId;
        m.CountryId.Should().Be(countryId);
        m.CountryId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Manufacturer_IsActive_DefaultsToTrue()
    {
        var m = new Manufacturer();
        m.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Manufacturer_Subsidiaries_DefaultsToEmptyCollection()
    {
        var m = new Manufacturer();
        m.Subsidiaries.Should().NotBeNull();
        m.Subsidiaries.Should().BeEmpty();
    }

    [Fact]
    public void Manufacturer_WmiAssignments_DefaultsToEmptyCollection()
    {
        var m = new Manufacturer();
        m.WmiAssignments.Should().NotBeNull();
        m.WmiAssignments.Should().BeEmpty();
    }

    [Fact]
    public void Manufacturer_OptionalFields_AreNullByDefault()
    {
        var m = new Manufacturer();
        m.NameOfficial.Should().BeNull();
        m.NameLocal.Should().BeNull();
        m.Founded.Should().BeNull();
        m.Dissolved.Should().BeNull();
        m.Website.Should().BeNull();
        m.ParentManufacturerId.Should().BeNull();
    }

    [Fact]
    public void Manufacturer_ParentManufacturerId_CanBeSet()
    {
        var parentId = Guid.NewGuid();
        var m = new Manufacturer { ParentManufacturerId = parentId };
        m.ParentManufacturerId.Should().Be(parentId);
    }
}
