using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces;
using RuanFa.Shop.SharedKernel.Models;

namespace RuanFa.Shop.SharedKernel.UnitTests.Models;

public class EntityTests
{
    #region Test Classes
    private class TestEvent : IDomainEvent { }

    private class TestEntity : Entity<int>
    {
        public TestEntity() : base() { }
        public TestEntity(int id) : base(id) { }

        public new ErrorOr<Updated> UpdateModificationTime() => base.UpdateModificationTime();
        public new ErrorOr<Updated> SetCreatedBy(string? creator) => base.SetCreatedBy(creator);
        public new ErrorOr<Updated> SetUpdatedBy(string? updater) => base.SetUpdatedBy(updater);
        protected override bool CanModify() => !IsModificationDisabled;
        public bool IsModificationDisabled { get; set; }
    }

    private class DifferentTestEntity : Entity<int>
    {
        public DifferentTestEntity(int id) : base(id) { }
    }
    #endregion

    #region Constructor Tests
    public class ConstructorTests
    {
        [Fact]
        public void DefaultConstructor_InitializesEmptyDomainEvents()
        {
            var entity = new TestEntity();

            entity.DomainEvents.ShouldNotBeNull();
            entity.DomainEvents.Count.ShouldBe(0);
            entity.CreatedAt.ShouldNotBe(default);
            entity.UpdatedAt.ShouldBeNull();
            entity.CreatedBy.ShouldBeNull();
            entity.UpdatedBy.ShouldBeNull();
        }

        [Fact]
        public void IdConstructor_SetsIdAndCreatedAt()
        {
            var entity = new TestEntity(42);

            entity.Id.ShouldBe(42);
            entity.CreatedAt.ShouldNotBe(default);
            entity.UpdatedAt.ShouldBeNull();
            entity.CreatedBy.ShouldBeNull();
            entity.UpdatedBy.ShouldBeNull();
        }
    }
    #endregion

    #region Domain Event Tests
    // in EntityTests.cs, update the DomainEventTests class
    public class DomainEventTests
    {
        [Fact]
        public void AddDomainEvent_WhenValid_Succeeds()
        {
            var entity = new TestEntity(1);
            var domainEvent = new TestEvent();

            var result = entity.AddDomainEvent(domainEvent);

            result.IsError.ShouldBeFalse();
            entity.DomainEvents.Count.ShouldBe(1);
            entity.DomainEvents.First().ShouldBe(domainEvent);
            entity.UpdatedAt.ShouldNotBeNull();
        }

        [Fact]
        public void AddDomainEvent_WhenNull_ReturnsError()
        {
            var entity = new TestEntity(1);

            var result = entity.AddDomainEvent(null!);

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.DomainEventNull");
            entity.DomainEvents.Count.ShouldBe(0);
        }

        [Fact]
        public void AddDomainEvent_WhenCannotModify_ReturnsError()
        {
            var entity = new TestEntity(1) { IsModificationDisabled = true };

            var result = entity.AddDomainEvent(new TestEvent());

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.InvalidModification");
            entity.DomainEvents.Count.ShouldBe(0);
        }

        [Fact]
        public void ClearDomainEvents_WhenCanModify_Succeeds()
        {
            var entity = new TestEntity(1);
            entity.AddDomainEvent(new TestEvent());
            var beforeClear = entity.UpdatedAt;

            var result = entity.ClearDomainEvents();

            result.IsError.ShouldBeFalse();
            entity.DomainEvents.Count.ShouldBe(0);
            entity.UpdatedAt.ShouldNotBe(beforeClear);
        }

        [Fact]
        public void ClearDomainEvents_WhenCannotModify_ReturnsError()
        {
            var entity = new TestEntity(1);
            entity.AddDomainEvent(new TestEvent());
            entity.IsModificationDisabled = true;
            var initialCount = entity.DomainEvents.Count;

            var result = entity.ClearDomainEvents();

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.InvalidModification");
            entity.DomainEvents.Count.ShouldBe(initialCount);
        }
    }


    #endregion

    #region Auditing Tests
    public class AuditingTests
    {
        [Fact]
        public void SetCreatedBy_WhenValid_Succeeds()
        {
            var entity = new TestEntity(1);
            var creator = "test.user";

            var result = entity.SetCreatedBy(creator);

            result.IsError.ShouldBeFalse();
            entity.CreatedBy.ShouldBe(creator);
            entity.UpdatedAt.ShouldNotBeNull();
        }

        [Fact]
        public void SetUpdatedBy_WhenValid_Succeeds()
        {
            var entity = new TestEntity(1);
            var updater = "test.updater";

            var result = entity.SetUpdatedBy(updater);

            result.IsError.ShouldBeFalse();
            entity.UpdatedBy.ShouldBe(updater);
            entity.UpdatedAt.ShouldNotBeNull();
        }

        [Fact]
        public void SetCreatedBy_WhenCannotModify_ReturnsError()
        {
            var entity = new TestEntity(1) { IsModificationDisabled = true };

            var result = entity.SetCreatedBy("test.user");

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.InvalidModification");
        }

        [Fact]
        public void SetUpdatedBy_WhenCannotModify_ReturnsError()
        {
            var entity = new TestEntity(1) { IsModificationDisabled = true };

            var result = entity.SetUpdatedBy("test.user");

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.InvalidModification");
        }

        [Fact]
        public void UpdateModificationTime_WhenValid_Succeeds()
        {
            var entity = new TestEntity(1);
            var beforeUpdate = DateTimeOffset.UtcNow;

            var result = entity.UpdateModificationTime();

            result.IsError.ShouldBeFalse();
            entity.UpdatedAt.ShouldNotBeNull();
            entity.UpdatedAt.Value.ShouldBeGreaterThan(beforeUpdate);
        }

        [Fact]
        public void UpdateModificationTime_WhenCannotModify_ReturnsError()
        {
            var entity = new TestEntity(1) { IsModificationDisabled = true };

            var result = entity.UpdateModificationTime();

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.InvalidModification");
        }
    }
    #endregion

    #region Equality Tests
    public class EqualityTests
    {
        [Fact]
        public void Equals_WhenSameReference_ReturnsTrue()
        {
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

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        public void Equals_WhenDifferentIds_ReturnsFalse(int id1, int id2)
        {
            var entity1 = new TestEntity(id1);
            var entity2 = new TestEntity(id2);

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
    #endregion
}
