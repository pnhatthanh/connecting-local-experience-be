using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateIam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old identity column
            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_role_permission",
                table: "tbl_role_permission");

            migrationBuilder.DropColumn(
                name: "id",
                table: "tbl_role_permission");

            // Add new UUID column
            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "tbl_role_permission",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_role_permission",
                table: "tbl_role_permission",
                column: "id");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "tbl_role_permission",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "tbl_role_permission",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "tbl_role_permission");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "tbl_role_permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_role_permission",
                table: "tbl_role_permission");

            migrationBuilder.DropColumn(
                name: "id",
                table: "tbl_role_permission");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "tbl_role_permission",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_role_permission",
                table: "tbl_role_permission",
                column: "id");
        }
    }
}
