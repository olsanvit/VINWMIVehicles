using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VINWMIVehicles.Data;

public class VehicleDbContextFactory : IDesignTimeDbContextFactory<VehicleDbContext>
{
    public VehicleDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<VehicleDbContext>()
            .UseNpgsql("Host=100.99.239.94;Port=5432;Database=VINWMIVehicles;Username=roundnet;Password=kindred;Ssl Mode=Disable")
            .Options;
        return new VehicleDbContext(opts);
    }
}
