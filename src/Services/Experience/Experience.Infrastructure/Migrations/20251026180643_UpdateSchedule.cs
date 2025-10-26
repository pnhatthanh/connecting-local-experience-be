using System;
using System.Collections.Generic;
using Experience.Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Experience.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_experience_schedule_experience_id",
                table: "tbl_experience_schedule");

            migrationBuilder.DropIndex(
                name: "IX_tbl_experience_category",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "is_recurring",
                table: "tbl_experience_schedule");

            migrationBuilder.DropColumn(
                name: "recurring_pattern",
                table: "tbl_experience_schedule");

            migrationBuilder.DropColumn(
                name: "timezone",
                table: "tbl_experience_schedule");

            migrationBuilder.DropColumn(
                name: "location",
                table: "tbl_experience_itinerary");

            migrationBuilder.DropColumn(
                name: "amenities",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "category",
                table: "tbl_experience");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "tbl_experience",
                newName: "child_price");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "tbl_experience_schedule",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<List<DayOfWeek>>(
                name: "days_of_week",
                table: "tbl_experience_schedule",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "recurrence_type",
                table: "tbl_experience_schedule",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<List<ScheduleTimeSlot>>(
                name: "time_slots",
                table: "tbl_experience_schedule",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "url",
                table: "tbl_experience_media",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "photo_url",
                table: "tbl_experience_itinerary",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Point>(
                name: "meeting_point",
                table: "tbl_experience",
                type: "geography(Point)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "cancellation_policy",
                table: "tbl_experience",
                type: "text",
                nullable: false,
                defaultValue: "AlwaysFreeCancellation",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "tbl_experience",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "adult_price",
                table: "tbl_experience",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "tbl_experience",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "tbl_experience",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "country",
                table: "tbl_experience",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "district",
                table: "tbl_experience",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "meeting_location",
                table: "tbl_experience",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "tbl_experience_category",
                columns: table => new
                {
                    id_category = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_experience_category", x => x.id_category);
                });

            migrationBuilder.InsertData(
                table: "tbl_experience_category",
                columns: new[] { "id_category", "created_at", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("3182f098-df79-4e18-b765-4c98bc13d084"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Entertainment", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("34929d1f-871a-4a9c-b584-bc03be1a0e87"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nature", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("48791c6f-5097-4db8-8012-0f71ce68b12e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sports", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6e8d931e-33d4-48b4-8865-578caccbb650"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nightlife", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8f5a637f-cda5-49fd-af07-6b61d4a78221"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Photography", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a6250fbd-486d-493e-9926-33968d5e20c3"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wellness", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b9338d5e-2d96-45d4-b5d1-a54f2bd18c33"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cooking", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ca2fa212-856c-4328-b902-15498ad2d63f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arts", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cb24e077-23cc-495f-bc7a-eebc7fe04c12"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Music", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("db745160-9e03-4321-ab75-5a093c09f661"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Animal", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dbc7a805-460a-4fc7-ba95-bef4f973ff97"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Social Impact", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f85a6b45-ca43-4c94-9b83-8ffb84f93b9d"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "History", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("fbc7eb03-df32-4ad8-8047-197c2c4db5c7"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Food Drink", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_schedule_experience_id",
                table: "tbl_experience_schedule",
                column: "experience_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_category_id",
                table: "tbl_experience",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_category_name",
                table: "tbl_experience_category",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_experience_tbl_experience_category_category_id",
                table: "tbl_experience",
                column: "category_id",
                principalTable: "tbl_experience_category",
                principalColumn: "id_category",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_experience_tbl_experience_category_category_id",
                table: "tbl_experience");

            migrationBuilder.DropTable(
                name: "tbl_experience_category");

            migrationBuilder.DropIndex(
                name: "IX_tbl_experience_schedule_experience_id",
                table: "tbl_experience_schedule");

            migrationBuilder.DropIndex(
                name: "IX_tbl_experience_category_id",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "days_of_week",
                table: "tbl_experience_schedule");

            migrationBuilder.DropColumn(
                name: "recurrence_type",
                table: "tbl_experience_schedule");

            migrationBuilder.DropColumn(
                name: "time_slots",
                table: "tbl_experience_schedule");

            migrationBuilder.DropColumn(
                name: "address",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "adult_price",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "city",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "country",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "district",
                table: "tbl_experience");

            migrationBuilder.DropColumn(
                name: "meeting_location",
                table: "tbl_experience");

            migrationBuilder.RenameColumn(
                name: "child_price",
                table: "tbl_experience",
                newName: "price");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "tbl_experience_schedule",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "is_recurring",
                table: "tbl_experience_schedule",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "recurring_pattern",
                table: "tbl_experience_schedule",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "timezone",
                table: "tbl_experience_schedule",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Asia/Ho_Chi_Minh");

            migrationBuilder.AlterColumn<string>(
                name: "url",
                table: "tbl_experience_media",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "photo_url",
                table: "tbl_experience_itinerary",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Point>(
                name: "location",
                table: "tbl_experience_itinerary",
                type: "geography(Point)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "meeting_point",
                table: "tbl_experience",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography(Point)");

            migrationBuilder.AlterColumn<string>(
                name: "cancellation_policy",
                table: "tbl_experience",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "AlwaysFreeCancellation");

            migrationBuilder.AddColumn<List<string>>(
                name: "amenities",
                table: "tbl_experience",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "tbl_experience",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_schedule_experience_id",
                table: "tbl_experience_schedule",
                column: "experience_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_experience_category",
                table: "tbl_experience",
                column: "category");
        }
    }
}
