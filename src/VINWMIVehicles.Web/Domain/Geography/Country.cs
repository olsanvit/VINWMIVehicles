namespace VINWMIVehicles.Domain.Geography;

using VINWMIVehicles.Domain.Manufacturers;
using VINWMIVehicles.Domain.Vehicles;

public class Country
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? NameLocal { get; set; }
    public string IsoCode2 { get; set; } = "";   // "CZ", "US", "DE"
    public string? IsoCode3 { get; set; }         // "CZE", "USA", "DEU"
    public Guid RegionId { get; set; }

    public Region Region { get; set; } = null!;
    public ICollection<Manufacturer> Manufacturers { get; set; } = new List<Manufacturer>();
    public ICollection<Brand> Brands { get; set; } = new List<Brand>();
}
