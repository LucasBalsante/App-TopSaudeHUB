using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.src.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class nomenclatura_colunas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_OrderId",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_products_ProductId",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_customers_CustomerId",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "products",
                newName: "sku");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "products",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "products",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StockQty",
                table: "products",
                newName: "stock_qty");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "products",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "products",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_products_Sku",
                table: "products",
                newName: "IX_products_sku");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "orders",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "orders",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "orders",
                newName: "total_amount");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "orders",
                newName: "customer_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "orders",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_orders_CustomerId",
                table: "orders",
                newName: "IX_orders_customer_id");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "order_items",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "order_items",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "order_items",
                newName: "unit_price");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "order_items",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "order_items",
                newName: "order_id");

            migrationBuilder.RenameColumn(
                name: "LineTotal",
                table: "order_items",
                newName: "line_total");

            migrationBuilder.RenameIndex(
                name: "IX_order_items_ProductId",
                table: "order_items",
                newName: "IX_order_items_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_items_OrderId",
                table: "order_items",
                newName: "IX_order_items_order_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "customers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "customers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Document",
                table: "customers",
                newName: "document");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "customers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "customers",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_customers_Email",
                table: "customers",
                newName: "IX_customers_email");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_orders_order_id",
                table: "order_items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_products_product_id",
                table: "order_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_customers_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_order_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_products_product_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_customers_customer_id",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "sku",
                table: "products",
                newName: "Sku");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "stock_qty",
                table: "products",
                newName: "StockQty");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "products",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "products",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_products_sku",
                table: "products",
                newName: "IX_products_Sku");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "orders",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "total_amount",
                table: "orders",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "orders",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "orders",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                newName: "IX_orders_CustomerId");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "order_items",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "order_items",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "unit_price",
                table: "order_items",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "order_items",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "order_items",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "line_total",
                table: "order_items",
                newName: "LineTotal");

            migrationBuilder.RenameIndex(
                name: "IX_order_items_product_id",
                table: "order_items",
                newName: "IX_order_items_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_order_items_order_id",
                table: "order_items",
                newName: "IX_order_items_OrderId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "customers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "customers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "document",
                table: "customers",
                newName: "Document");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "customers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "customers",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_customers_email",
                table: "customers",
                newName: "IX_customers_Email");

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_orders_OrderId",
                table: "order_items",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_products_ProductId",
                table: "order_items",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_customers_CustomerId",
                table: "orders",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
