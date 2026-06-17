using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cassandra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MutasiItemReadModels",
                columns: table => new
                {
                    MutasiId = table.Column<Guid>(type: "uuid", nullable: false),
                    NoMesin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutasiItemReadModels", x => new { x.MutasiId, x.NoMesin });
                });

            migrationBuilder.CreateTable(
                name: "MutasiKelengkapanReadModels",
                columns: table => new
                {
                    MutasiId = table.Column<Guid>(type: "uuid", nullable: false),
                    KelengkapanName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutasiKelengkapanReadModels", x => new { x.MutasiId, x.KelengkapanName });
                });

            migrationBuilder.CreateTable(
                name: "MutasiReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    MutasiNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MutasiDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SourceKiosId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationKiosId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MutasiReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoItemReadModels",
                columns: table => new
                {
                    SoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarnaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    NettPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoItemReadModels", x => new { x.SoId, x.TipeMotorId, x.WarnaId });
                });

            migrationBuilder.CreateTable(
                name: "SoPenerimaanItemKelengkapanReadModels",
                columns: table => new
                {
                    SoPenerimaanId = table.Column<Guid>(type: "uuid", nullable: false),
                    KelengkapanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoPenerimaanItemKelengkapanReadModels", x => new { x.SoPenerimaanId, x.KelengkapanId });
                });

            migrationBuilder.CreateTable(
                name: "SoPenerimaanItemMotorReadModels",
                columns: table => new
                {
                    SoPenerimaanId = table.Column<Guid>(type: "uuid", nullable: false),
                    NoMesin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarnaId = table.Column<Guid>(type: "uuid", nullable: false),
                    NoRangka = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    KiosId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssemblyYear = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoPenerimaanItemMotorReadModels", x => new { x.SoPenerimaanId, x.NoMesin });
                });

            migrationBuilder.CreateTable(
                name: "SoPenerimaanReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SuratJalanId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SuratJalanDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoPenerimaanReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SoNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SoDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaymentType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MetodeKeuanganId = table.Column<Guid>(type: "uuid", nullable: false),
                    QtyUnit = table.Column<int>(type: "integer", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Subsidi = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CashDiscount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PPn = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Df = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoReturItemReadModels",
                columns: table => new
                {
                    SoReturId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarnaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    NettPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoReturItemReadModels", x => new { x.SoReturId, x.TipeMotorId, x.WarnaId });
                });

            migrationBuilder.CreateTable(
                name: "SoReturReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReturNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReturDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PPn = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoReturReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    NoMesin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NoRangka = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarnaId = table.Column<Guid>(type: "uuid", nullable: false),
                    KiosId = table.Column<Guid>(type: "uuid", nullable: false),
                    SuratJalanId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SuratJalanDate = table.Column<DateOnly>(type: "date", nullable: false),
                    SoId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssemblyYear = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MutasiReadModels_DealerId_MutasiNumber",
                table: "MutasiReadModels",
                columns: new[] { "DealerId", "MutasiNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoPenerimaanReadModels_DealerId_SuratJalanId",
                table: "SoPenerimaanReadModels",
                columns: new[] { "DealerId", "SuratJalanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoReadModels_DealerId_SoNumber",
                table: "SoReadModels",
                columns: new[] { "DealerId", "SoNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoReturReadModels_DealerId_ReturNumber",
                table: "SoReturReadModels",
                columns: new[] { "DealerId", "ReturNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockReadModels_DealerId_NoMesin",
                table: "StockReadModels",
                columns: new[] { "DealerId", "NoMesin" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MutasiItemReadModels");

            migrationBuilder.DropTable(
                name: "MutasiKelengkapanReadModels");

            migrationBuilder.DropTable(
                name: "MutasiReadModels");

            migrationBuilder.DropTable(
                name: "SoItemReadModels");

            migrationBuilder.DropTable(
                name: "SoPenerimaanItemKelengkapanReadModels");

            migrationBuilder.DropTable(
                name: "SoPenerimaanItemMotorReadModels");

            migrationBuilder.DropTable(
                name: "SoPenerimaanReadModels");

            migrationBuilder.DropTable(
                name: "SoReadModels");

            migrationBuilder.DropTable(
                name: "SoReturItemReadModels");

            migrationBuilder.DropTable(
                name: "SoReturReadModels");

            migrationBuilder.DropTable(
                name: "StockReadModels");
        }
    }
}
