namespace VINWMIVehicles.Domain.Vehicles;

using VINWMIVehicles.Domain.Geography;
using VINWMIVehicles.Domain.Manufacturers;

public class Brand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? NameOfficial { get; set; }
    /// <summary>URL-friendly slug. E.g. "volkswagen", "skoda".</summary>
    public string Slug { get; set; } = "";
    public Guid ManufacturerId { get; set; }
    /// <summary>Country where the brand originates (may differ from manufacturer country).</summary>
    public Guid? OriginCountryId { get; set; }
    public int? Founded { get; set; }
    public int? Discontinued { get; set; }
    public bool IsActive { get; set; } = true;
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }

    public Manufacturer Manufacturer { get; set; } = null!;
    public Country? OriginCountry { get; set; }
    public ICollection<Series> Series { get; set; } = new List<Series>();
}
