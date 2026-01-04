using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntreLaunch.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseIdToConsultations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_consultation_tickets_consultations_consultation_id",
                table: "consultation_tickets");

            migrationBuilder.DropIndex(
                name: "ix_purchases_payment_id",
                table: "purchases");

            migrationBuilder.AddColumn<int>(
                name: "purchase_id",
                table: "consultations",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "date_time_slot",
                table: "consultation_times",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_recurring_daily",
                table: "consultation_times",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "sender_id",
                table: "consultation_ticket_messages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "send_time",
                table: "consultation_ticket_messages",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_purchases_payment_id",
                table: "purchases",
                column: "payment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_consultations_purchase_id",
                table: "consultations",
                column: "purchase_id");

            migrationBuilder.CreateIndex(
                name: "ix_consultations_status",
                table: "consultations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_consultations_type",
                table: "consultations",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_consultation_times_date_time_slot",
                table: "consultation_times",
                column: "date_time_slot");

            migrationBuilder.AddForeignKey(
                name: "fk_consultation_tickets_consultations_consultation_id",
                table: "consultation_tickets",
                column: "consultation_id",
                principalTable: "consultations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_consultations_purchases_purchase_id",
                table: "consultations",
                column: "purchase_id",
                principalTable: "purchases",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_consultation_tickets_consultations_consultation_id",
                table: "consultation_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_consultations_purchases_purchase_id",
                table: "consultations");

            migrationBuilder.DropIndex(
                name: "ix_purchases_payment_id",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "ix_consultations_purchase_id",
                table: "consultations");

            migrationBuilder.DropIndex(
                name: "ix_consultations_status",
                table: "consultations");

            migrationBuilder.DropIndex(
                name: "ix_consultations_type",
                table: "consultations");

            migrationBuilder.DropIndex(
                name: "ix_consultation_times_date_time_slot",
                table: "consultation_times");

            migrationBuilder.DropColumn(
                name: "purchase_id",
                table: "consultations");

            migrationBuilder.DropColumn(
                name: "is_recurring_daily",
                table: "consultation_times");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "date_time_slot",
                table: "consultation_times",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "sender_id",
                table: "consultation_ticket_messages",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "send_time",
                table: "consultation_ticket_messages",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateIndex(
                name: "ix_purchases_payment_id",
                table: "purchases",
                column: "payment_id");

            migrationBuilder.AddForeignKey(
                name: "fk_consultation_tickets_consultations_consultation_id",
                table: "consultation_tickets",
                column: "consultation_id",
                principalTable: "consultations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
