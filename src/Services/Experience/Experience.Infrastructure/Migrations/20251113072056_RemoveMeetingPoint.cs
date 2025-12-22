using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Experience.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMeetingPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "meeting_location",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "meeting_point",
                table: "tbl_experience");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "meeting_location",
                table: "tbl_experience",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Point>(
                name: "meeting_point",
                table: "tbl_experience",
                type: "geography(Point)",
                nullable: false);
        }
    }
}
