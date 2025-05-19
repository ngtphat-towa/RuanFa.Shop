using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Products;

internal class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        // Table name
        builder.ToTable(Schema.ProductImages);

        // Primary key
        builder.HasKey(p => p.Id);

        // Relationships
        builder.HasOne(p => p.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Variant)
            .WithMany(v => v.VariantImages)
            .HasForeignKey(p => p.VariantId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties
        builder.Property(p => p.IsDefault)
            .IsRequired();

        // Owned value object: ImageData
        builder.OwnsOne(p => p.Image, image =>
        {
            image.Property(i => i.ImageType)
                .IsRequired();

            image.Property(i => i.Url)
                .IsRequired()
                .HasMaxLength(500);

            image.Property(i => i.Alt)
                .IsRequired()
                .HasMaxLength(125);

            image.Property(i => i.Width);
            image.Property(i => i.Height);
            image.Property(i => i.FileSizeBytes);
        });

        // Indexes
        builder.HasIndex(p => new { p.ProductId, p.VariantId });
    }
}
