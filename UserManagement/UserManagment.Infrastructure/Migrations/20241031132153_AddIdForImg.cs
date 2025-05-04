using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdForImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserImage",
                table: "User");

            migrationBuilder.AddColumn<Guid>(
                name: "UserImageId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserImageId",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "UserImage",
                table: "User",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
