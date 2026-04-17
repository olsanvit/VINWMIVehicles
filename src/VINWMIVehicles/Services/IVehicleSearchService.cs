using SharedServices.Models.Car;
using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

public interface IVehicleSearchService
{
    Task<(NhtsaWmiResponse Nhtsa, string AiResponse, WmiEntry? Saved)> SearchWmiAsync(string code, WmiCodeType codeType);
    Task<(NhtsaVinResponse Nhtsa, string AiResponse, VinInfo? Saved)> SearchVinAsync(string vin);
    Task<(string AiResponse, VinInfo? Saved)> SearchCustomVinAsync(string vin, string? notes);
}
