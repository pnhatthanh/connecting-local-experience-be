using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace User.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "text", nullable: true),
                    avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    role = table.Column<string>(type: "text", nullable: false, defaultValue: "user"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_host_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bio = table.Column<string>(type: "TEXT", nullable: true),
                    spoken_languages = table.Column<string[]>(type: "jsonb", nullable: true),
                    location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    hosting_since = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    verify_status = table.Column<string>(type: "text", nullable: false, defaultValue: "pending"),
                    document_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    verify_reason = table.Column<string>(type: "TEXT", nullable: true),
                    verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    response_time = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    total_experiences = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_bookings = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_reviews = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    rating_avg = table.Column<decimal>(type: "numeric(3,2)", nullable: true),
                    work = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    education = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    fun_fact = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    topics_of_interest = table.Column<string[]>(type: "jsonb", nullable: false),
                    facebook_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    instagram_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    linkedin_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_host_profiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_tbl_host_profiles_tbl_users_user_id",
                        column: x => x.user_id,
                        principalTable: "tbl_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user_favorite_experiences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    experience_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
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
                name: "IX_tbl_host_profiles_user_id",
                table: "tbl_host_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_host_profiles_verify_status",
                table: "tbl_host_profiles",
                column: "verify_status");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_favorite_experiences_experience_id",
                table: "tbl_user_favorite_experiences",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_favorite_experiences_user_id_experience_id",
                table: "tbl_user_favorite_experiences",
                columns: new[] { "user_id", "experience_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_users_email",
                table: "tbl_users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_host_profiles");

            migrationBuilder.DropTable(
                name: "tbl_user_favorite_experiences");

            migrationBuilder.DropTable(
                name: "tbl_users");
        }
    }
}
