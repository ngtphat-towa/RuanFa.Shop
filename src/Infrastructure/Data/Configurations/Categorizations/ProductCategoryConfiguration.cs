using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Categorizations;

internal class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        // Table Name
        builder.ToTable(TableName.ProductCategories);

        // Composite Primary Key
        builder.HasKey(t => new { t.ProductId, t.CategoryId });

        // Relationships
        builder.HasOne(pc => pc.Product)
            .WithMany()
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(pc => pc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pc => pc.ProductId);
        builder.HasIndex(pc => pc.CategoryId);
    }
}
