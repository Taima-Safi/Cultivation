using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixFlowerOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowerOrder_Flower_FlowerId",
                table: "FlowerOrder");

            migrationBuilder.RenameColumn(
                name: "FlowerId",
                table: "FlowerOrder",
                newName: "FlowerStoreId");

            migrationBuilder.RenameIndex(
                name: "IX_FlowerOrder_FlowerId",
                table: "FlowerOrder",
                newName: "IX_FlowerOrder_FlowerStoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowerOrder_FlowerStore_FlowerStoreId",
                table: "FlowerOrder",
                column: "FlowerStoreId",
                principalTable: "FlowerStore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowerOrder_FlowerStore_FlowerStoreId",
                table: "FlowerOrder");

            migrationBuilder.RenameColumn(
                name: "FlowerStoreId",
                table: "FlowerOrder",
                newName: "FlowerId");

            migrationBuilder.RenameIndex(
                name: "IX_FlowerOrder_FlowerStoreId",
                table: "FlowerOrder",
                newName: "IX_FlowerOrder_FlowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowerOrder_Flower_FlowerId",
                table: "FlowerOrder",
                column: "FlowerId",
                principalTable: "Flower",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
