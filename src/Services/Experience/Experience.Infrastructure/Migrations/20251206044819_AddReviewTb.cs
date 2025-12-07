using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Experience.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalReviews",
                table: "tbl_experience",
                newName: "total_reviews");

            migrationBuilder.RenameColumn(
                name: "AverageRating",
                table: "tbl_experience",
                newName: "average_rating");

            migrationBuilder.AlterColumn<int>(
                name: "total_reviews",
                table: "tbl_experience",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "average_rating",
                table: "tbl_experience",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateTable(
                name: "tbl_review",
                columns: table => new
                {
                    id_review = table.Column<Guid>(type: "uuid", nullable: false),
                    experience_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_hidden = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_review", x => x.id_review);
                    table.ForeignKey(
                        name: "FK_tbl_review_tbl_experience_experience_id",
                        column: x => x.experience_id,
                        principalTable: "tbl_experience",
                        principalColumn: "id_experience",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_review_experience_id",
                table: "tbl_review",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_review_user_id",
                table: "tbl_review",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_review_user_id_experience_id",
                table: "tbl_review",
                columns: new[] { "user_id", "experience_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_review");

            migrationBuilder.RenameColumn(
                name: "total_reviews",
                table: "tbl_experience",
                newName: "TotalReviews");

            migrationBuilder.RenameColumn(
                name: "average_rating",
                table: "tbl_experience",
                newName: "AverageRating");

            migrationBuilder.AlterColumn<int>(
                name: "TotalReviews",
                table: "tbl_experience",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "AverageRating",
                table: "tbl_experience",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 0.0);
        }
    }
}
