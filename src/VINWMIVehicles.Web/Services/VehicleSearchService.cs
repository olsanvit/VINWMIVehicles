using SharedServices;
using SharedServices.Models.Car;
using SharedServices.Services;
using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

/// <summary>
/// Implements <see cref="IVehicleSearchService"/> by orchestrating concurrent NHTSA API and AI model calls,
/// then persisting the results through <see cref="EfCoreService{TContext}"/>.
/// Database save failures are caught and logged without propagating exceptions to callers.
/// </summary>
public class VehicleSearchService : IVehicleSearchService
{
    private readonly INhtsaService _nhtsa;
    private readonly ChatGPTWMI _wmiGpt;
    private readonly ChatGptAsker _gpt;
    private readonly EfCoreService<AppDbContextVehicle> _db;
    private readonly ILogger<VehicleSearchService> _log;

    /// <summary>
    /// Initializes a new instance of <see cref="VehicleSearchService"/> with its required dependencies.
    /// </summary>
    /// <param name="nhtsa">The NHTSA API client used for official VIN and WMI decoding.</param>
    /// <param name="wmiGpt">The specialized GPT service for detailed WMI/WMC manufacturer analysis.</param>
    /// <param name="gpt">The general-purpose GPT service used for VIN narrative analysis.</param>
    /// <param name="db">The EF Core service used to persist and retrieve vehicle records.</param>
    /// <param name="log">The logger used to record persistence errors.</param>
    public VehicleSearchService(
        INhtsaService nhtsa,
        ChatGPTWMI wmiGpt,
        ChatGptAsker gpt,
        EfCoreService<AppDbContextVehicle> db,
        ILogger<VehicleSearchService> log)
    {
        _nhtsa = nhtsa;
        _wmiGpt = wmiGpt;
        _gpt = gpt;
        _db = db;
        _log = log;
    }

    /// <summary>
    /// Fetches NHTSA WMI data and AI manufacturer analysis concurrently, then upserts the result
    /// as a <see cref="WmiEntry"/> with its associated manufacturers in the database.
    /// If the database save fails, the error is logged and the returned <c>Saved</c> value is <see langword="null"/>.
    /// </summary>
    /// <param name="code">The WMI or WMC code to search (3 or 6 characters, case-insensitive).</param>
    /// <param name="codeType">Indicates whether <paramref name="code"/> represents a WMI or WMC assignment.</param>
    /// <returns>
    /// A tuple of the raw NHTSA response, a pretty-printed JSON string of the AI result,
    /// and the persisted <see cref="WmiEntry"/> (or <see langword="null"/> on persistence failure).
    /// </returns>
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
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save WMI entry for code {Code}.", code);
        }

        return (nhtsa, aiText, saved);
    }

    /// <summary>
    /// Fetches NHTSA VIN decode data and an AI narrative analysis concurrently, then upserts the result
    /// as a <see cref="VinInfo"/> record in the database.
    /// If the database save fails, the error is logged and the returned <c>Saved</c> value is <see langword="null"/>.
    /// </summary>
    /// <param name="vin">The 17-character VIN to decode; leading/trailing whitespace is trimmed and the value is uppercased.</param>
    /// <returns>
    /// A tuple of the raw NHTSA response, the AI narrative text,
    /// and the persisted <see cref="VinInfo"/> record (or <see langword="null"/> on persistence failure).
    /// </returns>
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
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save VIN info for VIN {Vin}.", vin);
        }

        return (nhtsa, aiText, saved);
    }

    /// <summary>
    /// Analyzes a non-standard or custom VIN using only the AI model (no NHTSA lookup),
    /// then persists the result as a <see cref="VinInfo"/> record including any user-supplied notes.
    /// If the database save fails, the error is logged and the returned <c>Saved</c> value is <see langword="null"/>.
    /// </summary>
    /// <param name="vin">The custom or non-standard VIN string to analyze; whitespace is trimmed.</param>
    /// <param name="notes">Optional additional context or notes that are appended to the AI prompt.</param>
    /// <returns>
    /// A tuple of the AI analysis text and the persisted <see cref="VinInfo"/> record
    /// (or <see langword="null"/> on persistence failure).
    /// </returns>
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
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to save custom VIN info for VIN {Vin}.", vin);
        }

        return (aiText, saved);
    }
}
