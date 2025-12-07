using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Experience.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHostIdToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HostId",
                table: "tbl_review",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostId",
                table: "tbl_review");
        }
    }
}
