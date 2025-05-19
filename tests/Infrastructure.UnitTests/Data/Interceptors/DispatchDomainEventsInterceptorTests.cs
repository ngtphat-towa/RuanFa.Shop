using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RuanFa.Shop.Infrastructure.Data.Interceptors;
using RuanFa.Shop.SharedKernel.Models.Domains;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Infrastructure.UnitTests.Data.Interceptors;

public class DispatchDomainEventsInterceptorTests
{
    [Fact]
    public async Task SavingChangesAsync_ShouldPublishAndClearEvents()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        mediator.Publish(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

        var interceptor = new DispatchDomainEventsInterceptor(mediator);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("DispatchDb1")
            .AddInterceptors(interceptor)
            .Options;

        await using var ctx = new TestDbContext(options);

        var entity = new SampleEntity(Guid.NewGuid());
        var ev = new SampleEvent();
        entity.AddDomainEvent(ev).IsError.ShouldBeFalse();

        ctx.Entities.Add(entity);

        // Act
        await ctx.SaveChangesAsync();

        // Assert
        await mediator.Received(1).Publish(ev, Arg.Any<CancellationToken>());
        entity.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldHandleMultipleEvents()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        mediator.Publish(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);
        var interceptor = new DispatchDomainEventsInterceptor(mediator);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("DispatchDb2")
            .AddInterceptors(interceptor)
            .Options;

        await using var ctx = new TestDbContext(options);
        var entity = new SampleEntity(Guid.NewGuid());
        var events = new[] { new SampleEvent(), new SampleEvent() };
        foreach (var e in events) entity.AddDomainEvent(e).IsError.ShouldBeFalse();
        ctx.Entities.Add(entity);

        // Act
        await ctx.SaveChangesAsync();

        // Assert
        foreach (var e in events)
            await mediator.Received(1).Publish(e, Arg.Any<CancellationToken>());
        entity.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public async Task SavingChangesAsync_ShouldNotPublish_WhenNoEvents()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var interceptor = new DispatchDomainEventsInterceptor(mediator);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("DispatchDb3")
            .AddInterceptors(interceptor)
            .Options;

        await using var ctx = new TestDbContext(options);
        ctx.Entities.Add(new SampleEntity(Guid.NewGuid()));

        // Act
        await ctx.SaveChangesAsync();

        // Assert
        await mediator.DidNotReceive()
                      .Publish(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void SavingChanges_Sync_ShouldPublishEvents()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        mediator.Publish(Arg.Any<IDomainEvent>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

        var interceptor = new DispatchDomainEventsInterceptor(mediator);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("DispatchDb4")
            .AddInterceptors(interceptor)
            .Options;

        using var ctx = new TestDbContext(options);
        var entity = new SampleEntity(Guid.NewGuid());
        var ev = new SampleEvent();
        entity.AddDomainEvent(ev).IsError.ShouldBeFalse();

        ctx.Entities.Add(entity);

        // Act
        ctx.SaveChanges();

        // Assert
        mediator.Received(1).Publish(ev, Arg.Any<CancellationToken>());
        entity.DomainEvents.ShouldBeEmpty();
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> opts) : base(opts) { }
        public DbSet<SampleEntity> Entities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }

    private class SampleEntity : Entity<Guid>
    {
        public SampleEntity(Guid id) : base(id) { }
    }

    private class SampleEvent : IDomainEvent { }
}
