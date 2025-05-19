using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Todo;
using RuanFa.Shop.Domain.Todo.Entities;

namespace RuanFa.Shop.Application.Common.Data;

public interface IApplicationDbContext
{
    // Accounts
    DbSet<UserProfile> Profiles { get; }

    // Todo testing
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }

    // Attributes 
    DbSet<CatalogAttribute> Attributes { get; }
    DbSet<AttributeOption> AttributeOptions { get; }
    DbSet<AttributeGroup> AttributeGroups { get; }
    DbSet<AttributeGroupAttribute> AttributeGroupAttributes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
