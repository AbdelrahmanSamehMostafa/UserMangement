using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixNamingIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "content",
                table: "Attachment",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "Extenstion",
                table: "Attachment",
                newName: "Extension");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserImageId",
                table: "User",
                column: "UserImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Attachment_UserImageId",
                table: "User",
                column: "UserImageId",
                principalTable: "Attachment",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Attachment_UserImageId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_UserImageId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Attachment",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Extension",
                table: "Attachment",
                newName: "Extenstion");
        }
    }
}
