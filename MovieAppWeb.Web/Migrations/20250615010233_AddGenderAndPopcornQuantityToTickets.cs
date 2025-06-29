using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieAppWeb.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderAndPopcornQuantityToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopcornQuantity",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PopcornQuantity",
                table: "Tickets");
        }
    }
}
