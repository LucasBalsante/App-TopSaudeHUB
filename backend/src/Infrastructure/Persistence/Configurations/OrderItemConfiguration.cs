using backend.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.OrderId)
            .IsRequired()
            .HasColumnName("order_id");

        builder.Property(x => x.ProductId)
            .IsRequired()
            .HasColumnName("product_id");

        builder.Property(x => x.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("unit_price");

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasColumnName("quantity");

        builder.Property(x => x.LineTotal)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("line_total");
    }
}
