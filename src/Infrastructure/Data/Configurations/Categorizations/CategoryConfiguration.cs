using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Categorizations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table Name
        builder.ToTable(Schema.Categories);

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.UrlKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.IncludeInNav)
            .IsRequired();

        builder.Property(t => t.Position)
            .IsRequired(false);

        builder.Property(t => t.ShowProducts)
            .IsRequired();

        builder.OwnsOne(t => t.ShortDescription);

        builder.OwnsOne(t => t.Description);

        builder.OwnsOne(t => t.SeoMeta);

        // Value Object: CategoryImage
        builder.OwnsOne(t => t.Image, imageBuilder =>
        {
            imageBuilder.Property(i => i.Url)
                .IsRequired(false)
                .HasMaxLength(500);

            imageBuilder.Property(i => i.Alt)
                .IsRequired(false)
                .HasMaxLength(100);
        });

        // Relationships
        builder.HasOne(t => t.Parent)
            .WithMany(t => t.Children)
            .HasForeignKey(t => t.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Products)
            .WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.UrlKey).IsUnique();
        builder.HasIndex(t => t.ParentId);
    }
}
