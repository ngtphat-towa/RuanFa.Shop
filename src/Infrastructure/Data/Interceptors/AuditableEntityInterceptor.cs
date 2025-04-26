using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RuanFa.Shop.SharedKernel.Interfaces;
using RuanFa.Shop.SharedKernel.Services;

namespace Infrastructure.Data.Interceptors;

internal sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _userContext;

    public AuditableEntityInterceptor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ProcessAuditableEntities(eventData.Context);
        return result;
    }

    private void ProcessAuditableEntities(DbContext? context)
    {
        if (!ShouldProcess(context)) return;

        var username = _userContext.Username!;
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context!.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetCreationFields(entry.Entity, username, utcNow);
                    break;
                case EntityState.Modified:
                    SetModificationFields(entry.Entity, username, utcNow);
                    break;
            }
        }
    }

    private bool ShouldProcess(DbContext? context) => 
        context != null && 
        _userContext.IsAuthenticated &&
        !string.IsNullOrWhiteSpace(_userContext.Username);

    private static void SetCreationFields(IAuditableEntity entity, string username, DateTime utcNow)
    {
        entity.CreatedBy = username;
        entity.UpdatedBy = username;
        entity.CreatedAt = utcNow;
        entity.UpdatedAt = utcNow;
    }

    private static void SetModificationFields(IAuditableEntity entity, string username, DateTime utcNow)
    {
        entity.UpdatedBy = username;
        entity.UpdatedAt = utcNow;
    }
}
