using SharedServices;
using SharedServices.Models.Car;
using SharedServices.Services;
using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

public class VehicleSearchService : IVehicleSearchService
{
    private readonly INhtsaService _nhtsa;
    private readonly ChatGPTWMI _wmiGpt;
    private readonly ChatGptAsker _gpt;
    private readonly EfCoreService<AppDbContextVehicle> _db;

    public VehicleSearchService(
        INhtsaService nhtsa,
        ChatGPTWMI wmiGpt,
        ChatGptAsker gpt,
        EfCoreService<AppDbContextVehicle> db)
    {
        _nhtsa = nhtsa;
        _wmiGpt = wmiGpt;
        _gpt = gpt;
        _db = db;
    }

    public async Task<(NhtsaWmiResponse Nhtsa, string AiResponse, WmiEntry? Saved)> SearchWmiAsync(string code, WmiCodeType codeType)
    {
        var nhtsaTask = _nhtsa.DecodeWMIAsync(code);
        var aiTask = _wmiGpt.GetManufacturersDetailAsync(code, codeType);

        await Task.WhenAll(nhtsaTask, aiTask);

        var nhtsa = await nhtsaTask;
        var aiResult = await aiTask;

        string aiText = System.Text.Json.JsonSerializer.Serialize(aiResult,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        WmiEntry? saved = null;
        try
        {
            var entry = new WmiEntry
            {
                Code = code.Trim().ToUpperInvariant(),
                CodeType = codeType,
                CountryISO = aiResult.CountryISO,
                Region = aiResult.Region,
                AmbiguityNote = aiResult.AmbiguityNote
            };

            foreach (var m in aiResult.Manufacturers)
            {
                entry.Manufacturers.Add(new WmiManufacturer
                {
                    Manufacturer = new Manufacturer { OfficialName = m.ManufacturerOfficial },
                    ActiveYears = m.ActiveYears is null ? null : new Years
                    {
                        From = m.ActiveYears.From,
                        To = m.ActiveYears.To
                    }
                });
            }

            saved = await _db.AddUpdateWmiAndManufacturersAsync(entry);
        }
        catch { }

        return (nhtsa, aiText, saved);
    }

    public async Task<(NhtsaVinResponse Nhtsa, string AiResponse, VinInfo? Saved)> SearchVinAsync(string vin)
    {
        var nhtsaTask = _nhtsa.DecodeVINAsync(vin);
        var aiTask = _gpt.AskAsync(
            "You are a VIN decoder expert. Provide a concise analysis of the vehicle based on the VIN. Include make, model, year, country of manufacture, and notable features. Be factual and brief.",
            $"Analyze VIN: {vin.Trim().ToUpperInvariant()}");

        await Task.WhenAll(nhtsaTask, aiTask);

        var nhtsa = await nhtsaTask;
        var aiText = await aiTask ?? "";

        VinInfo? saved = null;
        try
        {
            var makeVar = nhtsa.Results.FirstOrDefault(r => r.Variable == "Make")?.Value ?? "";
            var modelVar = nhtsa.Results.FirstOrDefault(r => r.Variable == "Model")?.Value ?? "";
            var yearVar = nhtsa.Results.FirstOrDefault(r => r.Variable == "Model Year")?.Value ?? "";

            var info = new VinInfo
            {
                Vin = vin.Trim().ToUpperInvariant(),
                Make = makeVar,
                Model = modelVar,
                ModelYear = yearVar,
                Note = aiText
            };
            saved = await _db.AddUpdateVinInfo(info);
        }
        catch { }

        return (nhtsa, aiText, saved);
    }

    public async Task<(string AiResponse, VinInfo? Saved)> SearchCustomVinAsync(string vin, string? notes)
    {
        var userMsg = $"Custom VIN: {vin.Trim()}";
        if (!string.IsNullOrWhiteSpace(notes))
            userMsg += $"\nNotes: {notes}";

        var aiText = await _gpt.AskAsync(
            "You are a VIN decoder expert. Analyze the given custom or non-standard VIN. Extract as much vehicle information as possible: manufacturer, country, year, model line, body type, engine, etc. Be factual.",
            userMsg) ?? "";

        VinInfo? saved = null;
        try
        {
            var info = new VinInfo
            {
                Vin = vin.Trim(),
                Make = "",
                Model = "",
                ModelYear = "",
                Note = aiText,
                Description = notes
            };
            saved = await _db.AddUpdateVinInfo(info);
        }
        catch { }

        return (aiText, saved);
    }
}
