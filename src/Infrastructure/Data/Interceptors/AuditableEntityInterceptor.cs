using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RuanFa.Shop.Application.Common.Security.Authentications;
using RuanFa.Shop.Application.Common.Services;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Infrastructure.Data.Interceptors;

internal sealed class AuditableEntityInterceptor(IUserContext userContext, IDateTimeProvider dateTime)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified) && !entry.HasChangedOwnedEntities())
            {
                continue;
            }

            DateTime utcNow = dateTime.UtcNow;
            // Determine the current user or default to "System"
            string? userId = userContext.UserId;
            string userString = userId != null ? userId.ToString() : "System";

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = userString;
                entry.Entity.CreatedAt = utcNow;
            }
            entry.Entity.UpdatedBy = userString;
            entry.Entity.UpdatedAt = utcNow;
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
