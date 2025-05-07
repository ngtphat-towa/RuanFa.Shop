using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Catalogs;

internal class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        // Table name
        builder.ToTable(TableName.ProductImages);

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
                .HasColumnName("ImageType")
                .IsRequired();

            image.Property(i => i.Url)
                .HasColumnName("ImageUrl")
                .IsRequired()
                .HasMaxLength(500);

            image.Property(i => i.Alt)
                .HasColumnName("AltText")
                .IsRequired()
                .HasMaxLength(125);

            image.Property(i => i.Width)
                .HasColumnName("ImageWidth");

            image.Property(i => i.Height)
                .HasColumnName("ImageHeight");

            image.Property(i => i.FileSizeBytes)
                .HasColumnName("ImageFileSizeBytes");
        });

        // Indexes
        builder.HasIndex(p => new { p.ProductId, p.VariantId });
    }
}
