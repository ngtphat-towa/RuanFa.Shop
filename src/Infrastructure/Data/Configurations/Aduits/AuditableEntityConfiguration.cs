using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Aduits;

internal sealed class AuditableEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IAuditable, IHasDomainEvent
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(200);

        builder.Property(e => e.CreatedAt)
            .HasMaxLength(200);

        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.CreatedBy);
        builder.HasIndex(e => e.UpdatedBy);

        // Ignore Domain Events
        builder.Ignore(ca => ca.DomainEvents);
    }
}
