using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class InsecticideMix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsecticideMix",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsecticideMix", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsecticideMixDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Liter = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: true),
                    InsecticideId = table.Column<long>(type: "bigint", nullable: false),
                    InsecticideMixId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsecticideMixDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsecticideMixDetail_InsecticideMix_InsecticideMixId",
                        column: x => x.InsecticideMixId,
                        principalTable: "InsecticideMix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsecticideMixDetail_Insecticide_InsecticideId",
                        column: x => x.InsecticideId,
                        principalTable: "Insecticide",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideMixDetail_InsecticideId",
                table: "InsecticideMixDetail",
                column: "InsecticideId");

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideMixDetail_InsecticideMixId",
                table: "InsecticideMixDetail",
                column: "InsecticideMixId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsecticideMixDetail");

            migrationBuilder.DropTable(
                name: "InsecticideMix");
        }
    }
}
