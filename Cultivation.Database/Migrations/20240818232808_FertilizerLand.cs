using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class FertilizerLand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Fertilizer");

            migrationBuilder.CreateTable(
                name: "FertilizerLand",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Quantity = table.Column<double>(type: "double", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    LandId = table.Column<long>(type: "bigint", nullable: false),
                    FertilizerId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerLand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerLand_Fertilizer_FertilizerId",
                        column: x => x.FertilizerId,
                        principalTable: "Fertilizer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerLand_Land_LandId",
                        column: x => x.LandId,
                        principalTable: "Land",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerLand_FertilizerId",
                table: "FertilizerLand",
                column: "FertilizerId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerLand_LandId",
                table: "FertilizerLand",
                column: "LandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FertilizerLand");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Fertilizer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
