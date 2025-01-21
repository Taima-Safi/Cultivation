using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class fixCuttingLandsforinsecticide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsecticideMixLand_CuttingLand_CuttingLandId",
                table: "InsecticideMixLand");

            migrationBuilder.RenameColumn(
                name: "CuttingLandId",
                table: "InsecticideMixLand",
                newName: "LandId");

            migrationBuilder.RenameIndex(
                name: "IX_InsecticideMixLand_CuttingLandId",
                table: "InsecticideMixLand",
                newName: "IX_InsecticideMixLand_LandId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsecticideMixLand_Land_LandId",
                table: "InsecticideMixLand",
                column: "LandId",
                principalTable: "Land",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsecticideMixLand_Land_LandId",
                table: "InsecticideMixLand");

            migrationBuilder.RenameColumn(
                name: "LandId",
                table: "InsecticideMixLand",
                newName: "CuttingLandId");

            migrationBuilder.RenameIndex(
                name: "IX_InsecticideMixLand_LandId",
                table: "InsecticideMixLand",
                newName: "IX_InsecticideMixLand_CuttingLandId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsecticideMixLand_CuttingLand_CuttingLandId",
                table: "InsecticideMixLand",
                column: "CuttingLandId",
                principalTable: "CuttingLand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
