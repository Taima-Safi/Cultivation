using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class insecticideApplicable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsecticideApplicableMix",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonumCount = table.Column<double>(type: "float", nullable: false),
                    CurrentDonumCount = table.Column<double>(type: "float", nullable: false),
                    InsecticideMixId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsecticideApplicableMix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsecticideApplicableMix_InsecticideMix_InsecticideMixId",
                        column: x => x.InsecticideMixId,
                        principalTable: "InsecticideMix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsecticideStore",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalQuantity = table.Column<double>(type: "float", nullable: false),
                    InsecticideId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsecticideStore", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsecticideStore_Insecticide_InsecticideId",
                        column: x => x.InsecticideId,
                        principalTable: "Insecticide",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsecticideTransaction",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAdd = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityChange = table.Column<double>(type: "float", nullable: false),
                    InsecticideId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsecticideTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsecticideTransaction_Insecticide_InsecticideId",
                        column: x => x.InsecticideId,
                        principalTable: "Insecticide",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideApplicableMix_InsecticideMixId",
                table: "InsecticideApplicableMix",
                column: "InsecticideMixId");

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideStore_InsecticideId",
                table: "InsecticideStore",
                column: "InsecticideId");

            migrationBuilder.CreateIndex(
                name: "IX_InsecticideTransaction_InsecticideId",
                table: "InsecticideTransaction",
                column: "InsecticideId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsecticideApplicableMix");

            migrationBuilder.DropTable(
                name: "InsecticideStore");

            migrationBuilder.DropTable(
                name: "InsecticideTransaction");
        }
    }
}
