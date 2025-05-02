namespace RuanFa.Shop.Application.UnitTests.Common.Extensions.Queries;

using Microsoft.EntityFrameworkCore;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    public DbSet<TestEntity> TestEntities { get; set; } = null!;
    public DbSet<NonStringEntity> NonStringEntities { get; set; } = null!;
}

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public class NonStringEntity
{
    public int Id { get; set; }
    public int Number { get; set; }
}


public static class TestDataSetup
{
    public static async Task<IQueryable<TestEntity>> GetTestDataQueryable()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new TestDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var entities = new[]
        {
           new TestEntity { Id = 1, Name = "Alice", Age = 25, CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0) },
        new TestEntity { Id = 2, Name = "Bob",   Age = 30, CreatedDate = new DateTime(2025, 2, 1, 0, 0, 0) },
        new TestEntity { Id = 3, Name = "Carol", Age = 35, CreatedDate = new DateTime(2025, 3, 1, 0, 0, 0) },
        new TestEntity { Id = 4, Name = "Dave",  Age = 40, CreatedDate = new DateTime(2025, 4, 1, 0, 0, 0) }

        };

        context.AddRange(entities);
        await context.SaveChangesAsync();

        return context.Set<TestEntity>().AsQueryable();
    }

    // Placeholder for GetEmptyTestDataQueryable
    public static async Task<IQueryable<TestEntity>> GetEmptyTestDataQueryable()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new TestDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context.Set<TestEntity>().AsQueryable();
    }
}
