using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refunds_payments_PaymentId",
                table: "refunds");

            migrationBuilder.DropIndex(
                name: "IX_refunds_BookingId",
                table: "refunds");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "HostAmount",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "PlatformFee",
                table: "bookings");

            migrationBuilder.AddColumn<string>(
                name: "ExperienceTitle",
                table: "bookings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "bookings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_BookingId",
                table: "refunds",
                column: "BookingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_refunds_BookingId",
                table: "refunds");

            migrationBuilder.DropColumn(
                name: "ExperienceTitle",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "bookings");

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HostAmount",
                table: "bookings",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformFee",
                table: "bookings",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_refunds_BookingId",
                table: "refunds",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_refunds_payments_PaymentId",
                table: "refunds",
                column: "PaymentId",
                principalTable: "payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
