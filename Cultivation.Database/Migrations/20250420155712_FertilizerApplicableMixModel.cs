using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class FertilizerApplicableMixModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FertilizerApplicableMix",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DonumCount = table.Column<double>(type: "float", nullable: false),
                    CurrentDonumCount = table.Column<double>(type: "float", nullable: false),
                    FertilizerMixId = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsValid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FertilizerApplicableMix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FertilizerApplicableMix_FertilizerMix_FertilizerMixId",
                        column: x => x.FertilizerMixId,
                        principalTable: "FertilizerMix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FertilizerApplicableMix_FertilizerMixId",
                table: "FertilizerApplicableMix",
                column: "FertilizerMixId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FertilizerApplicableMix");
        }
    }
}
