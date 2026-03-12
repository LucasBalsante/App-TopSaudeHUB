using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.src.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class tabela_controle_idempotency_requests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "idempotency_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    request_path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    request_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    status = table.Column<int>(type: "integer", maxLength: 30, nullable: false),
                    ResponseCode = table.Column<int>(type: "integer", nullable: false),
                    response_payload = table.Column<string>(type: "jsonb", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idempotency_requests", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_idempotency_requests_key_request_path",
                table: "idempotency_requests",
                columns: new[] { "key", "request_path" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "idempotency_requests");
        }
    }
}
