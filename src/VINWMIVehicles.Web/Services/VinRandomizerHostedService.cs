using Microsoft.EntityFrameworkCore;
using SharedServices;
using SharedServices.Models.Common;

namespace VINWMIVehicles.Services;

/// <summary>
/// A long-running <see cref="BackgroundService"/> that triggers <see cref="VinRandomizerService.RunDailyAsync"/> once per day.
/// It polls every 15 minutes and runs the randomizer when the <c>VinRandomizerDaily</c> <see cref="WebsiteTask"/> record
/// indicates that the next scheduled run time has passed.
/// On first startup the task record is created and the randomizer is executed immediately.
/// </summary>
public class VinRandomizerHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<VinRandomizerHostedService> _log;
    private static readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Initializes a new instance of <see cref="VinRandomizerHostedService"/> with its required dependencies.
    /// </summary>
    /// <param name="scopes">The factory used to create DI scopes for each polling iteration.</param>
    /// <param name="log">The logger used to record execution start, completion, and error events.</param>
    public VinRandomizerHostedService(IServiceScopeFactory scopes, ILogger<VinRandomizerHostedService> log)
    {
        _scopes = scopes;
        _log    = log;
    }

    /// <summary>
    /// Starts the background polling loop after a 30-second startup delay.
    /// The loop checks whether the daily run is due every 15 minutes and stops when the cancellation token is signalled.
    /// </summary>
    /// <param name="ct">The cancellation token provided by the host to signal graceful shutdown.</param>
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
            .GetRequiredService<IDbContextFactory<AppDbContextVehicle>>()
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
