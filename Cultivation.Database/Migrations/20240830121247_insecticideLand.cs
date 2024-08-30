using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class insecticideLand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "Insecticide");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Insecticide",
                newName: "Description");

            migrationBuilder.CreateTable(
                name: "InsecticideLand",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Liter = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: true),
                    LandId = table.Column<long>(type: "bigint", nullable: false),
                    InsecticideId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsecticideLand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsecticideLand_Insecticide_InsecticideId",
                        column: x => x.InsecticideId,
                        principalTable: "Insecticide",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsecticideLand_Land_LandId",
                        column: x => x.LandId,
                        principalTable: "Land",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideLand_InsecticideId",
                table: "InsecticideLand",
                column: "InsecticideId");

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideLand_LandId",
                table: "InsecticideLand",
                column: "LandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsecticideLand");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Insecticide",
                newName: "Note");

            migrationBuilder.AddColumn<string>(
                name: "File",
                table: "Insecticide",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
