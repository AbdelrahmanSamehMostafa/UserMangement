using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyAuditName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrailType",
                table: "AuditTrails",
                newName: "LogsType");

            migrationBuilder.RenameColumn(
                name: "DateUtc",
                table: "AuditTrails",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "LogName",
                table: "AuditTrails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogName",
                table: "AuditTrails");

            migrationBuilder.RenameColumn(
                name: "LogsType",
                table: "AuditTrails",
                newName: "TrailType");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "AuditTrails",
                newName: "DateUtc");
        }
    }
}
