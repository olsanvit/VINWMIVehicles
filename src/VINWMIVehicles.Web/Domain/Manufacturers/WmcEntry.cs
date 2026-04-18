namespace VINWMIVehicles.Domain.Manufacturers;

using VINWMIVehicles.Domain;

public class WmcEntry
{
    public Guid Id { get; set; }
    /// <summary>The code value. Format depends on CodeType.</summary>
    public string Code { get; set; } = "";
    public WmcCodeType CodeType { get; set; }
    public Guid ManufacturerId { get; set; }
    public int? ValidFrom { get; set; }
    public int? ValidTo { get; set; }
    public DataSource Source { get; set; } = DataSource.Manual;
    public string? Description { get; set; }

    public Manufacturer Manufacturer { get; set; } = null!;
}
