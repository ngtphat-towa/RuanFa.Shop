using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.Infrastructure.Accounts.Entities;
using RuanFa.Shop.Infrastructure.Data.Configurations.Extensions;
using RuanFa.Shop.Infrastructure.Data.Constants;

namespace RuanFa.Shop.Infrastructure.Data.Configurations.Identities;

internal class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable(TableName.UserProfile);
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.UserId).IsUnique();
        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.FullName).IsRequired();
        builder.Property(e => e.Gender).IsRequired();
        builder.Property(e => e.LoyaltyPoints).IsRequired();
        builder.Property(e => e.MarketingConsent).IsRequired();

        // Value converter for Addresses (List<UserAddress>)
        builder.Property(up => up.Addresses)
            .HasValueJsonConverter()
            .HasColumnType("TEXT");

        // Value converter for Preferences (FashionPreferences)
        builder.Property(up => up.Preferences)
             .HasValueJsonConverter()
             .HasColumnType("TEXT");

        // Value converter for Wishlist (List<string>)
        builder.Property(e => e.Wishlist)
            .HasValueJsonConverter()
            .HasColumnType("TEXT");

        // Configure one-to-many with Order (optional relationship)
        builder.HasMany(e => e.Orders)
            .WithOne(e => e.Profile)
            .HasForeignKey(e => e.Id)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<ApplicationUser>()
            .WithOne(u => u.Profile)
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
