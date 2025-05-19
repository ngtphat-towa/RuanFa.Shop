using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Products;

internal class VariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        // Table name
        builder.ToTable(TableName.Variants);

        // Primary key
        builder.HasKey(v => v.Id);

        // Properties
        builder.Property(v => v.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.PriceOffset)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(v => v.StockQuantity)
            .IsRequired();

        builder.Property(v => v.LowStockThreshold)
            .IsRequired();

        builder.Property(v => v.StockStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(v => v.IsActive)
            .IsRequired();

        builder.Property(v => v.IsDefault)
            .IsRequired();

        // Relationships
        builder.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.StockMovements)
            .WithOne(v => v.Variant)
            .HasForeignKey(sm => sm.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.VariantAttributeOptions)
            .WithOne(opt => opt.Variant)
            .HasForeignKey(opt => opt.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.VariantImages)
            .WithOne(img => img.Variant)
            .HasForeignKey(img => img.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(v => v.Sku).IsUnique();
    }
}
