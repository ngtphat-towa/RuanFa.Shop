using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Attributes;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Attributes;

internal sealed class AttributeGroupConfiguration : IEntityTypeConfiguration<AttributeGroup>
{
    public void Configure(EntityTypeBuilder<AttributeGroup> builder)
    {
        // Table name
        builder.ToTable(TableName.AttributeGroups);

        // Primary key
        builder.HasKey(ac => ac.Id);

        // Properties
        builder.Property(ac => ac.Name)
            .IsRequired() 
            .HasMaxLength(255); 

        // Relationships
        builder.HasMany(ac => ac.Products)
            .WithOne(p => p.Group)
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ac => ac.AttributeGroupAttributes)
            .WithOne(agc => agc.AttributeGroup)
            .HasForeignKey(agc => agc.AttributeGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ag => ag.Name)
        .IsUnique();
    }
}
