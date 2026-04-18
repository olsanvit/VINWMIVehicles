namespace VINWMIVehicles.Domain.Vehicles;

public class VehicleType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";        // "Car", "Motorcycle", "Truck", "Bus", "Van"
    public string? Code { get; set; }
    public Guid? VehicleClassId { get; set; }

    public VehicleClass? VehicleClass { get; set; }
    public ICollection<VehicleBodyStyle> BodyStyles { get; set; } = new List<VehicleBodyStyle>();
}
