namespace VINWMIVehicles.Models;

/// <summary>
/// Represents the top-level response returned by the NHTSA vPIC API when decoding a WMI code.
/// Contains a collection of manufacturer results along with optional status information.
/// </summary>
public class NhtsaWmiResponse
{
    /// <summary>Gets or sets the list of manufacturer entries matching the WMI query.</summary>
    public List<NhtsaWmiResult> Results { get; set; } = new();

    /// <summary>Gets or sets the status or error message returned by the NHTSA API.</summary>
    public string? Message { get; set; }

    /// <summary>Gets or sets the search criteria echo returned by the NHTSA API.</summary>
    public string? SearchCriteria { get; set; }
}

/// <summary>
/// Represents a single manufacturer record returned by the NHTSA WMI decode endpoint.
/// Each result corresponds to one manufacturer assignment for the queried WMI prefix.
/// </summary>
public class NhtsaWmiResult
{
    /// <summary>Gets or sets the common or trade name of the manufacturer.</summary>
    public string? CommonName { get; set; }

    /// <summary>Gets or sets the country in which the manufacturer is registered.</summary>
    public string? Country { get; set; }

    /// <summary>Gets or sets the date when this WMI assignment was created in the NHTSA database.</summary>
    public string? CreatedOn { get; set; }

    /// <summary>Gets or sets the date from which this WMI data was made available to the public.</summary>
    public string? DateAvailableToPublic { get; set; }

    /// <summary>Gets or sets the official registered name of the manufacturer.</summary>
    public string? ManufacturerName { get; set; }

    /// <summary>Gets or sets the name of the parent company that owns this manufacturer.</summary>
    public string? ParentCompanyName { get; set; }

    /// <summary>Gets or sets the URL for the manufacturer's public information page.</summary>
    public string? URL { get; set; }

    /// <summary>Gets or sets the date when this WMI record was last updated in the NHTSA database.</summary>
    public string? UpdatedOn { get; set; }

    /// <summary>Gets or sets the category of vehicles produced under this WMI (e.g. Passenger Car, Truck).</summary>
    public string? VehicleTypeName { get; set; }
}

/// <summary>
/// Represents the top-level response returned by the NHTSA vPIC API when decoding a full VIN.
/// Contains a flat list of key/value variable pairs describing all decoded vehicle attributes.
/// </summary>
public class NhtsaVinResponse
{
    /// <summary>Gets or sets the decoded variable entries for the queried VIN.</summary>
    public List<NhtsaVinVariable> Results { get; set; } = new();

    /// <summary>Gets or sets the status or error message returned by the NHTSA API.</summary>
    public string? Message { get; set; }

    /// <summary>Gets or sets the search criteria echo returned by the NHTSA API.</summary>
    public string? SearchCriteria { get; set; }
}

/// <summary>
/// Represents a single decoded attribute variable from the NHTSA VIN decode response.
/// Each instance maps one vehicle characteristic (e.g. Make, Model Year) to its decoded value.
/// </summary>
public class NhtsaVinVariable
{
    /// <summary>Gets or sets the human-readable decoded value for this variable, or <see langword="null"/> if not available.</summary>
    public string? Value { get; set; }

    /// <summary>Gets or sets the numeric identifier of the decoded value within the NHTSA lookup table.</summary>
    public string? ValueId { get; set; }

    /// <summary>Gets or sets the name of the vehicle attribute described by this variable (e.g. "Make", "Model Year").</summary>
    public string? Variable { get; set; }

    /// <summary>Gets or sets the unique NHTSA identifier for the variable type.</summary>
    public int VariableId { get; set; }
}
