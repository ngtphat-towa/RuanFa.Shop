using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Products;

internal class VariantAttributeOptionConfiguration : IEntityTypeConfiguration<VariantAttributeOption>
{
    public void Configure(EntityTypeBuilder<VariantAttributeOption> builder)
    {
        // Table name
        builder.ToTable(TableName.VariantAttributeOptions);

        // Composite primary key
        builder.HasKey(v => new { v.VariantId, v.AttributeOptionId });

        // Relationships
        builder.HasOne(v => v.Variant)
            .WithMany(v => v.VariantAttributeOptions)
            .HasForeignKey(v => v.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.AttributeOption)
            .WithMany(v=>v.VariantAttributeOptions)
            .HasForeignKey(v => v.AttributeOptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
