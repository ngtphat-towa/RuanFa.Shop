using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Products;

public class VariantAttributeValueConfiguration : IEntityTypeConfiguration<VariantAttributeValue>
{
    public void Configure(EntityTypeBuilder<VariantAttributeValue> builder)
    {
        // Table and Key
        builder.ToTable(Schema.VariantAttributeValues);
        builder.HasKey(vav => vav.Id);

        // Properties
        builder.Property(vav => vav.Id)
            .ValueGeneratedOnAdd();

        builder.Property(vav => vav.VariantId)
            .IsRequired();

        builder.Property(vav => vav.AttributeId)
            .IsRequired();

        builder.Property(vav => vav.AttributeOptionId)
            .IsRequired(false);

        builder.Property(vav => vav.Value)
            .HasMaxLength(500)
            .IsRequired(false);

        // Relationships
        builder.HasOne(vav => vav.Variant)
            .WithMany(pv => pv.VariantAttributeValues)
            .HasForeignKey(vav => vav.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vav => vav.Attribute)
            .WithMany(ca => ca.VariantAttributeValues)
            .HasForeignKey(vav => vav.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vav => vav.AttributeOption)
            .WithMany(ao => ao.VariantAttributeValues)
            .HasForeignKey(vav => vav.AttributeOptionId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(vav => new { vav.VariantId, vav.AttributeId })
            .IsUnique();

        builder.HasIndex(vav => vav.AttributeOptionId);
    }
}
