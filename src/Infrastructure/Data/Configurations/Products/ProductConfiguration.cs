using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.Infrastructure.Data.Constants;
using RuanFa.Shop.Infrastructure.Data.Converters;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Products;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table and Key
        builder.ToTable(Schema.Products);
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.BasePrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Weight)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.TaxClass)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<TaxClass>(v))
            .HasDefaultValue(TaxClass.None);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ProductStatus>(v))
            .HasDefaultValue(ProductStatus.Draft);

        builder.Property(p => p.Descriptions)
           .HasValueJsonConverter()
           .HasDefaultValue(new List<DescriptionData>())
           .HasColumnType("TEXT");

        builder.Property(p => p.GroupId)
            .IsRequired();

        // Relationships
        builder.HasOne(p => p.Group)
            .WithMany()
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany(pc => pc.Products)
            .HasForeignKey(pc => pc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductCollections)
            .WithOne(pc => pc.Product)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Images)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.Sku)
            .IsUnique();
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.GroupId);

        // Ignore Domain Events
        builder.Ignore(p => p.DomainEvents);
    }
}
