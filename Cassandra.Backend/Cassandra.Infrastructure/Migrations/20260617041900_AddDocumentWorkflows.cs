using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cassandra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BpkbReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrasiPenjualanId = table.Column<Guid>(type: "uuid", nullable: false),
                    StnkId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BpkbNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BookNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReceiveDate = table.Column<DateOnly>(type: "date", nullable: true),
                    HandoverDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Receiver = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BpkbReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StnkReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrasiPenjualanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FakturDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FakturName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FakturAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProcessDate = table.Column<DateOnly>(type: "date", nullable: true),
                    BiroId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PlateNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    StnkNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    StnkCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ProgressiveCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NoticeCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ReceiveDate = table.Column<DateOnly>(type: "date", nullable: true),
                    HandoverDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StnkReceiver = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BbnCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PnbpCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AdminCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OtherCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ServiceCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PphCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsInvoiceValid = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StnkReadModels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BpkbReadModels_StnkId",
                table: "BpkbReadModels",
                column: "StnkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StnkReadModels_RegistrasiPenjualanId",
                table: "StnkReadModels",
                column: "RegistrasiPenjualanId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BpkbReadModels");

            migrationBuilder.DropTable(
                name: "StnkReadModels");
        }
    }
}
