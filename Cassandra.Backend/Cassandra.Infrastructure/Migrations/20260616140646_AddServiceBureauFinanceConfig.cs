using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cassandra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceBureauFinanceConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BiayaBiroJasaItemReadModels",
                columns: table => new
                {
                    BiayaBiroJasaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    BiayaStnk = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiayaBiroJasaItemReadModels", x => new { x.BiayaBiroJasaId, x.TipeMotorId });
                });

            migrationBuilder.CreateTable(
                name: "BiayaBiroJasaReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SamsatId = table.Column<Guid>(type: "uuid", nullable: false),
                    BiroId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiayaBiroJasaReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BiroReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Fax = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Pic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PphRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiroReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseTypeReadModels",
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
                    table.PrimaryKey("PK_ExpenseTypeReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LedgerReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PelanggaranWilayahReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AreaCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExtraFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PelanggaranWilayahReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SamsatCityReadModels",
                columns: table => new
                {
                    SamsatId = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamsatCityReadModels", x => new { x.SamsatId, x.City });
                });

            migrationBuilder.CreateTable(
                name: "SamsatReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamsatReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BiayaBiroJasaReadModels_DealerId_SamsatId_BiroId",
                table: "BiayaBiroJasaReadModels",
                columns: new[] { "DealerId", "SamsatId", "BiroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BiroReadModels_DealerId_Code",
                table: "BiroReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTypeReadModels_DealerId_Code",
                table: "ExpenseTypeReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerReadModels_DealerId_Name",
                table: "LedgerReadModels",
                columns: new[] { "DealerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PelanggaranWilayahReadModels_DealerId_AreaCode",
                table: "PelanggaranWilayahReadModels",
                columns: new[] { "DealerId", "AreaCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SamsatReadModels_DealerId_Name",
                table: "SamsatReadModels",
                columns: new[] { "DealerId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BiayaBiroJasaItemReadModels");

            migrationBuilder.DropTable(
                name: "BiayaBiroJasaReadModels");

            migrationBuilder.DropTable(
                name: "BiroReadModels");

            migrationBuilder.DropTable(
                name: "ExpenseTypeReadModels");

            migrationBuilder.DropTable(
                name: "LedgerReadModels");

            migrationBuilder.DropTable(
                name: "PelanggaranWilayahReadModels");

            migrationBuilder.DropTable(
                name: "SamsatCityReadModels");

            migrationBuilder.DropTable(
                name: "SamsatReadModels");
        }
    }
}
