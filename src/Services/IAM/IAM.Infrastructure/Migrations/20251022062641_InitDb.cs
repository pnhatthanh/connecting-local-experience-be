using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_permission",
                columns: table => new
                {
                    id_permission = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_permission", x => x.id_permission);
                });

            migrationBuilder.CreateTable(
                name: "tbl_role",
                columns: table => new
                {
                    id_role = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_role", x => x.id_role);
                });

            migrationBuilder.CreateTable(
                name: "tbl_account",
                columns: table => new
                {
                    id_account = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_email_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    email_confirmation_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_token_expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_account", x => x.id_account);
                    table.ForeignKey(
                        name: "FK_tbl_account_tbl_role_role_id",
                        column: x => x.role_id,
                        principalTable: "tbl_role",
                        principalColumn: "id_role",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tbl_role_permission",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_role = table.Column<Guid>(type: "uuid", nullable: false),
                    id_permission = table.Column<Guid>(type: "uuid", nullable: false),
                    licensed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_role_permission_tbl_permission_id_permission",
                        column: x => x.id_permission,
                        principalTable: "tbl_permission",
                        principalColumn: "id_permission",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_role_permission_tbl_role_id_role",
                        column: x => x.id_role,
                        principalTable: "tbl_role",
                        principalColumn: "id_role",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_refresh_token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_account = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_refresh_token_tbl_account_id_account",
                        column: x => x.id_account,
                        principalTable: "tbl_account",
                        principalColumn: "id_account",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_account_email",
                table: "tbl_account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_account_role_id",
                table: "tbl_account",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_refresh_token_id_account",
                table: "tbl_refresh_token",
                column: "id_account");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_refresh_token_token",
                table: "tbl_refresh_token",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_role_permission_id_permission",
                table: "tbl_role_permission",
                column: "id_permission");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_role_permission_id_role_id_permission",
                table: "tbl_role_permission",
                columns: new[] { "id_role", "id_permission" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_refresh_token");

            migrationBuilder.DropTable(
                name: "tbl_role_permission");

            migrationBuilder.DropTable(
                name: "tbl_account");

            migrationBuilder.DropTable(
                name: "tbl_permission");

            migrationBuilder.DropTable(
                name: "tbl_role");
        }
    }
}
