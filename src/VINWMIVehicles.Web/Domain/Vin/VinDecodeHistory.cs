namespace VINWMIVehicles.Domain.Vin;

using VINWMIVehicles.Domain;

public class VinDecodeHistory
{
    public Guid Id { get; set; }
    public Guid VinRecordId { get; set; }
    public DateTime DecodedAt { get; set; }
    public DataSource Source { get; set; }
    /// <summary>Full raw response from the decode API.</summary>
    public string? RawResponse { get; set; }
    /// <summary>JSON patch describing what fields changed vs previous decode.</summary>
    public string? ChangedFields { get; set; }

    public VinRecord VinRecord { get; set; } = null!;
}
