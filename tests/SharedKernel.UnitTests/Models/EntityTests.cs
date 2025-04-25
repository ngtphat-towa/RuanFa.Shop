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

        public void UpdateTime() => UpdateModificationTime();
    }

    private class DifferentTestEntity : Entity<int>
    {
        public DifferentTestEntity(int id) : base(id) { }
    }

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
        public void IdConstructor_SetsIdAndCreatedAt()
        {
            var entity = new TestEntity(42);

            entity.Id.ShouldBe(42);
            entity.CreatedAt.ShouldNotBe(default);
            entity.UpdatedAt.ShouldBeNull();
        }
    }

    public class DomainEventTests
    {
        [Fact]
        public void AddDomainEvent_AddsEventToList()
        {
            var entity = new TestEntity(1);
            var domainEvent = new TestEvent();

            entity.AddDomainEvent(domainEvent);

            entity.DomainEvents.Count.ShouldBe(1);
            entity.DomainEvents.First().ShouldBe(domainEvent);
        }

        [Fact]
        public void AddDomainEvent_ThrowsWhenEventIsNull()
        {
            var entity = new TestEntity(1);

            Should.Throw<ArgumentNullException>(() => entity.AddDomainEvent(null!));
        }

        [Fact]
        public void ClearDomainEvents_RemovesAllEvents()
        {
            var entity = new TestEntity(1);
            entity.AddDomainEvent(new TestEvent());
            entity.AddDomainEvent(new TestEvent());

            entity.ClearDomainEvents();

            entity.DomainEvents.Count.ShouldBe(0);
        }
    }

    public class AuditableTests
    {
        [Fact]
        public void UpdateModificationTime_SetsUpdatedAt()
        {
            var entity = new TestEntity(1);
            entity.UpdateTime();

            entity.UpdatedAt.ShouldNotBeNull();
            entity.UpdatedAt.Value.ShouldBeInRange(
                DateTimeOffset.UtcNow.AddSeconds(-1),
                DateTimeOffset.UtcNow.AddSeconds(1));
        }
    }

    public class EqualityTests
    {
        [Fact]
        public void Equals_WhenSameReference_ReturnsTrue()
        {
            // Original had entity compared to itself
            var entity1 = new TestEntity(1);
            var entity2 = entity1;

            entity1.Equals(entity2).ShouldBeTrue();
            (entity1 == entity2).ShouldBeTrue();
        }
        [Fact]
        public void Equals_WhenNull_ReturnsFalse()
        {
            var entity = new TestEntity(1);

            entity.Equals(null).ShouldBeFalse();
            (entity == null).ShouldBeFalse();
            (null == entity).ShouldBeFalse();
        }

        [Fact]
        public void Equals_WhenSameIdButDifferentTypes_ReturnsFalse()
        {
            var entity1 = new TestEntity(1);
            var entity2 = new DifferentTestEntity(1);

            entity1.Equals(entity2).ShouldBeFalse();
            (entity1 == entity2).ShouldBeFalse();
        }

        [Fact]
        public void Equals_WhenSameTypeAndId_ReturnsTrue()
        {
            var entity1 = new TestEntity(1);
            var entity2 = new TestEntity(1);

            entity1.Equals(entity2).ShouldBeTrue();
            (entity1 == entity2).ShouldBeTrue();
        }

        [Fact]
        public void Equals_WhenDifferentIds_ReturnsFalse()
        {
            var entity1 = new TestEntity(1);
            var entity2 = new TestEntity(2);

            entity1.Equals(entity2).ShouldBeFalse();
            (entity1 != entity2).ShouldBeTrue();
        }

        [Fact]
        public void GetHashCode_ReturnsSameValueForEqualEntities()
        {
            var entity1 = new TestEntity(1);
            var entity2 = new TestEntity(1);

            entity1.GetHashCode().ShouldBe(entity2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_ReturnsDifferentValuesForDifferentEntities()
        {
            var entity1 = new TestEntity(1);
            var entity2 = new TestEntity(2);

            entity1.GetHashCode().ShouldNotBe(entity2.GetHashCode());
        }

        [Fact]
        public void Operators_HandleNullValues()
        {
            TestEntity? nullEntity = null;
            var entity = new TestEntity(1);

            (nullEntity == null).ShouldBeTrue();
            (null == nullEntity).ShouldBeTrue();
            (entity != null).ShouldBeTrue();
            (null != entity).ShouldBeTrue();
        }
    }
}
