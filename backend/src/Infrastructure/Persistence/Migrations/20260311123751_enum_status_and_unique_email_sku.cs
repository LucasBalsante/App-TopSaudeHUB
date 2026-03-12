using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.src.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class enum_status_and_unique_email_sku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_products_Sku",
                table: "products",
                column: "Sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_customers_Email",
                table: "customers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_products_Sku",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_customers_Email",
                table: "customers");
        }
    }
}
