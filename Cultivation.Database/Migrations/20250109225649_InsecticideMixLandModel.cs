using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class InsecticideMixLandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "InsecticideMix",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InsecticideMixLand",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CuttingLandId = table.Column<long>(type: "bigint", nullable: false),
                    InsecticideMixId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsecticideMixLand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsecticideMixLand_CuttingLand_CuttingLandId",
                        column: x => x.CuttingLandId,
                        principalTable: "CuttingLand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsecticideMixLand_InsecticideMix_InsecticideMixId",
                        column: x => x.InsecticideMixId,
                        principalTable: "InsecticideMix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideMixLand_CuttingLandId",
                table: "InsecticideMixLand",
                column: "CuttingLandId");

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideMixLand_InsecticideMixId",
                table: "InsecticideMixLand",
                column: "InsecticideMixId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsecticideMixLand");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "InsecticideMix");
        }
    }
}
