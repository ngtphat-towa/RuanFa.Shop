using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Attributes;

internal sealed class AttributeGroupAttributeConfiguration
    : IEntityTypeConfiguration<AttributeGroupAttribute>
{
    public void Configure(EntityTypeBuilder<AttributeGroupAttribute> builder)
    {
        // Table Name
        builder.ToTable(TableName.AttributeGroupAttributes);

        // Keys
        builder.HasKey(x => new { x.AttributeGroupId, x.AttributeId});

        builder
            .HasOne(x => x.AttributeGroup)
            .WithMany(x => x.AttributeGroupAttributes)
            .HasForeignKey(x => x.AttributeGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Attribute)
            .WithMany(x => x.AttributeGroupAttributes)
            .HasForeignKey(x => x.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
