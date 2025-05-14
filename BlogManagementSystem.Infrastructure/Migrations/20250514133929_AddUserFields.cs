using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "local_user_identities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "organization",
                table: "local_user_identities",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "local_user_identities");

            migrationBuilder.DropColumn(
                name: "organization",
                table: "local_user_identities");
        }
    }
}
