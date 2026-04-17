using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VINWMIVehicles.Data;

public class VinIdentityDbContextFactory : IDesignTimeDbContextFactory<VinIdentityDbContext>
{
    public VinIdentityDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<VinIdentityDbContext>()
            .UseNpgsql("Host=100.99.239.94;Port=5432;Database=VINWMIVehicles;Username=roundnet;Password=kindred;Ssl Mode=Disable")
            .Options;
        return new VinIdentityDbContext(opts);
    }
}
