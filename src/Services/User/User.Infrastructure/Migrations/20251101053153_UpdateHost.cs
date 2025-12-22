using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "tbl_users",
                type: "text",
                nullable: false,
                defaultValue: "active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "tbl_users");
        }
    }
}
