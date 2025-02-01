using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class linkedInsecticideLandwithLand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsecticideLand_CuttingLand_CuttingLandId",
                table: "InsecticideLand");

            migrationBuilder.RenameColumn(
                name: "CuttingLandId",
                table: "InsecticideLand",
                newName: "LandId");

            migrationBuilder.RenameIndex(
                name: "IX_InsecticideLand_CuttingLandId",
                table: "InsecticideLand",
                newName: "IX_InsecticideLand_LandId");

            migrationBuilder.AddColumn<long>(
                name: "CuttingLandModelId",
                table: "InsecticideLand",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideLand_CuttingLandModelId",
                table: "InsecticideLand",
                column: "CuttingLandModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsecticideLand_CuttingLand_CuttingLandModelId",
                table: "InsecticideLand",
                column: "CuttingLandModelId",
                principalTable: "CuttingLand",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InsecticideLand_Land_LandId",
                table: "InsecticideLand",
                column: "LandId",
                principalTable: "Land",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsecticideLand_CuttingLand_CuttingLandModelId",
                table: "InsecticideLand");

            migrationBuilder.DropForeignKey(
                name: "FK_InsecticideLand_Land_LandId",
                table: "InsecticideLand");

            migrationBuilder.DropIndex(
                name: "IX_InsecticideLand_CuttingLandModelId",
                table: "InsecticideLand");

            migrationBuilder.DropColumn(
                name: "CuttingLandModelId",
                table: "InsecticideLand");

            migrationBuilder.RenameColumn(
                name: "LandId",
                table: "InsecticideLand",
                newName: "CuttingLandId");

            migrationBuilder.RenameIndex(
                name: "IX_InsecticideLand_LandId",
                table: "InsecticideLand",
                newName: "IX_InsecticideLand_CuttingLandId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsecticideLand_CuttingLand_CuttingLandId",
                table: "InsecticideLand",
                column: "CuttingLandId",
                principalTable: "CuttingLand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
