using Microsoft.EntityFrameworkCore;
using SharedServices;
using SharedServices.Models.VINWMIVehicles.AI;
using SharedServices.Models.VINWMIVehicles.Manufacturers;
using SharedServices.Models.VINWMIVehicles.Vin;
using SharedServices.Services;
using System.Text;

namespace VINWMIVehicles.Services;

/// <summary>
/// Denní randomizer VIN/WMI/WMC kódů.
/// Počty vychází z čísla dne v měsíci (1–31).
/// - VIN:            dayOfMonth kusů
/// - WMI:            dayOfMonth kusů
/// - WMI Extended:   dayOfMonth * 5 kusů
/// - WMC:            dayOfMonth kusů
/// Každý generovaný kód je dotázán přes ChatGptAsker a výsledek uložen do AI result tabulek.
/// </summary>
public class VinRandomizerService
{
    private readonly IDbContextFactory<AppDbContextVin> _factory;
    private readonly ChatGptAsker _gpt;
    private readonly ILogger<VinRandomizerService> _log;

    private static readonly char[] VinChars =
        "ABCDEFGHJKLMNPRSTUVWXYZ0123456789".ToCharArray(); // bez I, O, Q

    public VinRandomizerService(
        IDbContextFactory<AppDbContextVin> factory,
        ChatGptAsker gpt,
        ILogger<VinRandomizerService> log)
    {
        _factory = factory;
        _gpt     = gpt;
        _log     = log;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Hlavní metoda — volaná z WebsiteTask / scheduleru
    // ────────────────────────────────────────────────────────────────────────

    public async Task RunDailyAsync(CancellationToken ct = default)
    {
        var day = DateTime.UtcNow.Day; // 1–31

        _log.LogInformation("VinRandomizer: day={Day} → VIN={V} WMI={W} WMIExt={WE} WMC={WMC}",
            day, day, day, day * 5, day);

        await using var db = await _factory.CreateDbContextAsync(ct);

        var wmiCodes = await db.WmiAssignments
            .Select(w => new { w.Guid, w.Wmi })
            .ToListAsync(ct);

        var wmcCodes = await db.WmcEntries
            .Select(w => new { w.Guid, w.Code })
            .ToListAsync(ct);

        if (wmiCodes.Count == 0)
        {
            _log.LogWarning("VinRandomizer: žádné WMI kódy v DB — přeskočeno.");
            return;
        }

        var rng = new Random();

        // ── VINy ──────────────────────────────────────────────────────────────
        for (int i = 0; i < day; i++)
        {
            if (ct.IsCancellationRequested) break;
            try { await GenerateAndSaveVinAsync(db, wmiCodes.Select(w => w.Wmi).ToList(), rng, ct); }
            catch (Exception ex) { _log.LogWarning(ex, "VinRandomizer: chyba při generování VIN #{I}", i); }
        }

        // ── WMI dotazy ────────────────────────────────────────────────────────
        for (int i = 0; i < day; i++)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                var pick = wmiCodes[rng.Next(wmiCodes.Count)];
                await AskWmiAsync(db, pick.Guid, pick.Wmi, extended: false, ct);
            }
            catch (Exception ex) { _log.LogWarning(ex, "VinRandomizer: chyba WMI dotaz #{I}", i); }
        }

        // ── WMI Extended dotazy (5× více) ─────────────────────────────────────
        for (int i = 0; i < day * 5; i++)
        {
            if (ct.IsCancellationRequested) break;
            try
            {
                var pick = wmiCodes[rng.Next(wmiCodes.Count)];
                await AskWmiAsync(db, pick.Guid, pick.Wmi, extended: true, ct);
            }
            catch (Exception ex) { _log.LogWarning(ex, "VinRandomizer: chyba WMI Extended dotaz #{I}", i); }
        }

        // ── WMC dotazy ────────────────────────────────────────────────────────
        if (wmcCodes.Count > 0)
        {
            for (int i = 0; i < day; i++)
            {
                if (ct.IsCancellationRequested) break;
                try
                {
                    var pick = wmcCodes[rng.Next(wmcCodes.Count)];
                    await AskWmcAsync(db, pick.Guid, pick.Code, ct);
                }
                catch (Exception ex) { _log.LogWarning(ex, "VinRandomizer: chyba WMC dotaz #{I}", i); }
            }
        }

        await db.SaveChangesAsync(ct);
        _log.LogInformation("VinRandomizer: hotovo.");
    }

    // ────────────────────────────────────────────────────────────────────────
    // VIN generování + AI dotaz
    // ────────────────────────────────────────────────────────────────────────

    private async Task GenerateAndSaveVinAsync(
        AppDbContextVin db,
        List<string> wmiPool,
        Random rng,
        CancellationToken ct)
    {
        var wmi = wmiPool[rng.Next(wmiPool.Count)];
        var vds = RandomChars(rng, 6);
        var vis = RandomChars(rng, 8);
        var vin = wmi + vds + vis;

        // Přeskoč duplicitní VIN
        if (await db.VinRecords.AnyAsync(v => v.Vin == vin, ct))
            return;

        var record = new VinRecord
        {
            Vin = vin,
            Wmi = wmi,
            Vds = vds,
            Vis = vis
        };
        db.VinRecords.Add(record);
        await db.SaveChangesAsync(ct);

        // AI dotaz
        var query = $"Analyze VIN: {vin}. Identify: manufacturer (WMI={wmi}), model year, country of manufacture, vehicle type, and any notable characteristics. Be concise and factual.";
        var response = await _gpt.AskAsync(
            "You are a VIN decoder expert. Analyze the given VIN and provide structured information about the vehicle.",
            query) ?? "";

        db.VinAiResults.Add(new VinAiResult
        {
            VinRecordId = record.Guid,
            Query       = query,
            Response    = response,
            AiModel     = "gpt-5-mini",
            CreatedAt   = DateTime.UtcNow
        });
    }

    // ────────────────────────────────────────────────────────────────────────
    // WMI AI dotaz
    // ────────────────────────────────────────────────────────────────────────

    private async Task AskWmiAsync(
        AppDbContextVin db,
        Guid wmiAssignmentId,
        string wmi,
        bool extended,
        CancellationToken ct)
    {
        var suffix = extended ? " extended (6-character prefix)" : " (3-character prefix)";
        var query = extended
            ? $"Provide detailed information about WMI extended code '{wmi}': manufacturer name, country, vehicle types produced, years active, parent company, and any notable models."
            : $"Provide information about WMI code '{wmi}': manufacturer name, country of manufacture, vehicle type, and key facts.";

        var response = await _gpt.AskAsync(
            $"You are a World Manufacturer Identifier (WMI{suffix}) expert. Answer questions about WMI codes accurately and concisely.",
            query) ?? "";

        db.WmiAiResults.Add(new WmiAiResult
        {
            WmiAssignmentId = wmiAssignmentId,
            Query           = query,
            Response        = response,
            AiModel         = "gpt-5-mini",
            CreatedAt       = DateTime.UtcNow
        });
    }

    // ────────────────────────────────────────────────────────────────────────
    // WMC AI dotaz
    // ────────────────────────────────────────────────────────────────────────

    private async Task AskWmcAsync(
        AppDbContextVin db,
        Guid wmcEntryId,
        string code,
        CancellationToken ct)
    {
        // WMC nemá vlastní AI result tabulku — používáme WmiAiResult s FK na WmiAssignment
        // Místo toho hledáme odpovídající WmiAssignment podle kódu
        var wmiPrefix = code[..Math.Min(3, code.Length)];
        var assignment = await db.WmiAssignments
            .FirstOrDefaultAsync(w => w.Wmi == wmiPrefix, ct);

        if (assignment is null)
            return;

        var query = $"Provide information about WMC (World Manufacturer Code) '{code}': which manufacturer holds this code, what vehicles are produced under it, country of manufacture, and any relevant regulatory information.";

        var response = await _gpt.AskAsync(
            "You are a World Manufacturer Code (WMC) expert. Provide accurate information about WMC codes from the AEM public record.",
            query) ?? "";

        db.WmiAiResults.Add(new WmiAiResult
        {
            WmiAssignmentId = assignment.Guid,
            Query           = query,
            Response        = response,
            AiModel         = "gpt-5-mini",
            CreatedAt       = DateTime.UtcNow
        });
    }

    // ────────────────────────────────────────────────────────────────────────
    // Helpers
    // ────────────────────────────────────────────────────────────────────────

    private static string RandomChars(Random rng, int length)
    {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
            sb.Append(VinChars[rng.Next(VinChars.Length)]);
        return sb.ToString();
    }
}
