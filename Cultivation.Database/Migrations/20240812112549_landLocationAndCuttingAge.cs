using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class landLocationAndCuttingAge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Land",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "Quantity",
                table: "CuttingLand",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Cutting",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Land");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Cutting");

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "CuttingLand",
                type: "double",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
