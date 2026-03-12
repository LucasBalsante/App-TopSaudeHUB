using backend.src.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.src.Infrastructure.Persistence.Configurations
{
    public class IdempotencyRequestConfiguration : IEntityTypeConfiguration<IdempotencyRequest>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRequest> builder)
        {
            builder.ToTable("idempotency_requests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.Key)
                .HasMaxLength(200)
                .HasColumnName("key")
                .IsRequired();

            builder.Property(x => x.RequestPath)
                .HasMaxLength(200)
                .HasColumnName("request_path")
                .IsRequired();

            builder.Property(x => x.RequestHash)
                .HasMaxLength(128)
                .HasColumnName("request_hash")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasMaxLength(30)
                .HasColumnName("status")
                .IsRequired();

            builder.Property(x => x.ResponsePayload)
                .HasColumnName("response_payload")
                .HasColumnType("jsonb");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp without time zone")
                .IsRequired();

            builder.Property(x => x.CompletedAt)
                .HasColumnName("completed_at")
                .HasColumnType("timestamp without time zone");

            builder.HasIndex(x => new { x.Key, x.RequestPath })
                .IsUnique();
        }
    }
}
