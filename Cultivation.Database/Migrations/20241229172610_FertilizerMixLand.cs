using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class FertilizerMixLand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FertilizerMixLand",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CuttingLandId = table.Column<long>(type: "bigint", nullable: false),
                    FertilizerMixId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerMixLand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerMixLand_CuttingLand_CuttingLandId",
                        column: x => x.CuttingLandId,
                        principalTable: "CuttingLand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerMixLand_FertilizerMix_FertilizerMixId",
                        column: x => x.FertilizerMixId,
                        principalTable: "FertilizerMix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerMixLand_CuttingLandId",
                table: "FertilizerMixLand",
                column: "CuttingLandId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerMixLand_FertilizerMixId",
                table: "FertilizerMixLand",
                column: "FertilizerMixId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FertilizerMixLand");
        }
    }
}
