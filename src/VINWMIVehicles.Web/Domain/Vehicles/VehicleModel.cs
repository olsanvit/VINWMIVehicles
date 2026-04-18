namespace VINWMIVehicles.Domain.Vehicles;

using VINWMIVehicles.Domain;
using VINWMIVehicles.Domain.Vin;

public class VehicleModel
{
    public Guid Id { get; set; }
    /// <summary>Full commercial name including trim. E.g. "Golf 1.5 TSI Life".</summary>
    public string Name { get; set; } = "";
    public Guid SeriesId { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    /// <summary>Trim level name. E.g. "Comfortline", "R-Line", "xDrive".</summary>
    public string? Trim { get; set; }
    public string? EngineCode { get; set; }       // "EA211", "B57"
    public int? DisplacementCc { get; set; }
    public FuelType? FuelType { get; set; }
    public int? PowerKw { get; set; }
    public int? TorqueNm { get; set; }
    public TransmissionType? TransmissionType { get; set; }
    public int? Gears { get; set; }
    public DriveType? DriveType { get; set; }
    public int? Doors { get; set; }
    public int? Seats { get; set; }
    public int? LengthMm { get; set; }
    public int? WidthMm { get; set; }
    public int? HeightMm { get; set; }
    public int? WeightKg { get; set; }
    public int? MaxSpeedKmh { get; set; }
    public decimal? Acceleration0100s { get; set; }
    public int? Co2gKm { get; set; }
    public decimal? FuelConsumptionL100km { get; set; }
    /// <summary>VDS pattern (chars 4-9 of VIN) that matches this model. Used for reverse lookup.</summary>
    public string? VdsPattern { get; set; }
    public string? Description { get; set; }

    public Series Series { get; set; } = null!;
    public ICollection<VinRecord> VinRecords { get; set; } = new List<VinRecord>();
}
