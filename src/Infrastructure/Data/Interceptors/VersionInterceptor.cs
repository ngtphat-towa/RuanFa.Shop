using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.SharedKernel.Interfaces;

namespace Infrastructure.Data.Interceptors;
public sealed class VersionInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        HandleVersioning(eventData.Context);
        return result;
    }

    private static void HandleVersioning(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker
            .Entries<IVersionable>()
            .Where(e => e.State == EntityState.Modified &&
                       (e.Entity is not IPersistable persistable || persistable.EnableVersioning));

        foreach (var entry in entries)
        {
            var property = entry.Property(nameof(IVersionable.Version));
            var currentVersion = (int?)property.CurrentValue ?? 0; // Handle null case
            property.CurrentValue = currentVersion + 1;
        }
    }
}
