using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Attributes;

public class AttributeGroupAttributeConfiguration : IEntityTypeConfiguration<AttributeGroupAttribute>
{
    public void Configure(EntityTypeBuilder<AttributeGroupAttribute> builder)
    {
        // Table and Key
        builder.ToTable(Schema.AttributeGroupAttributes);
        builder.HasKey(aga => new {aga.AttributeGroupId, aga.AttributeId});

        // Properties
        builder.Property(aga => aga.AttributeGroupId)
            .IsRequired();

        builder.Property(aga => aga.AttributeId)
            .IsRequired();

        // Relationships
        builder.HasOne(aga => aga.AttributeGroup)
            .WithMany()
            .HasForeignKey(aga => aga.AttributeGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(aga => aga.Attribute)
            .WithMany(ca => ca.AttributeGroupAttributes)
            .HasForeignKey(aga => aga.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(aga => new { aga.AttributeGroupId, aga.AttributeId })
            .IsUnique();
    }
}
