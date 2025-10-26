using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Experience.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "tbl_experience",
                columns: table => new
                {
                    id_experience = table.Column<Guid>(type: "uuid", nullable: false),
                    host_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<Point>(type: "geography(Point)", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    max_participants = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    activity_level = table.Column<string>(type: "text", nullable: false),
                    skill_level = table.Column<string>(type: "text", nullable: false),
                    min_age = table.Column<int>(type: "integer", nullable: false),
                    accessibility = table.Column<string>(type: "text", nullable: true),
                    amenities = table.Column<List<string>>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Draft"),
                    cancellation_policy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    meeting_point = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_experience", x => x.id_experience);
                });

            migrationBuilder.CreateTable(
                name: "tbl_experience_itinerary",
                columns: table => new
                {
                    id_itinerary = table.Column<Guid>(type: "uuid", nullable: false),
                    experience_id = table.Column<Guid>(type: "uuid", nullable: false),
                    step_number = table.Column<int>(type: "integer", nullable: false),
                    photo_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<Point>(type: "geography(Point)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_experience_itinerary", x => x.id_itinerary);
                    table.ForeignKey(
                        name: "FK_tbl_experience_itinerary_tbl_experience_experience_id",
                        column: x => x.experience_id,
                        principalTable: "tbl_experience",
                        principalColumn: "id_experience",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_experience_media",
                columns: table => new
                {
                    id_media = table.Column<Guid>(type: "uuid", nullable: false),
                    experience_id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_experience_media", x => x.id_media);
                    table.ForeignKey(
                        name: "FK_tbl_experience_media_tbl_experience_experience_id",
                        column: x => x.experience_id,
                        principalTable: "tbl_experience",
                        principalColumn: "id_experience",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_experience_schedule",
                columns: table => new
                {
                    id_schedule = table.Column<Guid>(type: "uuid", nullable: false),
                    experience_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_recurring = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    recurring_pattern = table.Column<string>(type: "jsonb", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Asia/Ho_Chi_Minh"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_experience_schedule", x => x.id_schedule);
                    table.ForeignKey(
                        name: "FK_tbl_experience_schedule_tbl_experience_experience_id",
                        column: x => x.experience_id,
                        principalTable: "tbl_experience",
                        principalColumn: "id_experience",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_experience_schedule_slot",
                columns: table => new
                {
                    id_slot = table.Column<Guid>(type: "uuid", nullable: false),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    total_slots = table.Column<int>(type: "integer", nullable: false),
                    available_slots = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Open"),
                    cancel_reason = table.Column<string>(type: "text", nullable: true),
                    cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_experience_schedule_slot", x => x.id_slot);
                    table.ForeignKey(
                        name: "FK_tbl_experience_schedule_slot_tbl_experience_schedule_schedu~",
                        column: x => x.schedule_id,
                        principalTable: "tbl_experience_schedule",
                        principalColumn: "id_schedule",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_category",
                table: "tbl_experience",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_host_id",
                table: "tbl_experience",
                column: "host_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_status",
                table: "tbl_experience",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_itinerary_experience_id",
                table: "tbl_experience_itinerary",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_media_experience_id",
                table: "tbl_experience_media",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_schedule_experience_id",
                table: "tbl_experience_schedule",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_schedule_slot_date",
                table: "tbl_experience_schedule_slot",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_schedule_slot_schedule_id",
                table: "tbl_experience_schedule_slot",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_schedule_slot_status",
                table: "tbl_experience_schedule_slot",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_experience_itinerary");

            migrationBuilder.DropTable(
                name: "tbl_experience_media");

            migrationBuilder.DropTable(
                name: "tbl_experience_schedule_slot");

            migrationBuilder.DropTable(
                name: "tbl_experience_schedule");

            migrationBuilder.DropTable(
                name: "tbl_experience");
        }
    }
}
