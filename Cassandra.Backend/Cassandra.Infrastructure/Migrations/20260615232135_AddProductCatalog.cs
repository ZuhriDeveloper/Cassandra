using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cassandra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GrupTipeMotorReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupTipeMotorReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KelengkapanReadModels",
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
                    table.PrimaryKey("PK_KelengkapanReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipeMotorReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    GrupTipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShortName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WmsCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AhmCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EngineNumberFormat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ChassisNumberFormat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NettPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OrJakarta = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OrTangerang = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    BbnJakarta = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    BbnTangerang = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipeMotorReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipeMotorWarnaReadModels",
                columns: table => new
                {
                    TipeMotorId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarnaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipeMotorWarnaReadModels", x => new { x.TipeMotorId, x.WarnaId });
                });

            migrationBuilder.CreateTable(
                name: "WarnaReadModels",
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
                    table.PrimaryKey("PK_WarnaReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrupTipeMotorReadModels_DealerId_Code",
                table: "GrupTipeMotorReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KelengkapanReadModels_DealerId_Name",
                table: "KelengkapanReadModels",
                columns: new[] { "DealerId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipeMotorReadModels_DealerId_Code",
                table: "TipeMotorReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarnaReadModels_DealerId_Code",
                table: "WarnaReadModels",
                columns: new[] { "DealerId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrupTipeMotorReadModels");

            migrationBuilder.DropTable(
                name: "KelengkapanReadModels");

            migrationBuilder.DropTable(
                name: "TipeMotorReadModels");

            migrationBuilder.DropTable(
                name: "TipeMotorWarnaReadModels");

            migrationBuilder.DropTable(
                name: "WarnaReadModels");
        }
    }
}
