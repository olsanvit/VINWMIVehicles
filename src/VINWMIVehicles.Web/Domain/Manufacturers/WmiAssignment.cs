namespace VINWMIVehicles.Domain.Manufacturers;

using VINWMIVehicles.Domain;

public class WmiAssignment
{
    public Guid Id { get; set; }
    /// <summary>3-character WMI code. First 3 chars of VIN.</summary>
    public string Wmi { get; set; } = "";
    public Guid ManufacturerId { get; set; }
    /// <summary>Year this WMI was first assigned to this manufacturer. Null = unknown.</summary>
    public int? YearFrom { get; set; }
    /// <summary>Year this WMI was last valid for this manufacturer. Null = still valid.</summary>
    public int? YearTo { get; set; }
    /// <summary>What vehicle category this WMI applies to (some WMIs are type-specific).</summary>
    public string? VehicleTypeScope { get; set; }
    public DataSource Source { get; set; } = DataSource.Manual;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    public Manufacturer Manufacturer { get; set; } = null!;
}
