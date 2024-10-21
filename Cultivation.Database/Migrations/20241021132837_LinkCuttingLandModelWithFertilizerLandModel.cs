using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class LinkCuttingLandModelWithFertilizerLandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerLand_Land_LandId",
                table: "FertilizerLand");

            migrationBuilder.RenameColumn(
                name: "LandId",
                table: "FertilizerLand",
                newName: "CuttingLandId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerLand_LandId",
                table: "FertilizerLand",
                newName: "IX_FertilizerLand_CuttingLandId");

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerLand_CuttingLand_CuttingLandId",
                table: "FertilizerLand",
                column: "CuttingLandId",
                principalTable: "CuttingLand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FertilizerLand_CuttingLand_CuttingLandId",
                table: "FertilizerLand");

            migrationBuilder.RenameColumn(
                name: "CuttingLandId",
                table: "FertilizerLand",
                newName: "LandId");

            migrationBuilder.RenameIndex(
                name: "IX_FertilizerLand_CuttingLandId",
                table: "FertilizerLand",
                newName: "IX_FertilizerLand_LandId");

            migrationBuilder.AddForeignKey(
                name: "FK_FertilizerLand_Land_LandId",
                table: "FertilizerLand",
                column: "LandId",
                principalTable: "Land",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
