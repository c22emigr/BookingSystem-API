using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResourceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Resources");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Resources",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Resources");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Resources",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
