using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cassandra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiDealer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DealerId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DealerReadModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealerReadModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DealerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateType = table.Column<string>(type: "text", nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    EventData = table.Column<string>(type: "text", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DealerReadModels_Code",
                table: "DealerReadModels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoredEvents_DealerId_AggregateId_AggregateType",
                table: "StoredEvents",
                columns: new[] { "DealerId", "AggregateId", "AggregateType" });

            migrationBuilder.CreateIndex(
                name: "IX_StoredEvents_DealerId_AggregateType_AggregateId_Version",
                table: "StoredEvents",
                columns: new[] { "DealerId", "AggregateType", "AggregateId", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DealerReadModels");

            migrationBuilder.DropTable(
                name: "StoredEvents");

            migrationBuilder.DropColumn(
                name: "DealerId",
                table: "AspNetUsers");
        }
    }
}
