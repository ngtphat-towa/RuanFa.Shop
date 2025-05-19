using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Categorizations;

internal class CollectionConfiguration : IEntityTypeConfiguration<CatalogCollection>
{
    public void Configure(EntityTypeBuilder<CatalogCollection> builder)
    {
        // Table Name
        builder.ToTable(Schema.Collections);

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.ImageUrl)
            .HasMaxLength(500);

        builder.Property(t => t.Type)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.IsFeatured)
            .IsRequired();

        builder.Property(t => t.DisplayOrder)
            .IsRequired(false);

        // Relationships
        builder.HasMany(t => t.ProductCollections)
            .WithOne(m => m.Collection)
            .HasForeignKey(pc => pc.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.Slug).IsUnique();
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.IsFeatured);
    }
}
