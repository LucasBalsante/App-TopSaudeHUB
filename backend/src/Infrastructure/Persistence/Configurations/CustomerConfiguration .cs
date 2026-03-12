using backend.src.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(60)
            .HasColumnName("name");

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("email");

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.Document)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("document");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.HasMany(x => x.Orders)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}