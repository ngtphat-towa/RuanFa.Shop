using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Attributes;

public class AttributeConfiguration : IEntityTypeConfiguration<CatalogAttribute>
{
    public void Configure(EntityTypeBuilder<CatalogAttribute> builder)
    {
        // Table and Key
        builder.ToTable(Schema.Attributes);
        builder.HasKey(ca => ca.Id);

        // Properties
        builder.Property(ca => ca.Id)
            .ValueGeneratedOnAdd();

        builder.Property(ca => ca.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ca => ca.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ca => ca.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<AttributeType>(v))
            .HasDefaultValue(AttributeType.None);

        builder.Property(ca => ca.IsRequired)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ca => ca.DisplayOnFrontend)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ca => ca.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ca => ca.IsFilterable)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasMany(ca => ca.AttributeOptions)
            .WithOne(ao => ao.Attribute)
            .HasForeignKey(ao => ao.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ca => ca.AttributeGroupAttributes)
            .WithOne(aga => aga.Attribute)
            .HasForeignKey(aga => aga.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(ca => ca.VariantAttributeValues)
            .WithOne(vav => vav.Attribute)
            .HasForeignKey(vav => vav.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ca => ca.Code)
            .IsUnique();
        builder.HasIndex(ca => ca.Name);
    }
}
