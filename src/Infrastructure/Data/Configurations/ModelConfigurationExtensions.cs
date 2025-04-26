using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RuanFa.Shop.SharedKernel.Interfaces;

namespace Infrastructure.Data.Configurations
{
    public static class ModelConfigurationExtensions
    {
        public static void ConfigureTracking<T>(this EntityTypeBuilder<T> builder)
            where T : class, IActionTrackable
        {
            builder.Property(x => x.CreatedBy).HasMaxLength(256);
            builder.Property(x => x.UpdatedBy).HasMaxLength(256);
        }

        public static void ConfigureSoftDelete<T>(this EntityTypeBuilder<T> builder)
            where T : class, IDeletableEntity
        {
            var entityType = typeof(T);
            var persistable = typeof(IPersistable).IsAssignableFrom(entityType);

            if (!persistable ||
                (persistable && ((IPersistable)Activator.CreateInstance<T>()!).EnableSoftDelete))
            {
                builder.Property(x => x.IsDeleted);
                builder.Property(x => x.DeletedBy).HasMaxLength(256);
                builder.Property(x => x.DeletedAt);

                builder.HasQueryFilter(x => !x.IsDeleted);
            }
        }

        public static void ConfigureVersioning<T>(this EntityTypeBuilder<T> builder)
            where T : class, IVersionable
        {
            var entityType = typeof(T);
            var persistable = typeof(IPersistable).IsAssignableFrom(entityType);

            if (!persistable ||
                (persistable && ((IPersistable)Activator.CreateInstance<T>()!).EnableVersioning))
            {
                builder.Property(x => x.Version)
                      .IsConcurrencyToken()
                      .IsRequired();
            }
        }
    }

}
