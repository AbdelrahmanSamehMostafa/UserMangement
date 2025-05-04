using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class configurationdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    ConfigKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConfigValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfigType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsertedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.ConfigKey);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configurations_ConfigKey",
                table: "Configurations",
                column: "ConfigKey",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configurations");
        }
    }
}
