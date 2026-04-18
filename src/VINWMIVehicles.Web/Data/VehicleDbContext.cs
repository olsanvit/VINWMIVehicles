using Microsoft.EntityFrameworkCore;

// Explicit aliases to avoid conflict with global-namespace types in SharedServices
using GeoRegion    = VINWMIVehicles.Domain.Geography.Region;
using GeoCountry   = VINWMIVehicles.Domain.Geography.Country;
using VehManufacturer = VINWMIVehicles.Domain.Manufacturers.Manufacturer;
using VehWmiAssignment = VINWMIVehicles.Domain.Manufacturers.WmiAssignment;
using VehWmcEntry  = VINWMIVehicles.Domain.Manufacturers.WmcEntry;
using VehClass     = VINWMIVehicles.Domain.Vehicles.VehicleClass;
using VehType      = VINWMIVehicles.Domain.Vehicles.VehicleType;
using VehBodyStyle = VINWMIVehicles.Domain.Vehicles.VehicleBodyStyle;
using VehBrand     = VINWMIVehicles.Domain.Vehicles.Brand;
using VehSeries    = VINWMIVehicles.Domain.Vehicles.Series;
using VehModel     = VINWMIVehicles.Domain.Vehicles.VehicleModel;
using VINWMIVehicles.Domain.Vin;

namespace VINWMIVehicles.Data;

public class VehicleDbContext : DbContext
{
    public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options) { }

    // Geography
    public DbSet<GeoRegion> Regions => Set<GeoRegion>();
    public DbSet<GeoCountry> Countries => Set<GeoCountry>();

    // Manufacturers
    public DbSet<VehManufacturer> Manufacturers => Set<VehManufacturer>();
    public DbSet<VehWmiAssignment> WmiAssignments => Set<VehWmiAssignment>();
    public DbSet<VehWmcEntry> WmcEntries => Set<VehWmcEntry>();

    // Vehicle catalog
    public DbSet<VehClass> VehicleClasses => Set<VehClass>();
    public DbSet<VehType> VehicleTypes => Set<VehType>();
    public DbSet<VehBodyStyle> VehicleBodyStyles => Set<VehBodyStyle>();
    public DbSet<VehBrand> Brands => Set<VehBrand>();
    public DbSet<VehSeries> Series => Set<VehSeries>();
    public DbSet<VehModel> VehicleModels => Set<VehModel>();

    // VIN
    public DbSet<VinRecord> VinRecords => Set<VinRecord>();
    public DbSet<VinDecodeHistory> VinDecodeHistories => Set<VinDecodeHistory>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // ── Geography ────────────────────────────────────────────
        mb.Entity<GeoRegion>(e =>
        {
            e.ToTable("Regions");
            e.HasKey(x => x.Id);
            e.Property(x => x.VinChars).HasMaxLength(30).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
        });

        mb.Entity<GeoCountry>(e =>
        {
            e.ToTable("Countries");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.NameLocal).HasMaxLength(100);
            e.Property(x => x.IsoCode2).HasMaxLength(2).IsRequired();
            e.Property(x => x.IsoCode3).HasMaxLength(3);
            e.HasIndex(x => x.IsoCode2).IsUnique();
            e.HasOne(x => x.Region).WithMany(r => r.Countries)
                .HasForeignKey(x => x.RegionId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── Manufacturers ────────────────────────────────────────
        mb.Entity<VehManufacturer>(e =>
        {
            e.ToTable("Manufacturers");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.NameOfficial).HasMaxLength(300);
            e.Property(x => x.NameLocal).HasMaxLength(300);
            e.Property(x => x.Website).HasMaxLength(500);
            e.Property(x => x.LogoUrl).HasMaxLength(500);
            e.Property(x => x.HeadquartersCity).HasMaxLength(100);
            e.Property(x => x.StockSymbol).HasMaxLength(20);
            e.Property(x => x.Description).HasMaxLength(2000);
            e.HasOne(x => x.Country).WithMany(c => c.Manufacturers)
                .HasForeignKey(x => x.CountryId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.ParentManufacturer).WithMany(m => m.Subsidiaries)
                .HasForeignKey(x => x.ParentManufacturerId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<VehWmiAssignment>(e =>
        {
            e.ToTable("WmiAssignments");
            e.HasKey(x => x.Id);
            e.Property(x => x.Wmi).HasMaxLength(3).IsRequired();
            e.Property(x => x.VehicleTypeScope).HasMaxLength(100);
            e.Property(x => x.Notes).HasMaxLength(500);
            e.Property(x => x.Source).HasConversion<string>().HasMaxLength(30);
            // Composite index: lookup by WMI + year
            e.HasIndex(x => new { x.Wmi, x.YearFrom, x.YearTo });
            e.HasOne(x => x.Manufacturer).WithMany(m => m.WmiAssignments)
                .HasForeignKey(x => x.ManufacturerId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<VehWmcEntry>(e =>
        {
            e.ToTable("WmcEntries");
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.CodeType).HasConversion<string>().HasMaxLength(30);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Source).HasConversion<string>().HasMaxLength(30);
            e.HasIndex(x => new { x.CodeType, x.Code });
            e.HasOne(x => x.Manufacturer).WithMany(m => m.WmcEntries)
                .HasForeignKey(x => x.ManufacturerId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── Vehicle catalog ──────────────────────────────────────
        mb.Entity<VehClass>(e =>
        {
            e.ToTable("VehicleClasses");
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(10).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Category).HasConversion<string>().HasMaxLength(30);
            e.HasIndex(x => x.Code).IsUnique();
        });

        mb.Entity<VehType>(e =>
        {
            e.ToTable("VehicleTypes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Code).HasMaxLength(20);
            e.HasOne(x => x.VehicleClass).WithMany(c => c.VehicleTypes)
                .HasForeignKey(x => x.VehicleClassId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<VehBodyStyle>(e =>
        {
            e.ToTable("VehicleBodyStyles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.TypicalDoorOptions).HasMaxLength(20);
            e.HasOne(x => x.VehicleType).WithMany(t => t.BodyStyles)
                .HasForeignKey(x => x.VehicleTypeId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<VehBrand>(e =>
        {
            e.ToTable("Brands");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.NameOfficial).HasMaxLength(200);
            e.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            e.Property(x => x.LogoUrl).HasMaxLength(500);
            e.Property(x => x.Website).HasMaxLength(500);
            e.Property(x => x.Description).HasMaxLength(2000);
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasOne(x => x.Manufacturer).WithMany(m => m.Brands)
                .HasForeignKey(x => x.ManufacturerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.OriginCountry).WithMany(c => c.Brands)
                .HasForeignKey(x => x.OriginCountryId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<VehSeries>(e =>
        {
            e.ToTable("Series");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(200);
            e.Property(x => x.GenerationName).HasMaxLength(50);
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.ImageUrl).HasMaxLength(500);
            e.HasOne(x => x.Brand).WithMany(b => b.Series)
                .HasForeignKey(x => x.BrandId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.VehicleBodyStyle).WithMany(bs => bs.Series)
                .HasForeignKey(x => x.VehicleBodyStyleId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<VehModel>(e =>
        {
            e.ToTable("VehicleModels");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(300).IsRequired();
            e.Property(x => x.Trim).HasMaxLength(100);
            e.Property(x => x.EngineCode).HasMaxLength(50);
            e.Property(x => x.FuelType).HasConversion<string>().HasMaxLength(30);
            e.Property(x => x.TransmissionType).HasConversion<string>().HasMaxLength(30);
            e.Property(x => x.DriveType).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.VdsPattern).HasMaxLength(6);
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.Acceleration0100s).HasPrecision(4, 1);
            e.Property(x => x.FuelConsumptionL100km).HasPrecision(4, 1);
            e.HasIndex(x => x.VdsPattern).HasFilter("\"VdsPattern\" IS NOT NULL");
            e.HasOne(x => x.Series).WithMany(s => s.Models)
                .HasForeignKey(x => x.SeriesId).OnDelete(DeleteBehavior.Restrict);
        });

        // ── VIN ──────────────────────────────────────────────────
        mb.Entity<VinRecord>(e =>
        {
            e.ToTable("VinRecords");
            e.HasKey(x => x.Id);
            e.Property(x => x.Vin).HasMaxLength(17).IsRequired();
            e.Property(x => x.Wmi).HasMaxLength(3).IsRequired();
            e.Property(x => x.Vds).HasMaxLength(6).IsRequired();
            e.Property(x => x.Vis).HasMaxLength(8).IsRequired();
            e.Property(x => x.SequentialNumber).HasMaxLength(6);
            e.Property(x => x.ValidationErrors).HasMaxLength(500);
            e.Property(x => x.DecodedSource).HasConversion<string>().HasMaxLength(30);
            e.Property(x => x.Notes).HasMaxLength(1000);
            e.HasIndex(x => x.Vin).IsUnique();
            e.HasIndex(x => x.Wmi);
            e.HasOne(x => x.Manufacturer).WithMany()
                .HasForeignKey(x => x.ManufacturerId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.VehicleModel).WithMany(m => m.VinRecords)
                .HasForeignKey(x => x.VehicleModelId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.CountryOfManufacture).WithMany()
                .HasForeignKey(x => x.CountryOfManufactureId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<VinDecodeHistory>(e =>
        {
            e.ToTable("VinDecodeHistories");
            e.HasKey(x => x.Id);
            e.Property(x => x.Source).HasConversion<string>().HasMaxLength(30);
            e.Property(x => x.ChangedFields).HasMaxLength(4000);
            e.HasOne(x => x.VinRecord).WithMany(v => v.DecodeHistory)
                .HasForeignKey(x => x.VinRecordId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
