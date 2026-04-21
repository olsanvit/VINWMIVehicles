using Microsoft.EntityFrameworkCore;
using SharedServices;
using SharedServices.Models.Common;

namespace VINWMIVehicles.Services;

/// <summary>
/// Background service který denně spouští VinRandomizerService.
/// Sleduje WebsiteTask záznamy s názvem VinRandomizerDaily / WmiRandomizerDaily.
/// Pokud NextRunAtUtc je v minulosti → spustí randomizer a nastaví NextRunAtUtc na zítřek.
/// </summary>
public class VinRandomizerHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<VinRandomizerHostedService> _log;
    private static readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

    public VinRandomizerHostedService(IServiceScopeFactory scopes, ILogger<VinRandomizerHostedService> log)
    {
        _scopes = scopes;
        _log    = log;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // Počkej na startup
        await Task.Delay(TimeSpan.FromSeconds(30), ct);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await RunIfDueAsync(ct);
            }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                _log.LogError(ex, "VinRandomizerHostedService: neočekávaná chyba");
            }

            await Task.Delay(_checkInterval, ct);
        }
    }

    private async Task RunIfDueAsync(CancellationToken ct)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var db = await scope.ServiceProvider
            .GetRequiredService<IDbContextFactory<AppDbContextVin>>()
            .CreateDbContextAsync(ct);

        var taskName = WebsiteTaskName.VinRandomizerDaily.ToString();
        var task = await db.WebsiteTasks.FirstOrDefaultAsync(t => t.Name == taskName, ct);

        if (task is null)
        {
            // Prvotní vytvoření záznamu → spustit hned
            task = new WebsiteTask
            {
                Guid         = Guid.NewGuid(),
                Name         = taskName,
                Url          = "internal://vin-randomizer",
                Enabled      = true,
                FirstRun     = true,
                NextRunAtUtc = DateTimeOffset.UtcNow
            };
            db.WebsiteTasks.Add(task);
            await db.SaveChangesAsync(ct);
        }

        if (!task.Enabled || task.NextRunAtUtc > DateTimeOffset.UtcNow)
            return;

        _log.LogInformation("VinRandomizerHostedService: spouštím VinRandomizerService...");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var svc = scope.ServiceProvider.GetRequiredService<VinRandomizerService>();
        await svc.RunDailyAsync(ct);

        sw.Stop();
        task.LastRunAtUtc       = DateTimeOffset.UtcNow;
        task.LastRunDurationMs  = sw.ElapsedMilliseconds;
        task.FirstRun           = false;
        task.NextRunAtUtc       = DateTimeOffset.UtcNow.Date.AddDays(1); // zítra o půlnoci
        task.LastError          = null;

        await db.SaveChangesAsync(ct);
        _log.LogInformation("VinRandomizerHostedService: hotovo za {Ms}ms", sw.ElapsedMilliseconds);
    }
}
