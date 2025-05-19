using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RuanFa.Shop.Application.Common.Security.Authentications;
using RuanFa.Shop.Application.Common.Services;
using RuanFa.Shop.Infrastructure.Data.Interceptors;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Infrastructure.UnitTests.Data.Interceptors;

public class AuditableEntityInterceptorTests
{
    [Fact]
    public async Task SavingChangesAsync_ShouldSetAuditFields_OnNewEntity()
    {
        // Arrange
        var userContext = Substitute.For<IUserContext>();
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var interceptor = new AuditableEntityInterceptor(userContext, dateTimeProvider);

        userContext.IsAuthenticated.Returns(true);
        userContext.Username.Returns("Alice");

        var now = DateTime.UtcNow;
        dateTimeProvider.UtcNow.Returns(now);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("AuditDb1")
            .AddInterceptors(interceptor)
            .Options;

        await using var ctx = new TestDbContext(options);
        var e = new SampleEntity(Guid.NewGuid());
        ctx.Entities.Add(e);

        // Act
        await ctx.SaveChangesAsync();

        // Assert
        e.CreatedBy.ShouldBe("Alice");
        e.CreatedAt.ShouldBe(now);
        e.UpdatedBy.ShouldBe("Alice");
        e.UpdatedAt.ShouldBe(now);
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldPreserveCreatedFields_OnUpdate()
    {
        var dbName = Guid.NewGuid().ToString();
        var id = Guid.NewGuid();

        //
        // 1) First context: stamp Created* and Updated* as Creator/originalCreated
        //
        var userCtx = Substitute.For<IUserContext>();
        var dt = Substitute.For<IDateTimeProvider>();
        userCtx.IsAuthenticated.Returns(true);
        userCtx.Username.Returns("Creator");
        var originalCreated = DateTime.UtcNow.AddDays(-2);
        dt.UtcNow.Returns(originalCreated);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(dbName)
            .AddInterceptors(new AuditableEntityInterceptor(userCtx, dt))
            .Options;

        await using (var ctx1 = new TestDbContext(options))
        {
            var e1 = new SampleEntity(id);
            ctx1.Entities.Add(e1);
            await ctx1.SaveChangesAsync();

            // verify first‐save stamps
            e1.CreatedBy.ShouldBe("Creator");
            e1.CreatedAt.ShouldBe(originalCreated);
            e1.UpdatedBy.ShouldBe("Creator");
            e1.UpdatedAt.ShouldBe(originalCreated);
        }

        //
        // 2) Second context: load the same row, update, stamp Updated* only
        //
        userCtx.IsAuthenticated.Returns(true);
        userCtx.Username.Returns("Bob");
        var now = DateTime.UtcNow;
        dt.UtcNow.Returns(now);

        await using (var ctx2 = new TestDbContext(options))
        {
            var e2 = await ctx2.Entities.SingleAsync(x => x.Id == id);
            // Modify a non‐audit field so EF marks it Modified
            e2.SomeFeild = "changed";
            await ctx2.SaveChangesAsync();

            // Created* must remain from originalCreated/Creator
            e2.CreatedBy.ShouldBe("Creator");
            e2.CreatedAt.ShouldBe(originalCreated);

            // Updated* must now be from now/Bob
            e2.UpdatedBy.ShouldBe("Bob");
            e2.UpdatedAt.ShouldBe(now);
        }
    }



    [Fact]
    public async Task SavingChangesAsync_ShouldDoNothing_WhenNotAuthenticated()
    {
        // Arrange
        var userContext = Substitute.For<IUserContext>();
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        var interceptor = new AuditableEntityInterceptor(userContext, dateTimeProvider);

        userContext.IsAuthenticated.Returns(false);
        dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("AuditDb3")
            .AddInterceptors(interceptor)
            .Options;

        await using var ctx = new TestDbContext(options);
        var e = new SampleEntity(Guid.NewGuid());
        ctx.Entities.Add(e);

        // Act
        await ctx.SaveChangesAsync();

        // Assert
        e.CreatedBy.ShouldBeNull();
        e.UpdatedBy.ShouldBeNull();
        e.CreatedAt.ShouldBe(default);
        e.UpdatedAt.ShouldBeNull();
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> opts) : base(opts) { }
        public DbSet<SampleEntity> Entities { get; set; } = null!;
    }

    private class SampleEntity : Entity<Guid>
    {
        public SampleEntity(Guid id) : base(id) { }

        public string? SomeFeild { get; set; }
    }
}
