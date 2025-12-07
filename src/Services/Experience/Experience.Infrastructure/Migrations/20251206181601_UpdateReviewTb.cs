using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Experience.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviewTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_review_user_id",
                table: "tbl_review");

            migrationBuilder.DropIndex(
                name: "IX_tbl_review_user_id_experience_id",
                table: "tbl_review");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
