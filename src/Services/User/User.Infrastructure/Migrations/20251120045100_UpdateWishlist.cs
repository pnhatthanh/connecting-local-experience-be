using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_user_favorite_experiences");

            migrationBuilder.CreateTable(
                name: "tbl_user_wishlists",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    experience_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_wishlists", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_user_wishlists_tbl_users_user_id",
                        column: x => x.user_id,
                        principalTable: "tbl_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_wishlist_experiences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    wishlist_id = table.Column<Guid>(type: "uuid", nullable: false),
                    experience_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_wishlist_experiences", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_wishlist_experiences_tbl_user_wishlists_wishlist_id",
                        column: x => x.wishlist_id,
                        principalTable: "tbl_user_wishlists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_wishlists_user_id",
                table: "tbl_user_wishlists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_wishlists_user_id_name",
                table: "tbl_user_wishlists",
                columns: new[] { "user_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_wishlist_experiences_experience_id",
                table: "tbl_wishlist_experiences",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_wishlist_experiences_wishlist_id",
                table: "tbl_wishlist_experiences",
                column: "wishlist_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_wishlist_experiences_wishlist_id_experience_id",
                table: "tbl_wishlist_experiences",
                columns: new[] { "wishlist_id", "experience_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_wishlist_experiences");

            migrationBuilder.DropTable(
                name: "tbl_user_wishlists");

            migrationBuilder.CreateTable(
                name: "tbl_user_favorite_experiences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    experience_id = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user_favorite_experiences", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_user_favorite_experiences_tbl_users_user_id",
                        column: x => x.user_id,
                        principalTable: "tbl_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_favorite_experiences_experience_id",
                table: "tbl_user_favorite_experiences",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_favorite_experiences_user_id_experience_id",
                table: "tbl_user_favorite_experiences",
                columns: new[] { "user_id", "experience_id" },
                unique: true);
        }
    }
}
