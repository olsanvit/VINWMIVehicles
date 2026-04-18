namespace VINWMIVehicles.Domain.Geography;

public class Region
{
    public Guid Id { get; set; }
    /// <summary>First character(s) of VIN that identify this region. E.g. "1,2,3,4,5" for North America.</summary>
    public string VinChars { get; set; } = "";   // comma-separated chars
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    public ICollection<Country> Countries { get; set; } = new List<Country>();
}
