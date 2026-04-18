using System.Net.Http.Json;
using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

public class NhtsaService : INhtsaService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://vpic.nhtsa.dot.gov/api/vehicles";

    public NhtsaService(HttpClient http)
    {
        _http = http;
    }

    public async Task<NhtsaWmiResponse> DecodeWMIAsync(string wmi)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<NhtsaWmiResponse>(
                $"{BaseUrl}/decodewmi/{Uri.EscapeDataString(wmi)}?format=json");
            return result ?? new NhtsaWmiResponse();
        }
        catch
        {
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
        catch
        {
            return new NhtsaVinResponse { Message = "NHTSA API chyba" };
        }
    }
}
