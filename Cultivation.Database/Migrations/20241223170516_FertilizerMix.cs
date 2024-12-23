using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class FertilizerMix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FertilizerMix",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerMix", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FertilizerMixDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    FertilizerId = table.Column<long>(type: "bigint", nullable: false),
                    FertilizerMixId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerMixDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerMixDetail_FertilizerMix_FertilizerMixId",
                        column: x => x.FertilizerMixId,
                        principalTable: "FertilizerMix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FertilizerMixDetail_Fertilizer_FertilizerId",
                        column: x => x.FertilizerId,
                        principalTable: "Fertilizer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerMixDetail_FertilizerId",
                table: "FertilizerMixDetail",
                column: "FertilizerId");

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerMixDetail_FertilizerMixId",
                table: "FertilizerMixDetail",
                column: "FertilizerMixId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FertilizerMixDetail");

            migrationBuilder.DropTable(
                name: "FertilizerMix");
        }
    }
}
