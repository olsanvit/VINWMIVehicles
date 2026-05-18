using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedServices;

namespace VINWMIVehicles.Web;

/// <summary>
/// Design-time factory used by the EF Core tooling (e.g. <c>dotnet ef migrations add</c>)
/// when the application's DI host cannot be fully resolved.
/// It constructs an <see cref="AppDbContextVehicle"/> with a hard-coded development connection string
/// so that migrations can be created without starting the full web host.
/// </summary>
public class AppDbContextVehicleDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContextVehicle>
{
    /// <summary>
    /// Creates a configured <see cref="AppDbContextVehicle"/> instance for use during design-time tooling operations.
    /// The returned context targets the development PostgreSQL database and uses the <c>SharedServices</c> assembly for migrations.
    /// </summary>
    /// <param name="args">Command-line arguments passed by the EF Core tooling; not used by this implementation.</param>
    /// <returns>A fully configured <see cref="AppDbContextVehicle"/> ready for migration operations.</returns>
    public AppDbContextVehicle CreateDbContext(string[] args)
    {
        const string cs =
            "Host=100.99.239.94;Port=5432;Database=vinWmi;Username=roundnet;Password=kindred;" +
            "Pooling=true;Timeout=50;Command Timeout=120;Ssl Mode=Disable";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContextVehicle>();
        optionsBuilder.UseNpgsql(cs, o =>
        {
            o.MigrationsAssembly("SharedServices");
        });

        return new AppDbContextVehicle(optionsBuilder.Options);
    }
}
