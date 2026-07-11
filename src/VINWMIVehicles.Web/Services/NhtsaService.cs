using System.Net.Http.Json;
using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

/// <summary>
/// HTTP client implementation of <see cref="INhtsaService"/> that communicates with the
/// NHTSA vPIC REST API to decode WMI codes and full VINs.
/// Network and unexpected errors are caught, logged, and returned as error-message responses rather than thrown.
/// </summary>
public class NhtsaService : INhtsaService
{
    private readonly HttpClient _http;
    private readonly ILogger<NhtsaService> _log;
    private const string BaseUrl = "https://vpic.nhtsa.dot.gov/api/vehicles";

    /// <summary>
    /// Initializes a new instance of <see cref="NhtsaService"/> with the provided HTTP client and logger.
    /// </summary>
    /// <param name="http">The typed <see cref="HttpClient"/> pre-configured for NHTSA API requests.</param>
    /// <param name="log">The logger used to record warnings and errors during API calls.</param>
    public NhtsaService(HttpClient http, ILogger<NhtsaService> log)
    {
        _http = http;
        _log  = log;
    }

    /// <summary>
    /// Sends a GET request to the NHTSA vPIC WMI decode endpoint and deserializes the JSON response.
    /// Returns an empty response with an error message instead of throwing if the HTTP call fails.
    /// </summary>
    /// <param name="wmi">The 3- or 6-character WMI code to decode.</param>
    /// <returns>
    /// A <see cref="NhtsaWmiResponse"/> with manufacturer results on success,
    /// or a response whose <c>Message</c> property describes the failure.
    /// </returns>
    // AUDIT:PENDING|Střední|Bez retry logiky a rate limiting pro NHTSA API
    public async Task<NhtsaWmiResponse> DecodeWMIAsync(string wmi)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<NhtsaWmiResponse>(
                $"{BaseUrl}/decodewmi/{Uri.EscapeDataString(wmi)}?format=json");
            return result ?? new NhtsaWmiResponse();
        }
        catch (HttpRequestException ex)
        {
            _log.LogWarning(ex, "NHTSA HTTP chyba při DecodeWMI pro WMI={Wmi} (status={Status})", wmi, ex.StatusCode);
            return new NhtsaWmiResponse { Message = $"NHTSA API nedostupné: {ex.Message}" };
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Neočekávaná chyba při DecodeWMI pro WMI={Wmi}", wmi);
            return new NhtsaWmiResponse { Message = "NHTSA API chyba" };
        }
    }

    /// <summary>
    /// Sends a GET request to the NHTSA vPIC VIN decode endpoint and deserializes the JSON response.
    /// Returns an empty response with an error message instead of throwing if the HTTP call fails.
    /// </summary>
    /// <param name="vin">The 17-character VIN to decode.</param>
    /// <returns>
    /// A <see cref="NhtsaVinResponse"/> with decoded variable entries on success,
    /// or a response whose <c>Message</c> property describes the failure.
    /// </returns>
    // AUDIT:PENDING|Střední|Bez retry logiky a rate limiting
    public async Task<NhtsaVinResponse> DecodeVINAsync(string vin)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<NhtsaVinResponse>(
                $"{BaseUrl}/decodevin/{Uri.EscapeDataString(vin)}?format=json");
            return result ?? new NhtsaVinResponse();
        }
        catch (HttpRequestException ex)
        {
            _log.LogWarning(ex, "NHTSA HTTP chyba při DecodeVIN pro VIN={Vin} (status={Status})", vin, ex.StatusCode);
            return new NhtsaVinResponse { Message = $"NHTSA API nedostupné: {ex.Message}" };
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Neočekávaná chyba při DecodeVIN pro VIN={Vin}", vin);
            return new NhtsaVinResponse { Message = "NHTSA API chyba" };
        }
    }
}
