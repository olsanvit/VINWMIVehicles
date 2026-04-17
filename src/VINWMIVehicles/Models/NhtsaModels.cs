namespace VINWMIVehicles.Models;

public class NhtsaWmiResponse
{
    public List<NhtsaWmiResult> Results { get; set; } = new();
    public string? Message { get; set; }
    public string? SearchCriteria { get; set; }
}

public class NhtsaWmiResult
{
    public string? CommonName { get; set; }
    public string? Country { get; set; }
    public string? CreatedOn { get; set; }
    public string? DateAvailableToPublic { get; set; }
    public string? ManufacturerName { get; set; }
    public string? ParentCompanyName { get; set; }
    public string? URL { get; set; }
    public string? UpdatedOn { get; set; }
    public string? VehicleTypeName { get; set; }
}

public class NhtsaVinResponse
{
    public List<NhtsaVinVariable> Results { get; set; } = new();
    public string? Message { get; set; }
    public string? SearchCriteria { get; set; }
}

public class NhtsaVinVariable
{
    public string? Value { get; set; }
    public string? ValueId { get; set; }
    public string? Variable { get; set; }
    public int VariableId { get; set; }
}
