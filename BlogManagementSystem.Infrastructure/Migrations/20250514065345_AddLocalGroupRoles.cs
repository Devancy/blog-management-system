using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalGroupRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "local_group_roles",
                columns: table => new
                {
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_local_group_roles", x => new { x.group_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_local_group_roles_local_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "local_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_local_group_roles_local_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "local_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_local_group_roles_role_id",
                table: "local_group_roles",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "local_group_roles");
        }
    }
}
