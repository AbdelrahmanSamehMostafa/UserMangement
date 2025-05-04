using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsMainMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "URL",
                table: "Screen",
                newName: "AreaName");

            migrationBuilder.AddColumn<bool>(
                name: "ISMainMenu",
                table: "Screen",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISMainMenu",
                table: "Screen");

            migrationBuilder.RenameColumn(
                name: "AreaName",
                table: "Screen",
                newName: "URL");
        }
    }
}
