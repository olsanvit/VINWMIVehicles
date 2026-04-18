using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VINWMIVehicles.Migrations
{
    /// <inheritdoc />
    public partial class InitVehicleDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VinChars = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameLocal = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsoCode2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    IsoCode3 = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    RegionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Countries_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VehicleClassId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTypes_VehicleClasses_VehicleClassId",
                        column: x => x.VehicleClassId,
                        principalTable: "VehicleClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NameOfficial = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NameLocal = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Founded = table.Column<int>(type: "integer", nullable: true),
                    Dissolved = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    HeadquartersCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StockSymbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ParentManufacturerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manufacturers_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Manufacturers_Manufacturers_ParentManufacturerId",
                        column: x => x.ParentManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleBodyStyles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VehicleTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypicalDoorOptions = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleBodyStyles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleBodyStyles_VehicleTypes_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "VehicleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NameOfficial = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginCountryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Founded = table.Column<int>(type: "integer", nullable: true),
                    Discontinued = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brands_Countries_OriginCountryId",
                        column: x => x.OriginCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Brands_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WmcEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CodeType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValidFrom = table.Column<int>(type: "integer", nullable: true),
                    ValidTo = table.Column<int>(type: "integer", nullable: true),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmcEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WmcEntries_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WmiAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Wmi = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uuid", nullable: false),
                    YearFrom = table.Column<int>(type: "integer", nullable: true),
                    YearTo = table.Column<int>(type: "integer", nullable: true),
                    VehicleTypeScope = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmiAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WmiAssignments_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleBodyStyleId = table.Column<Guid>(type: "uuid", nullable: true),
                    GenerationName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GenerationFrom = table.Column<int>(type: "integer", nullable: true),
                    GenerationTo = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Series_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Series_VehicleBodyStyles_VehicleBodyStyleId",
                        column: x => x.VehicleBodyStyleId,
                        principalTable: "VehicleBodyStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "VehicleModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    SeriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    YearFrom = table.Column<int>(type: "integer", nullable: true),
                    YearTo = table.Column<int>(type: "integer", nullable: true),
                    Trim = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EngineCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplacementCc = table.Column<int>(type: "integer", nullable: true),
                    FuelType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PowerKw = table.Column<int>(type: "integer", nullable: true),
                    TorqueNm = table.Column<int>(type: "integer", nullable: true),
                    TransmissionType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Gears = table.Column<int>(type: "integer", nullable: true),
                    DriveType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Doors = table.Column<int>(type: "integer", nullable: true),
                    Seats = table.Column<int>(type: "integer", nullable: true),
                    LengthMm = table.Column<int>(type: "integer", nullable: true),
                    WidthMm = table.Column<int>(type: "integer", nullable: true),
                    HeightMm = table.Column<int>(type: "integer", nullable: true),
                    WeightKg = table.Column<int>(type: "integer", nullable: true),
                    MaxSpeedKmh = table.Column<int>(type: "integer", nullable: true),
                    Acceleration0100s = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    Co2gKm = table.Column<int>(type: "integer", nullable: true),
                    FuelConsumptionL100km = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    VdsPattern = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleModels_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VinRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Vin = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    Wmi = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Vds = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Vis = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CheckDigit = table.Column<char>(type: "character(1)", nullable: false),
                    ModelYearChar = table.Column<char>(type: "character(1)", nullable: false),
                    PlantCode = table.Column<char>(type: "character(1)", nullable: false),
                    SequentialNumber = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ModelYear = table.Column<int>(type: "integer", nullable: true),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    ValidationErrors = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ManufacturerId = table.Column<Guid>(type: "uuid", nullable: true),
                    VehicleModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    CountryOfManufactureId = table.Column<Guid>(type: "uuid", nullable: true),
                    DecodedSource = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DecodedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RawApiResponse = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VinRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VinRecords_Countries_CountryOfManufactureId",
                        column: x => x.CountryOfManufactureId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VinRecords_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VinRecords_VehicleModels_VehicleModelId",
                        column: x => x.VehicleModelId,
                        principalTable: "VehicleModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "VinDecodeHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VinRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    DecodedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RawResponse = table.Column<string>(type: "text", nullable: true),
                    ChangedFields = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VinDecodeHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VinDecodeHistories_VinRecords_VinRecordId",
                        column: x => x.VinRecordId,
                        principalTable: "VinRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_ManufacturerId",
                table: "Brands",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_OriginCountryId",
                table: "Brands",
                column: "OriginCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Slug",
                table: "Brands",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_IsoCode2",
                table: "Countries",
                column: "IsoCode2",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_RegionId",
                table: "Countries",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_CountryId",
                table: "Manufacturers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_ParentManufacturerId",
                table: "Manufacturers",
                column: "ParentManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_BrandId",
                table: "Series",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Series_VehicleBodyStyleId",
                table: "Series",
                column: "VehicleBodyStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBodyStyles_VehicleTypeId",
                table: "VehicleBodyStyles",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleClasses_Code",
                table: "VehicleClasses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_SeriesId",
                table: "VehicleModels",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_VdsPattern",
                table: "VehicleModels",
                column: "VdsPattern",
                filter: "\"VdsPattern\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_VehicleClassId",
                table: "VehicleTypes",
                column: "VehicleClassId");

            migrationBuilder.CreateIndex(
                name: "IX_VinDecodeHistories_VinRecordId",
                table: "VinDecodeHistories",
                column: "VinRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_VinRecords_CountryOfManufactureId",
                table: "VinRecords",
                column: "CountryOfManufactureId");

            migrationBuilder.CreateIndex(
                name: "IX_VinRecords_ManufacturerId",
                table: "VinRecords",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_VinRecords_VehicleModelId",
                table: "VinRecords",
                column: "VehicleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_VinRecords_Vin",
                table: "VinRecords",
                column: "Vin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VinRecords_Wmi",
                table: "VinRecords",
                column: "Wmi");

            migrationBuilder.CreateIndex(
                name: "IX_WmcEntries_CodeType_Code",
                table: "WmcEntries",
                columns: new[] { "CodeType", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_WmcEntries_ManufacturerId",
                table: "WmcEntries",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_WmiAssignments_ManufacturerId",
                table: "WmiAssignments",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_WmiAssignments_Wmi_YearFrom_YearTo",
                table: "WmiAssignments",
                columns: new[] { "Wmi", "YearFrom", "YearTo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VinDecodeHistories");

            migrationBuilder.DropTable(
                name: "WmcEntries");

            migrationBuilder.DropTable(
                name: "WmiAssignments");

            migrationBuilder.DropTable(
                name: "VinRecords");

            migrationBuilder.DropTable(
                name: "VehicleModels");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "VehicleBodyStyles");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "VehicleTypes");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "VehicleClasses");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
