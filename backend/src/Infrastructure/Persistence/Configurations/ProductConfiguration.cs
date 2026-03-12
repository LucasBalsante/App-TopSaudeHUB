using backend.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.HasIndex(x => x.Sku)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("sku");

        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("price");

        builder.Property(x => x.StockQty)
            .IsRequired()
            .HasColumnName("stock_qty");

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.HasMany(x => x.OrderItems)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
