using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalUserIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "local_user_identities",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_local_user_identities", x => x.user_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_local_user_identities_email",
                table: "local_user_identities",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_local_user_identities_username",
                table: "local_user_identities",
                column: "username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_local_user_groups_local_user_identities_user_id",
                table: "local_user_groups",
                column: "user_id",
                principalTable: "local_user_identities",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_local_user_roles_local_user_identities_user_id",
                table: "local_user_roles",
                column: "user_id",
                principalTable: "local_user_identities",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_local_user_groups_local_user_identities_user_id",
                table: "local_user_groups");

            migrationBuilder.DropForeignKey(
                name: "fk_local_user_roles_local_user_identities_user_id",
                table: "local_user_roles");

            migrationBuilder.DropTable(
                name: "local_user_identities");
        }
    }
}
