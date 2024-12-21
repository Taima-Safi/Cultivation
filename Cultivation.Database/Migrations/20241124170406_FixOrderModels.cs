using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlowerModelFlowerOrderModel");

            migrationBuilder.DropTable(
                name: "FlowerOrderModelOrderModel");

            migrationBuilder.CreateIndex(
                name: "IX_FlowerOrder_FlowerId",
                table: "FlowerOrder",
                column: "FlowerId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowerOrder_OrderId",
                table: "FlowerOrder",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowerOrder_Flower_FlowerId",
                table: "FlowerOrder",
                column: "FlowerId",
                principalTable: "Flower",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowerOrder_Order_OrderId",
                table: "FlowerOrder",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowerOrder_Flower_FlowerId",
                table: "FlowerOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowerOrder_Order_OrderId",
                table: "FlowerOrder");

            migrationBuilder.DropIndex(
                name: "IX_FlowerOrder_FlowerId",
                table: "FlowerOrder");

            migrationBuilder.DropIndex(
                name: "IX_FlowerOrder_OrderId",
                table: "FlowerOrder");

            migrationBuilder.CreateTable(
                name: "FlowerModelFlowerOrderModel",
                columns: table => new
                {
                    FlowerId = table.Column<long>(type: "bigint", nullable: false),
                    FlowerOrdersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowerModelFlowerOrderModel", x => new { x.FlowerId, x.FlowerOrdersId });
                    table.ForeignKey(
                        name: "FK_FlowerModelFlowerOrderModel_FlowerOrder_FlowerOrdersId",
                        column: x => x.FlowerOrdersId,
                        principalTable: "FlowerOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlowerModelFlowerOrderModel_Flower_FlowerId",
                        column: x => x.FlowerId,
                        principalTable: "Flower",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowerOrderModelOrderModel",
                columns: table => new
                {
                    FlowerOrdersId = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowerOrderModelOrderModel", x => new { x.FlowerOrdersId, x.OrderId });
                    table.ForeignKey(
                        name: "FK_FlowerOrderModelOrderModel_FlowerOrder_FlowerOrdersId",
                        column: x => x.FlowerOrdersId,
                        principalTable: "FlowerOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlowerOrderModelOrderModel_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlowerModelFlowerOrderModel_FlowerOrdersId",
                table: "FlowerModelFlowerOrderModel",
                column: "FlowerOrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowerOrderModelOrderModel_OrderId",
                table: "FlowerOrderModelOrderModel",
                column: "OrderId");
        }
    }
}
