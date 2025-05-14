using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalIdentityEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "local_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    parent_group_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_local_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_local_groups_local_groups_parent_group_id",
                        column: x => x.parent_group_id,
                        principalTable: "local_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "local_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_local_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "local_user_groups",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_local_user_groups", x => new { x.user_id, x.group_id });
                    table.ForeignKey(
                        name: "fk_local_user_groups_local_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "local_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "local_user_roles",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_local_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_local_user_roles_local_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "local_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_local_groups_name",
                table: "local_groups",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_local_groups_parent_group_id",
                table: "local_groups",
                column: "parent_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_local_groups_path",
                table: "local_groups",
                column: "path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_local_roles_name",
                table: "local_roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_local_user_groups_group_id",
                table: "local_user_groups",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_local_user_roles_role_id",
                table: "local_user_roles",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "local_user_groups");

            migrationBuilder.DropTable(
                name: "local_user_roles");

            migrationBuilder.DropTable(
                name: "local_groups");

            migrationBuilder.DropTable(
                name: "local_roles");
        }
    }
}
