using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirportDemo.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartureLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartureLocation",
                table: "Flights",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartureLocation",
                table: "Flights");
        }
    }
}
