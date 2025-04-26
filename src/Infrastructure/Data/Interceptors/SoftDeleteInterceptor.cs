using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.SharedKernel.Interfaces;
using RuanFa.Shop.SharedKernel.Models;
using RuanFa.Shop.SharedKernel.Services;

namespace Infrastructure.Data.Interceptors;
internal sealed class SoftDeleteInterceptor(IUserContext userContext) : SaveChangesInterceptor
{
    private readonly IUserContext _userContext = userContext;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        HandleSoftDelete(eventData.Context);
        return result;
    }

    private void HandleSoftDelete(DbContext? context)
    {
        if (context is null || !_userContext.IsAuthenticated) return;

        var entries = context.ChangeTracker
            .Entries<IDeletableEntity>()
            .Where(e => e.State == EntityState.Deleted &&
                       e.Entity is IPersistable persistable && persistable.EnableSoftDelete);

        foreach (var entry in entries)
        {
            if (entry.Entity is AggregateRoot<Guid> aggregate)
            {
                aggregate.Delete(_userContext.Username);
                entry.State = EntityState.Modified;
            }
        }
    }
}
