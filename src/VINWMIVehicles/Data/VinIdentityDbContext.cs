using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MercenariesAndBeasts.Infrastructure;

namespace VINWMIVehicles.Data;

// Slim Identity context — only stores ASP.NET Identity tables (AspNetUsers etc.)
// AppDbContextCar handles vehicle data separately; both share the same DB.
public class VinIdentityDbContext : IdentityDbContext<AppUser>
{
    public VinIdentityDbContext(DbContextOptions<VinIdentityDbContext> options) : base(options) { }
}
