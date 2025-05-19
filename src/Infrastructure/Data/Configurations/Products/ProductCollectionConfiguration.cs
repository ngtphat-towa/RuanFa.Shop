using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Products;

internal class ProductCollectionConfiguration : IEntityTypeConfiguration<ProductCollection>
{
    public void Configure(EntityTypeBuilder<ProductCollection> builder)
    {
        // Table Name
        builder.ToTable(Schema.ProductCollections);

        // Composite Primary Key
        builder.HasKey(t => new { t.ProductId, t.CollectionId });

        // Relationships
        builder.HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCollections)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Collection)
            .WithMany(c => c.ProductCollections)
            .HasForeignKey(pc => pc.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pc => pc.ProductId);
        builder.HasIndex(pc => pc.CollectionId);
    }
}
