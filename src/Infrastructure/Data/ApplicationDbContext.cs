using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.Domain.Todo;
using RuanFa.Shop.Domain.Todo.Entities;
using RuanFa.Shop.Infrastructure.Accounts.Entities;

namespace RuanFa.Shop.Infrastructure.Data;

internal class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options), IApplicationDbContext
{
    public DbSet<UserProfile> Profiles => Set<UserProfile>();

    // Todo
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
