using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.src.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class correcao_completed_at : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "idempotency_requests",
                newName: "completed_at");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "idempotency_requests",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "completed_at",
                table: "idempotency_requests",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "completed_at",
                table: "idempotency_requests",
                newName: "CompletedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "idempotency_requests",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "idempotency_requests",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }
    }
}
