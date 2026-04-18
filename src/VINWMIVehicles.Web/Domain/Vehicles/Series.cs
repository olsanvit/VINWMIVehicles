namespace VINWMIVehicles.Domain.Vehicles;

public class Series
{
    public Guid Id { get; set; }
    /// <summary>Commercial name. E.g. "Golf", "Octavia", "Corolla", "3 Series".</summary>
    public string Name { get; set; } = "";
    public string? Slug { get; set; }
    public Guid BrandId { get; set; }
    public Guid? VehicleBodyStyleId { get; set; }
    /// <summary>Generation label. E.g. "Mk8", "A8", "F30".</summary>
    public string? GenerationName { get; set; }
    public int? GenerationFrom { get; set; }
    public int? GenerationTo { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public Brand Brand { get; set; } = null!;
    public VehicleBodyStyle? VehicleBodyStyle { get; set; }
    public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
}
