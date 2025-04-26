using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces;
using RuanFa.Shop.SharedKernel.Models;
using Shouldly;
using Xunit;

namespace RuanFa.Shop.SharedKernel.UnitTests.Models;

public class AggregateRootTests
{
    private class TestAggregateRoot : AggregateRoot<int>
    {
        public TestAggregateRoot() : base() { }
        public TestAggregateRoot(int id) : base(id) { }
        public void SetCreatedBy(string createdBy) => CreatedBy = createdBy;
        public void SetUpdatedBy(string updatedBy) => UpdatedBy = updatedBy;
    }

    private class DifferentTestAggregateRoot : AggregateRoot<int>
    {
        public DifferentTestAggregateRoot(int id) : base(id) { }
    }

    private class TestEvent : IDomainEvent { }

    #region Constructor Tests
    public class ConstructorTests
    {
        [Fact]
        public void DefaultConstructor_InitializesPropertiesCorrectly()
        {
            var aggregate = new TestAggregateRoot();
            aggregate.Id.ShouldBe(0); // Default value for int
            aggregate.CreatedBy.ShouldBeNull();
            aggregate.UpdatedBy.ShouldBeNull();
            aggregate.CreatedAt.ShouldBe(default);
            aggregate.UpdatedAt.ShouldBeNull();
            aggregate.DomainEvents.ShouldNotBeNull();
            aggregate.DomainEvents.Count.ShouldBe(0);
        }

        [Fact]
        public void IdConstructor_InitializesPropertiesCorrectly()
        {
            var aggregate = new TestAggregateRoot(42);
            aggregate.Id.ShouldBe(42);
            aggregate.CreatedBy.ShouldBeNull();
            aggregate.UpdatedBy.ShouldBeNull();
            aggregate.CreatedAt.ShouldBe(default);
            aggregate.UpdatedAt.ShouldBeNull();
            aggregate.DomainEvents.ShouldNotBeNull();
            aggregate.DomainEvents.Count.ShouldBe(0);
        }
    }
    #endregion

    #region Property Tests (Inherited from Entity)
    public class PropertyTests
    {
        [Fact]
        public void CreatedBy_CanBeSetAndRetrieved()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.SetCreatedBy("user1");
            aggregate.CreatedBy.ShouldBe("user1");
        }

        [Fact]
        public void UpdatedBy_CanBeSetAndRetrieved()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.SetUpdatedBy("user2");
            aggregate.UpdatedBy.ShouldBe("user2");
        }

        [Fact]
        public void CreatedAt_CanBeSetAndRetrieved()
        {
            var aggregate = new TestAggregateRoot(1);
            var date = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
            aggregate.CreatedAt = date;
            aggregate.CreatedAt.ShouldBe(date);
        }

        [Fact]
        public void UpdatedAt_CanBeSetAndRetrieved()
        {
            var aggregate = new TestAggregateRoot(1);
            var date = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
            aggregate.UpdatedAt = date;
            aggregate.UpdatedAt.ShouldBe(date);
        }
    }
    #endregion

    #region Domain Event Tests
    public class DomainEventTests
    {
        [Fact]
        public void AddDomainEvent_WithValidEvent_AddsToList()
        {
            var aggregate = new TestAggregateRoot(1);
            var domainEvent = new TestEvent();
            var result = aggregate.AddDomainEvent(domainEvent);

            result.IsError.ShouldBeFalse();
            aggregate.DomainEvents.Count.ShouldBe(1);
            aggregate.DomainEvents.ShouldContain(domainEvent);
        }

        [Fact]
        public void AddDomainEvent_WithNullEvent_ReturnsError()
        {
            var aggregate = new TestAggregateRoot(1);
            var result = aggregate.AddDomainEvent(null!);

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("AggregateRoot.NullEvent");
            aggregate.DomainEvents.Count.ShouldBe(0);
        }

        [Fact]
        public void ClearDomainEvents_RemovesAllEvents()
        {
            var aggregate = new TestAggregateRoot(1);
            var domainEvent = new TestEvent();
            aggregate.AddDomainEvent(domainEvent);

            var result = aggregate.ClearDomainEvents();

            result.IsError.ShouldBeFalse();
            aggregate.DomainEvents.Count.ShouldBe(0);
        }
    }
    #endregion

    #region Equality Tests (Inherited from Entity)
    public class EqualityTests
    {
        [Fact]
        public void Equals_SameIdAndType_ReturnsTrue()
        {
            var aggregate1 = new TestAggregateRoot(1);
            var aggregate2 = new TestAggregateRoot(1);

            aggregate1.Equals(aggregate2).ShouldBeTrue();
            (aggregate1 == aggregate2).ShouldBeTrue();
            aggregate1.GetHashCode().ShouldBe(aggregate2.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentId_ReturnsFalse()
        {
            var aggregate1 = new TestAggregateRoot(1);
            var aggregate2 = new TestAggregateRoot(2);

            aggregate1.Equals(aggregate2).ShouldBeFalse();
            (aggregate1 != aggregate2).ShouldBeTrue();
            aggregate1.GetHashCode().ShouldNotBe(aggregate2.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var aggregate1 = new TestAggregateRoot(1);
            var aggregate2 = new DifferentTestAggregateRoot(1);

            aggregate1.Equals(aggregate2).ShouldBeFalse();
            (aggregate1 != aggregate2).ShouldBeTrue();
        }

        [Fact]
        public void Equals_Null_ReturnsFalse()
        {
            var aggregate = new TestAggregateRoot(1);

            aggregate.Equals(null).ShouldBeFalse();
            (aggregate == null).ShouldBeFalse();
            (null != aggregate).ShouldBeTrue();
        }

        [Fact]
        public void Equals_SameReference_ReturnsTrue()
        {
            var aggregate = new TestAggregateRoot(1);

            aggregate.Equals(aggregate).ShouldBeTrue();
        }

        [Fact]
        public void GetHashCode_SameIdAndType_ReturnsSameHash()
        {
            var aggregate1 = new TestAggregateRoot(1);
            var aggregate2 = new TestAggregateRoot(1);

            aggregate1.GetHashCode().ShouldBe(aggregate2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentId_ReturnsDifferentHash()
        {
            var aggregate1 = new TestAggregateRoot(1);
            var aggregate2 = new TestAggregateRoot(2);

            aggregate1.GetHashCode().ShouldNotBe(aggregate2.GetHashCode());
        }
    }
    #endregion
}
