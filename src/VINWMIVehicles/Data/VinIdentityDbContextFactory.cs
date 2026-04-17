using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SharedServices;

namespace VINWMIVehicles.Data;

public class AppDbContextVinFactory : IDesignTimeDbContextFactory<AppDbContextVin>
{
    public AppDbContextVin CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<AppDbContextVin>()
            .UseNpgsql("Host=100.99.239.94;Port=5432;Database=VINWMIVehicles;Username=roundnet;Password=kindred;Ssl Mode=Disable")
            .Options;
        return new AppDbContextVin(opts);
    }
}
