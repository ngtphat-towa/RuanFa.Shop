using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Products;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        // Table and Key
        builder.ToTable(Schema.Variants);
        builder.HasKey(pv => pv.Id);

        // Properties
        builder.Property(pv => pv.Id)
            .ValueGeneratedOnAdd();

        builder.Property(pv => pv.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pv => pv.PriceOffset)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(pv => pv.StockQuantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pv => pv.LowStockThreshold)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(pv => pv.StockStatus)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<StockStatus>(v))
            .HasDefaultValue(StockStatus.InStock);

        builder.Property(pv => pv.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(pv => pv.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(pv => pv.ProductId)
            .IsRequired();

        // Relationships
        builder.HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pv => pv.VariantAttributeValues)
            .WithOne(vav => vav.Variant)
            .HasForeignKey(vav => vav.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pv => pv.StockMovements)
            .WithOne(sm => sm.Variant)
            .HasForeignKey(sm => sm.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pv => pv.VariantImages)
            .WithOne(pi => pi.Variant)
            .HasForeignKey(pi => pi.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pv => pv.Sku)
            .IsUnique();

        builder.HasIndex(pv => pv.ProductId);
    }
}
