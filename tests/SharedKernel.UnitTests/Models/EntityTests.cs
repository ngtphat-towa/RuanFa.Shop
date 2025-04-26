using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces;
using RuanFa.Shop.SharedKernel.Models;

namespace RuanFa.Shop.SharedKernel.UnitTests.Models;

public class EntityTests
{
    private class TestEvent : IDomainEvent { }

    private class TestEntity : Entity<int>
    {
        public TestEntity() : base() { }
        public TestEntity(int id) : base(id) { }

        // Helper methods for testing - exposing the results directly
        public ErrorOr<Success> AddTestEvent(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);
        public ErrorOr<Success> ClearTestEvents() => ClearDomainEvents();
    }

    private class DifferentTestEntity(int id) : Entity<int>(id)
    {
    }

    #region Constructor Tests
    public class ConstructorTests
    {
        [Fact]
        public void DefaultConstructor_InitializesEmptyDomainEvents()
        {
            var entity = new TestEntity();
            entity.DomainEvents.ShouldNotBeNull();
            entity.DomainEvents.Count.ShouldBe(0);
        }

        [Fact]
        public void IdConstructor_SetsId()
        {
            var entity = new TestEntity(42);
            entity.Id.ShouldBe(42);
        }
    }
    #endregion

    #region Domain Event Tests
    public class DomainEventTests
    {
        [Fact]
        public void AddDomainEvent_WithValidEvent_AddsToList()
        {
            var entity = new TestEntity();
            var domainEvent = new TestEvent();
            var result = entity.AddTestEvent(domainEvent);

            result.IsError.ShouldBeFalse();
            entity.DomainEvents.Count.ShouldBe(1);
            entity.DomainEvents.ShouldContain(domainEvent);
        }

        [Fact]
        public void AddDomainEvent_WithNullEvent_ReturnsError()
        {
            var entity = new TestEntity();
            var result = entity.AddTestEvent(null!);

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("DomainEvent.Null");
            entity.DomainEvents.Count.ShouldBe(0);
        }

        [Fact]
        public void ClearDomainEvents_RemovesAllEvents()
        {
            var entity = new TestEntity();
            var domainEvent = new TestEvent();
            entity.AddTestEvent(domainEvent);

            var result = entity.ClearTestEvents();

            result.IsError.ShouldBeFalse();
            entity.DomainEvents.Count.ShouldBe(0);
        }
    }
    #endregion

    #region Equality Tests
    public class EqualityTests
    {
        [Fact]
        public void Equals_SameIdAndType_ReturnsTrue()
        {
            var entity1 = new TestEntity(1);
            var entity2 = new TestEntity(1);

            entity1.Equals(entity2).ShouldBeTrue();
            (entity1 == entity2).ShouldBeTrue();
        }

        [Fact]
        public void Equals_DifferentId_ReturnsFalse()
        {
            var entity1 = new TestEntity(1);
            var entity2 = new TestEntity(2);

            entity1.Equals(entity2).ShouldBeFalse();
            (entity1 != entity2).ShouldBeTrue();
        }

        [Fact]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var entity1 = new TestEntity(1);
            var entity2 = new DifferentTestEntity(1);

            entity1.Equals(entity2).ShouldBeFalse();
            (entity1 != entity2).ShouldBeTrue();
        }

        [Fact]
        public void Equals_Null_ReturnsFalse()
        {
            var entity = new TestEntity(1);

            entity.Equals(null).ShouldBeFalse();
            (entity == null).ShouldBeFalse();
            (null != entity).ShouldBeTrue();
        }
    }
    #endregion
}
