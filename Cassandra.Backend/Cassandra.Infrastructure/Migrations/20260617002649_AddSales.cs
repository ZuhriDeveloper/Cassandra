using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cassandra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PengirimanMotorReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrasiPenjualanId = table.Column<Guid>(type: "uuid", nullable: false),
                    NoMesin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Driver1Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Driver2Id = table.Column<Guid>(type: "uuid", nullable: true),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Zona = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PengirimanMotorReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrasiPenjualanReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    NoPenjualan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SaleDate = table.Column<DateOnly>(type: "date", nullable: false),
                    KaryawanId = table.Column<Guid>(type: "uuid", nullable: false),
                    KiosId = table.Column<Guid>(type: "uuid", nullable: false),
                    MediatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    MetodePenjualan = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TipePenjualan = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NoMesin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NoRangka = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NamaCustomer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Phone1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Phone2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OffRoad = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Bbn = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ApprovedDiscount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OriginalDiscount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AmbilUang = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Dp = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Angsuran = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tac = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DaftarHargaLeasingId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenorCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TipeMotorCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WarnaName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SerahTerimaKendaraanId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TandaTerimaSementaraId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Kelengkapan = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false),
                    IsVoid = table.Column<bool>(type: "boolean", nullable: false),
                    EnableToVoid = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrasiPenjualanReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrasiPenjualanReadModels_DealerId_NoPenjualan",
                table: "RegistrasiPenjualanReadModels",
                columns: new[] { "DealerId", "NoPenjualan" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PengirimanMotorReadModels");

            migrationBuilder.DropTable(
                name: "RegistrasiPenjualanReadModels");
        }
    }
}
