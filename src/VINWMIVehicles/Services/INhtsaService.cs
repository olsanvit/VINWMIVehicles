using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

public interface INhtsaService
{
    Task<NhtsaWmiResponse> DecodeWMIAsync(string wmi);
    Task<NhtsaVinResponse> DecodeVINAsync(string vin);
}
