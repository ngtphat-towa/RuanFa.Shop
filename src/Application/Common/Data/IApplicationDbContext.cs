using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Domain.Accounts.Entities;
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
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
