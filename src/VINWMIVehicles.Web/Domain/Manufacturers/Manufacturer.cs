namespace VINWMIVehicles.Domain.Manufacturers;

using VINWMIVehicles.Domain.Geography;
using VINWMIVehicles.Domain.Vehicles;

public class Manufacturer
{
    public Guid Id { get; set; }
    /// <summary>Common English name. E.g. "Volkswagen".</summary>
    public string Name { get; set; } = "";
    /// <summary>Official legal entity name.</summary>
    public string? NameOfficial { get; set; }
    /// <summary>Name in local language.</summary>
    public string? NameLocal { get; set; }
    public Guid? CountryId { get; set; }
    public int? Founded { get; set; }          // year
    public int? Dissolved { get; set; }        // year, null = still active
    public bool IsActive { get; set; } = true;
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? HeadquartersCity { get; set; }
    public string? StockSymbol { get; set; }
    public string? Description { get; set; }
    /// <summary>Owning corporation. E.g. Škoda → Volkswagen Group.</summary>
    public Guid? ParentManufacturerId { get; set; }

    public Country? Country { get; set; }
    public Manufacturer? ParentManufacturer { get; set; }
    public ICollection<Manufacturer> Subsidiaries { get; set; } = new List<Manufacturer>();
    public ICollection<WmiAssignment> WmiAssignments { get; set; } = new List<WmiAssignment>();
    public ICollection<WmcEntry> WmcEntries { get; set; } = new List<WmcEntry>();
    public ICollection<Brand> Brands { get; set; } = new List<Brand>();
}
