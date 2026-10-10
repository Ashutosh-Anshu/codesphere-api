using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace codesphere_api.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class addIconInMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Menus",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Menus");
        }
    }
}
