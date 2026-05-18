using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

/// <summary>
/// Defines a client for the NHTSA vPIC (Vehicle Product Information Catalog) API.
/// Provides methods for decoding World Manufacturer Identifiers (WMI) and full Vehicle Identification Numbers (VIN).
/// </summary>
public interface INhtsaService
{
    /// <summary>
    /// Queries the NHTSA API to decode a World Manufacturer Identifier code.
    /// Returns manufacturer details such as name, country, and vehicle type for the given WMI.
    /// </summary>
    /// <param name="wmi">The 3- or 6-character WMI code to decode.</param>
    /// <returns>
    /// A <see cref="NhtsaWmiResponse"/> containing matching manufacturer records,
    /// or a response with an error message if the API call fails.
    /// </returns>
    Task<NhtsaWmiResponse> DecodeWMIAsync(string wmi);

    /// <summary>
    /// Queries the NHTSA API to decode a full 17-character Vehicle Identification Number.
    /// Returns a flat list of decoded vehicle attribute variables such as Make, Model, and Model Year.
    /// </summary>
    /// <param name="vin">The 17-character VIN to decode.</param>
    /// <returns>
    /// A <see cref="NhtsaVinResponse"/> containing decoded attribute variables,
    /// or a response with an error message if the API call fails.
    /// </returns>
    Task<NhtsaVinResponse> DecodeVINAsync(string vin);
}
