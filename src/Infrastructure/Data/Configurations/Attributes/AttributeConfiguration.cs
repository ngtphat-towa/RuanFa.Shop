using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Attributes;
using RuanFa.Shop.Domain.Attributes.Enums;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Attributes;

internal class AttributeConfiguration :
    IEntityTypeConfiguration<CatalogAttribute>
{
    public void Configure(EntityTypeBuilder<CatalogAttribute> builder)
    {
        // Table Name
        builder.ToTable(TableName.Attributes);

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Type)
            .HasConversion(
                v => (int)v,
                v => (AttributeType)v)
            .IsRequired();

        builder.Property(t => t.IsRequired)
            .IsRequired();

        builder.Property(t => t.IsFilterable)
            .IsRequired();

        builder.Property(t => t.DisplayOnFrontend)
            .IsRequired();

        // Relationships
        builder.HasMany(t => t.AttributeOptions)
            .WithOne(t => t.Attribute)
            .HasForeignKey(t => t.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.AttributeGroupAttributes)
            .WithOne(t => t.Attribute)
            .HasForeignKey(t => t.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.Name).IsUnique();
        builder.HasIndex(t => t.Code).IsUnique();
    }
}
