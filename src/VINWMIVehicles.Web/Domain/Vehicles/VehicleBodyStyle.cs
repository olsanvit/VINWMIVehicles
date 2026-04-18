namespace VINWMIVehicles.Domain.Vehicles;

public class VehicleBodyStyle
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";        // "Sedan", "Hatchback", "Estate", "SUV", "Coupe", "Convertible", "MPV", "Pickup", "Van"
    public string? Description { get; set; }
    public Guid VehicleTypeId { get; set; }
    /// <summary>Typical door count options, comma-separated. E.g. "3,5".</summary>
    public string? TypicalDoorOptions { get; set; }

    public VehicleType VehicleType { get; set; } = null!;
    public ICollection<Series> Series { get; set; } = new List<Series>();
}
