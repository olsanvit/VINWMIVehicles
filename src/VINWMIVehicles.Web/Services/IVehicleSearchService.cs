using SharedServices.Models.Car;
using VINWMIVehicles.Models;

namespace VINWMIVehicles.Services;

/// <summary>
/// Orchestrates vehicle lookup operations by combining NHTSA API data with AI-generated analysis
/// and persisting results to the database.
/// Each search method fetches external data concurrently and saves a record of the result.
/// </summary>
public interface IVehicleSearchService
{
    /// <summary>
    /// Decodes a WMI or WMC code by querying both the NHTSA API and an AI model in parallel,
    /// then persists the result as a <see cref="WmiEntry"/> in the database.
    /// </summary>
    /// <param name="code">The WMI or WMC code to look up (3 or 6 characters).</param>
    /// <param name="codeType">Specifies whether <paramref name="code"/> is a WMI or WMC code.</param>
    /// <returns>
    /// A tuple containing the raw NHTSA response, a JSON-serialized AI analysis string,
    /// and the persisted <see cref="WmiEntry"/> record (or <see langword="null"/> if persistence failed).
    /// </returns>
    Task<(NhtsaWmiResponse Nhtsa, string AiResponse, WmiEntry? Saved)> SearchWmiAsync(string code, WmiCodeType codeType);

    /// <summary>
    /// Decodes a standard 17-character VIN by querying both the NHTSA API and an AI model in parallel,
    /// then persists the decoded information as a <see cref="VinInfo"/> record in the database.
    /// </summary>
    /// <param name="vin">The 17-character Vehicle Identification Number to decode.</param>
    /// <returns>
    /// A tuple containing the raw NHTSA response, the AI analysis text,
    /// and the persisted <see cref="VinInfo"/> record (or <see langword="null"/> if persistence failed).
    /// </returns>
    Task<(NhtsaVinResponse Nhtsa, string AiResponse, VinInfo? Saved)> SearchVinAsync(string vin);

    /// <summary>
    /// Analyzes a non-standard or custom VIN using only an AI model, then persists the result
    /// as a <see cref="VinInfo"/> record with any optional user-supplied notes.
    /// </summary>
    /// <param name="vin">The custom or non-standard VIN string to analyze.</param>
    /// <param name="notes">Optional user-supplied context or notes to include in the AI prompt.</param>
    /// <returns>
    /// A tuple containing the AI analysis text and the persisted <see cref="VinInfo"/> record
    /// (or <see langword="null"/> if persistence failed).
    /// </returns>
    Task<(string AiResponse, VinInfo? Saved)> SearchCustomVinAsync(string vin, string? notes);
}
