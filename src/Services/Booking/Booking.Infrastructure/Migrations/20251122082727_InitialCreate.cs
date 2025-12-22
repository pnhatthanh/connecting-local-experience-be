using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExperienceId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Adults = table.Column<int>(type: "integer", nullable: false),
                    Children = table.Column<int>(type: "integer", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    PlatformFee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    HostAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    IsPayoutCreated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PayoutScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContactPhone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bookings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "booking_cancellations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CancelledBy = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CancellationFee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking_cancellations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_booking_cancellations_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "host_payouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "VND"),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BankName = table.Column<string>(type: "text", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "text", nullable: true),
                    BankAccountName = table.Column<string>(type: "text", nullable: true),
                    TransactionReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "VND"),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payments_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refunds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "VND"),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    VnpRefundRef = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VnpResponseCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refunds_bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_refunds_payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_booking_cancellations_BookingId",
                table: "booking_cancellations",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_booking_cancellations_CancelledBy",
                table: "booking_cancellations",
                column: "CancelledBy");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_BookingCode",
                table: "bookings",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_Date",
                table: "bookings",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_ExperienceId",
                table: "bookings",
                column: "ExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_HostId",
                table: "bookings",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_Status",
                table: "bookings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_UserId",
                table: "bookings",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_payments_BookingId",
                table: "payments",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_TransactionId",
                table: "payments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_BookingId",
                table: "refunds",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_PaymentId",
                table: "refunds",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_refunds_Status",
                table: "refunds",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booking_cancellations");

            migrationBuilder.DropTable(
                name: "host_payouts");

            migrationBuilder.DropTable(
                name: "refunds");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "bookings");
        }
    }
}
