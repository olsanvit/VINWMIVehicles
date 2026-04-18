namespace VINWMIVehicles.Domain.Vehicles;

using VINWMIVehicles.Domain;

public class VehicleClass
{
    public Guid Id { get; set; }
    /// <summary>EU/UN ECE class code. E.g. "M1", "N1", "L3e", "O2".</summary>
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public VehicleCategory Category { get; set; }

    public ICollection<VehicleType> VehicleTypes { get; set; } = new List<VehicleType>();
}
