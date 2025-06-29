using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAppWeb.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHallLayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rows",
                table: "Halls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeatsPerRow",
                table: "Halls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rows",
                table: "Halls");

            migrationBuilder.DropColumn(
                name: "SeatsPerRow",
                table: "Halls");
        }
    }
}
