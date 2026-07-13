using SharedServices.Models.Car;
using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

/// <summary>
/// No-op implementation used when OpenAI API key is not configured.
/// Returns empty results without making any external calls.
/// </summary>
public class DisabledVehicleSearchService : IVehicleSearchService
{
    public Task<(NhtsaWmiResponse Nhtsa, string AiResponse, WmiEntry? Saved)> SearchWmiAsync(string code, WmiCodeType codeType)
        => Task.FromResult<(NhtsaWmiResponse, string, WmiEntry?)>((new NhtsaWmiResponse(), string.Empty, null));

    public Task<(NhtsaVinResponse Nhtsa, string AiResponse, VinInfo? Saved)> SearchVinAsync(string vin)
        => Task.FromResult<(NhtsaVinResponse, string, VinInfo?)>((new NhtsaVinResponse(), string.Empty, null));

    public Task<(string AiResponse, VinInfo? Saved)> SearchCustomVinAsync(string vin, string? notes)
        => Task.FromResult<(string, VinInfo?)>((string.Empty, null));
}
