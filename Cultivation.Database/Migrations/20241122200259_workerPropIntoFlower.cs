using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cultivation.Database.Migrations
{
    /// <inheritdoc />
    public partial class workerPropIntoFlower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Worker",
                table: "Flower",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Worker",
                table: "Flower");
        }
    }
}
