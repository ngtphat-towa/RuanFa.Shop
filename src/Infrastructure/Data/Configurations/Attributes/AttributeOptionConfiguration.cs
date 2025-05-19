using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Attributes;

internal sealed class AttributeOptionConfiguration : IEntityTypeConfiguration<AttributeOption>
{
    public void Configure(EntityTypeBuilder<AttributeOption> builder)
    {
        // Table Name
        builder.ToTable(TableName.AttributeOptions);

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.OptionText)
            .IsRequired()
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(t => t.Attribute)
            .WithMany(t => t.AttributeOptions)
            .HasForeignKey(t => t.AttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(av => new { av.Code, av.OptionText })
            .IsUnique();

    }
}
