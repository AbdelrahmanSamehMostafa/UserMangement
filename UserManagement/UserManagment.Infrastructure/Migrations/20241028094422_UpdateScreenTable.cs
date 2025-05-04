using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScreenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISMainMenu",
                table: "Screen");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Screen",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Screen_ParentId",
                table: "Screen",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Screen_Screen_ParentId",
                table: "Screen",
                column: "ParentId",
                principalTable: "Screen",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Screen_Screen_ParentId",
                table: "Screen");

            migrationBuilder.DropIndex(
                name: "IX_Screen_ParentId",
                table: "Screen");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Screen");

            migrationBuilder.AddColumn<bool>(
                name: "ISMainMenu",
                table: "Screen",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
