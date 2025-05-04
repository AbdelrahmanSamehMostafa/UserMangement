using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseModelIntoAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "AuditTrails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "AuditTrails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InsertedBy",
                table: "AuditTrails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "AuditTrails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AuditTrails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "AuditTrails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "AuditTrails",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "InsertedBy",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AuditTrails");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "AuditTrails");
        }
    }
}
