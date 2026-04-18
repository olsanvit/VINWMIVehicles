namespace VINWMIVehicles.Domain.Vin;

using VINWMIVehicles.Domain;
using VINWMIVehicles.Domain.Geography;
using VINWMIVehicles.Domain.Manufacturers;
using VINWMIVehicles.Domain.Vehicles;

public class VinRecord
{
    public Guid Id { get; set; }
    /// <summary>Full 17-character VIN. Always stored uppercase.</summary>
    public string Vin { get; set; } = "";

    // ── Structural decomposition ────────────────────────────────
    /// <summary>Chars 1-3: World Manufacturer Identifier.</summary>
    public string Wmi { get; set; } = "";
    /// <summary>Chars 4-9: Vehicle Descriptor Section.</summary>
    public string Vds { get; set; } = "";
    /// <summary>Chars 10-17: Vehicle Identifier Section.</summary>
    public string Vis { get; set; } = "";
    /// <summary>Char 9: Check digit (0-9 or X).</summary>
    public char CheckDigit { get; set; }
    /// <summary>Char 10: Model year code (A=1980, B=1981...Y=2000, 1=2001...9=2009, A=2010...).</summary>
    public char ModelYearChar { get; set; }
    /// <summary>Char 11: Plant/factory code.</summary>
    public char PlantCode { get; set; }
    /// <summary>Chars 12-17: Sequential production number.</summary>
    public string SequentialNumber { get; set; } = "";

    // ── Decoded fields ──────────────────────────────────────────
    public int? ModelYear { get; set; }
    public bool IsValid { get; set; }
    public string? ValidationErrors { get; set; }

    // ── Foreign keys (resolved by decoding) ────────────────────
    public Guid? ManufacturerId { get; set; }
    public Guid? VehicleModelId { get; set; }
    public Guid? CountryOfManufactureId { get; set; }

    // ── Decode metadata ─────────────────────────────────────────
    public DataSource? DecodedSource { get; set; }
    public DateTime? DecodedAt { get; set; }
    /// <summary>Raw JSON response from NHTSA or other API — audit trail + re-decode source.</summary>
    public string? RawApiResponse { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Manufacturer? Manufacturer { get; set; }
    public VehicleModel? VehicleModel { get; set; }
    public Country? CountryOfManufacture { get; set; }
    public ICollection<VinDecodeHistory> DecodeHistory { get; set; } = new List<VinDecodeHistory>();
}
