using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SharedServices.Models.VINWMIVehicles.Geography;
using SharedServices.Models.VINWMIVehicles.Manufacturers;

namespace VINWMIVehicles.Tests;

/// <summary>
/// Minimal InMemory DbContext for testing manufacturer persistence without
/// pulling in the full AppDbContextVehicle (which requires ASP.NET Identity).
/// </summary>
public class VehicleTestDbContext : DbContext
{
    public VehicleTestDbContext(DbContextOptions<VehicleTestDbContext> options) : base(options) { }

    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<WmiAssignment> WmiAssignments => Set<WmiAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Manufacturer>(e =>
        {
            e.HasKey(x => x.Guid);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasOne(x => x.Country).WithMany(c => c.Manufacturers)
                .HasForeignKey(x => x.CountryId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.ParentManufacturer).WithMany(m => m.Subsidiaries)
                .HasForeignKey(x => x.ParentManufacturerId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Region>(e =>
        {
            e.HasKey(x => x.Guid);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.VinChars).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<Country>(e =>
        {
            e.HasKey(x => x.Guid);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.IsoCode2).HasMaxLength(2).IsRequired();
            e.HasIndex(x => x.IsoCode2).IsUnique();
            e.HasOne(x => x.Region).WithMany(r => r.Countries)
                .HasForeignKey(x => x.RegionId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WmiAssignment>(e =>
        {
            e.HasKey(x => x.Guid);
            e.Property(x => x.Wmi).HasMaxLength(3).IsRequired();
            e.Property(x => x.Source).HasConversion<string>().HasMaxLength(30);
            e.HasOne(x => x.Manufacturer).WithMany(m => m.WmiAssignments)
                .HasForeignKey(x => x.ManufacturerId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}

/// <summary>
/// InMemory EF Core tests: add Manufacturer, save, query back.
/// </summary>
public class DbContextTests
{
    private static VehicleTestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<VehicleTestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new VehicleTestDbContext(options);
    }

    [Fact]
    public async Task AddManufacturer_SaveAndQueryBack_ReturnsCorrectEntity()
    {
        await using var ctx = CreateContext();
        var manufacturer = new Manufacturer
        {
            Name = "Toyota",
            IsActive = true
        };

        ctx.Manufacturers.Add(manufacturer);
        await ctx.SaveChangesAsync();

        var saved = await ctx.Manufacturers.FirstOrDefaultAsync(m => m.Name == "Toyota");
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Toyota");
        saved.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task AddManufacturer_WithCountry_RelationshipSaved()
    {
        await using var ctx = CreateContext();

        var region = new Region { Name = "Europe", VinChars = "SAZ" };
        var country = new Country
        {
            Name = "Germany",
            IsoCode2 = "DE",
            RegionId = region.Guid
        };

        ctx.Regions.Add(region);
        ctx.Countries.Add(country);

        var manufacturer = new Manufacturer
        {
            Name = "Volkswagen",
            CountryId = country.Guid
        };
        ctx.Manufacturers.Add(manufacturer);
        await ctx.SaveChangesAsync();

        var saved = await ctx.Manufacturers
            .Include(m => m.Country)
            .FirstOrDefaultAsync(m => m.Name == "Volkswagen");

        saved.Should().NotBeNull();
        saved!.CountryId.Should().Be(country.Guid);
        saved.Country!.Name.Should().Be("Germany");
    }

    [Fact]
    public async Task AddMultipleManufacturers_QueryAll_ReturnsAll()
    {
        await using var ctx = CreateContext();

        ctx.Manufacturers.AddRange(
            new Manufacturer { Name = "BMW" },
            new Manufacturer { Name = "Mercedes-Benz" },
            new Manufacturer { Name = "Audi" }
        );
        await ctx.SaveChangesAsync();

        var count = await ctx.Manufacturers.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task ManufacturerGuid_IsAutomaticallyGenerated()
    {
        await using var ctx = CreateContext();
        var manufacturer = new Manufacturer { Name = "Honda" };

        ctx.Manufacturers.Add(manufacturer);
        await ctx.SaveChangesAsync();

        manufacturer.Guid.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task AddWmiAssignment_LinkedToManufacturer_SavedCorrectly()
    {
        await using var ctx = CreateContext();

        var manufacturer = new Manufacturer { Name = "Honda" };
        ctx.Manufacturers.Add(manufacturer);
        await ctx.SaveChangesAsync();

        var assignment = new WmiAssignment
        {
            Wmi = "1HG",
            ManufacturerId = manufacturer.Guid,
            IsActive = true
        };
        ctx.WmiAssignments.Add(assignment);
        await ctx.SaveChangesAsync();

        var savedAssignment = await ctx.WmiAssignments
            .FirstOrDefaultAsync(a => a.Wmi == "1HG");

        savedAssignment.Should().NotBeNull();
        savedAssignment!.ManufacturerId.Should().Be(manufacturer.Guid);
    }
}
