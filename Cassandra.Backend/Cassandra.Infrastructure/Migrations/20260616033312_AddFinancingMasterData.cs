using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cassandra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancingMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlokasiDiskonReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    KaryawanId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlokasiDiskonReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CabangLeasingReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Fax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Contact = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GlobalLeasingId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabangLeasingReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DaftarHargaLeasingItemReadModels",
                columns: table => new
                {
                    DaftarHargaLeasingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupTipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subsidi = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Incentive = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    LainLain = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaftarHargaLeasingItemReadModels", x => new { x.DaftarHargaLeasingId, x.GrupTipeMotorId });
                });

            migrationBuilder.CreateTable(
                name: "DaftarHargaLeasingReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GlobalLeasingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupTenorId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaftarHargaLeasingReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DfReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DfReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCashReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DirectDiscount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ChannelDiscount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCashReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountItemReadModels",
                columns: table => new
                {
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupTipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountItemReadModels", x => new { x.DiscountId, x.GrupTipeMotorId });
                });

            migrationBuilder.CreateTable(
                name: "DiscountReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DaftarHargaLeasingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GlobalLeasingReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Fax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Contact = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalLeasingReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrupTenorReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupTenorReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetodeKeuanganReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodeKeuanganReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenorReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Months = table.Column<int>(type: "integer", nullable: false),
                    GrupTenorId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenorReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlokasiDiskonReadModels_DealerId_KaryawanId",
                table: "AlokasiDiskonReadModels",
                columns: new[] { "DealerId", "KaryawanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CabangLeasingReadModels_DealerId_Code",
                table: "CabangLeasingReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DaftarHargaLeasingReadModels_DealerId_Name_GlobalLeasingId_~",
                table: "DaftarHargaLeasingReadModels",
                columns: new[] { "DealerId", "Name", "GlobalLeasingId", "GrupTenorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DfReadModels_DealerId",
                table: "DfReadModels",
                column: "DealerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCashReadModels_DealerId_TipeMotorId",
                table: "DiscountCashReadModels",
                columns: new[] { "DealerId", "TipeMotorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountReadModels_DealerId_DaftarHargaLeasingId_Level",
                table: "DiscountReadModels",
                columns: new[] { "DealerId", "DaftarHargaLeasingId", "Level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlobalLeasingReadModels_DealerId_Code",
                table: "GlobalLeasingReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrupTenorReadModels_DealerId_Code",
                table: "GrupTenorReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MetodeKeuanganReadModels_DealerId_Code",
                table: "MetodeKeuanganReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenorReadModels_DealerId_Code",
                table: "TenorReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlokasiDiskonReadModels");

            migrationBuilder.DropTable(
                name: "CabangLeasingReadModels");

            migrationBuilder.DropTable(
                name: "DaftarHargaLeasingItemReadModels");

            migrationBuilder.DropTable(
                name: "DaftarHargaLeasingReadModels");

            migrationBuilder.DropTable(
                name: "DfReadModels");

            migrationBuilder.DropTable(
                name: "DiscountCashReadModels");

            migrationBuilder.DropTable(
                name: "DiscountItemReadModels");

            migrationBuilder.DropTable(
                name: "DiscountReadModels");

            migrationBuilder.DropTable(
                name: "GlobalLeasingReadModels");

            migrationBuilder.DropTable(
                name: "GrupTenorReadModels");

            migrationBuilder.DropTable(
                name: "MetodeKeuanganReadModels");

            migrationBuilder.DropTable(
                name: "TenorReadModels");
        }
    }
}
