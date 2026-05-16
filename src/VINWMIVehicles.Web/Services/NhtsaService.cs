using System.Net.Http.Json;
using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

public class NhtsaService : INhtsaService
{
    private readonly HttpClient _http;
    private readonly ILogger<NhtsaService> _log;
    private const string BaseUrl = "https://vpic.nhtsa.dot.gov/api/vehicles";

    public NhtsaService(HttpClient http, ILogger<NhtsaService> log)
    {
        _http = http;
        _log  = log;
    }

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
