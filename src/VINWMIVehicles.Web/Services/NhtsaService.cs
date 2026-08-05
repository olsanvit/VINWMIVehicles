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
    // AUDIT:FIXED|byl: bez retry; nyní 3 pokusy s exponenciálním backoffem
    public async Task<NhtsaWmiResponse> DecodeWMIAsync(string wmi)
    {
        return await RetryAsync(
            () => _http.GetFromJsonAsync<NhtsaWmiResponse>(
                $"{BaseUrl}/decodewmi/{Uri.EscapeDataString(wmi)}?format=json"),
            ex => _log.LogWarning(ex, "NHTSA chyba při DecodeWMI WMI={Wmi}", wmi),
            new NhtsaWmiResponse { Message = "NHTSA API chyba" });
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
    // AUDIT:FIXED|byl: bez retry; nyní 3 pokusy s exponenciálním backoffem
    public async Task<NhtsaVinResponse> DecodeVINAsync(string vin)
    {
        return await RetryAsync(
            () => _http.GetFromJsonAsync<NhtsaVinResponse>(
                $"{BaseUrl}/decodevin/{Uri.EscapeDataString(vin)}?format=json"),
            ex => _log.LogWarning(ex, "NHTSA chyba při DecodeVIN VIN={Vin}", vin),
            new NhtsaVinResponse { Message = "NHTSA API chyba" });
    }

    // 3 pokusy s exponenciálním backoffem: 1s, 2s, 4s
    private static async Task<T> RetryAsync<T>(
        Func<Task<T?>> call,
        Action<Exception> logWarning,
        T fallback,
        int maxAttempts = 3)
    {
        int delay = 1000;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var result = await call();
                return result ?? fallback;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logWarning(ex);
                await Task.Delay(delay);
                delay *= 2;
            }
            catch (Exception ex)
            {
                logWarning(ex);
            }
        }
        return fallback;
    }
}
