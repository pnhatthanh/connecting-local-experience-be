using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "host_payouts");

            migrationBuilder.DropColumn(
                name: "IsPayoutCreated",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "PayoutScheduledDate",
                table: "bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPayoutCreated",
                table: "bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PayoutScheduledDate",
                table: "bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "host_payouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    BankAccountName = table.Column<string>(type: "text", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "text", nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "VND"),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TransactionReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_host_payouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_host_payouts_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_host_payouts_BookingId",
                table: "host_payouts",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_host_payouts_HostId",
                table: "host_payouts",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_host_payouts_ScheduledDate",
                table: "host_payouts",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_host_payouts_Status",
                table: "host_payouts",
                column: "Status");
        }
    }
}
