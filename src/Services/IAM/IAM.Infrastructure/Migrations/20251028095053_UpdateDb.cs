using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "tbl_permission");

            migrationBuilder.AddColumn<string>(
                name: "permission_code",
                table: "tbl_permission",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "permission_code",
                table: "tbl_permission");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "tbl_permission",
                type: "text",
                nullable: true);
        }
    }
}
