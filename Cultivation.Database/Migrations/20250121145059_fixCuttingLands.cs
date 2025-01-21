using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class fixCuttingLands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerMixLand_CuttingLand_CuttingLandId",
                table: "FertilizerMixLand");

            migrationBuilder.RenameColumn(
                name: "CuttingLandId",
                table: "FertilizerMixLand",
                newName: "LandId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerMixLand_CuttingLandId",
                table: "FertilizerMixLand",
                newName: "IX_FertilizerMixLand_LandId");

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerMixLand_Land_LandId",
                table: "FertilizerMixLand",
                column: "LandId",
                principalTable: "Land",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerMixLand_Land_LandId",
                table: "FertilizerMixLand");

            migrationBuilder.RenameColumn(
                name: "LandId",
                table: "FertilizerMixLand",
                newName: "CuttingLandId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerMixLand_LandId",
                table: "FertilizerMixLand",
                newName: "IX_FertilizerMixLand_CuttingLandId");

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerMixLand_CuttingLand_CuttingLandId",
                table: "FertilizerMixLand",
                column: "CuttingLandId",
                principalTable: "CuttingLand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
