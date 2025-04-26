using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces;
using RuanFa.Shop.SharedKernel.Models;
using Xunit.Sdk;

namespace RuanFa.Shop.SharedKernel.UnitTests.Models;

public class AggregateRootTests
{
    private class TestAggregateRoot : AggregateRoot<int>
    {
        public TestAggregateRoot() : base() { }
        public TestAggregateRoot(int id) : base(id) { }

        // Exposed protected members for testing
        public new ErrorOr<Updated> SetVersion(int version) => base.SetVersion(version);
        public new ErrorOr<Updated> SetDeletedState(bool isDeleted) => base.SetDeletedState(isDeleted);
        public new ErrorOr<Updated> SetDeletedBy(string? deletedBy) => base.SetDeletedBy(deletedBy);
        public new ErrorOr<Updated> SetDeletedAt(DateTimeOffset? deletedAt) => base.SetDeletedAt(deletedAt);
        public new ErrorOr<Updated> SetCreatedBy(string? creator) => base.SetCreatedBy(creator);
        public new ErrorOr<Updated> SetUpdatedBy(string? updater) => base.SetUpdatedBy(updater);
        public new ErrorOr<Success> AddDomainEvent(IDomainEvent domainEvent) => base.AddDomainEvent(domainEvent);
        public new ErrorOr<Success> ClearDomainEvents() => base.ClearDomainEvents();
        public void TestStateChange() => IncrementVersion();
        public bool CanModifyValue => CanModify();
    }

    public class ConstructorTests
    {
        [Fact]
        public void DefaultConstructor_InitializesPropertiesCorrectly()
        {
            var aggregate = new TestAggregateRoot();
            aggregate.CreatedBy.ShouldBeNull();
            aggregate.UpdatedBy.ShouldBeNull();
            aggregate.IsDeleted.ShouldBeFalse();
            aggregate.DeletedAt.ShouldBeNull();
            aggregate.DeletedBy.ShouldBeNull();
            aggregate.Version.ShouldBe(1);
        }

        [Fact]
        public void IdConstructor_InitializesPropertiesCorrectly()
        {
            var aggregate = new TestAggregateRoot(42);
            aggregate.Id.ShouldBe(42);
            aggregate.Version.ShouldBe(1);
            aggregate.CreatedAt.ShouldNotBe(default);
        }
    }

    public class VersionManagementTests
    {
        [Fact]
        public void SetVersion_WhenValidValue_Succeeds()
        {
            var aggregate = new TestAggregateRoot(1) { Version = 1 };
            var result = aggregate.SetVersion(2);

            result.IsError.ShouldBeFalse();
            aggregate.Version.ShouldBe(2);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void SetVersion_WhenInvalidValue_Fails(int version)
        {
            var aggregate = new TestAggregateRoot(1) { Version = 2 };
            var result = aggregate.SetVersion(version);

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Version.InvalidValue");
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void SetVersion_WhenDeleted_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            var result = aggregate.SetVersion(2);

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.InvalidModification");
        }
    }

    public class DeletionStateTests
    {
        [Fact]
        public void SetDeletedState_WhenNotDeleted_Succeeds()
        {
            var aggregate = new TestAggregateRoot(1);
            var result = aggregate.SetDeletedState(true);

            result.IsError.ShouldBeFalse();
            aggregate.IsDeleted.ShouldBeTrue();
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void SetDeletedState_WhenAlreadyDeleted_NoChange()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            var result = aggregate.SetDeletedState(true);

            result.IsError.ShouldBeFalse();
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void SetDeletedState_WhenReactivating_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            var result = aggregate.SetDeletedState(false);

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.InvalidModification");
        }
    }

    public class DeletionMetadataTests
    {
        [Fact]
        public void SetDeletedBy_WhenActive_Succeeds()
        {
            var aggregate = new TestAggregateRoot(1);
            var result = aggregate.SetDeletedBy("admin");

            result.IsError.ShouldBeFalse();
            aggregate.DeletedBy.ShouldBe("admin");
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void SetDeletedAt_WhenActive_Succeeds()
        {
            var aggregate = new TestAggregateRoot(1);
            var timestamp = DateTimeOffset.UtcNow;
            var result = aggregate.SetDeletedAt(timestamp);

            result.IsError.ShouldBeFalse();
            aggregate.DeletedAt.ShouldBe(timestamp);
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void ModifyDeletedMetadata_WhenDeleted_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");

            var atResult = aggregate.SetDeletedAt(DateTimeOffset.UtcNow);
            var byResult = aggregate.SetDeletedBy("new.user");

            atResult.IsError.ShouldBeTrue();
            byResult.IsError.ShouldBeTrue();
        }
    }

    public class StateManagementTests
    {
        [Fact]
        public void CanModify_WhenActive_ReturnsTrue()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.CanModifyValue.ShouldBeTrue();
        }

        [Fact]
        public void CanModify_WhenDeleted_ReturnsFalse()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            aggregate.CanModifyValue.ShouldBeFalse();
        }

        [Fact]
        public void StateChange_IncreasesVersion()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.TestStateChange();
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void StateChange_WhenDeleted_Throws()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            Should.Throw<InvalidOperationException>(() => aggregate.TestStateChange());
        }
    }

    public class ClearDomainEventsTests
    {
        [Fact]
        public void ClearEvents_WhenActive_Succeeds()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.AddDomainEvent(new TestEvent());
            var result = aggregate.ClearDomainEvents();

            result.IsError.ShouldBeFalse();
            aggregate.DomainEvents.ShouldBeEmpty();
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void ClearEvents_WhenDeleted_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            var result = aggregate.ClearDomainEvents();

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("Entity.InvalidModification");
        }

        private class TestEvent : IDomainEvent { }
    }

    public class SoftDeleteTests
    {
        [Fact]
        public void Delete_UpdatesAllProperties()
        {
            var aggregate = new TestAggregateRoot(1);
            var result = aggregate.Delete("admin");

            result.IsError.ShouldBeFalse();
            aggregate.IsDeleted.ShouldBeTrue();
            aggregate.DeletedBy.ShouldBe("admin");
            aggregate.DeletedAt.ShouldNotBeNull();
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void Delete_WhenAlreadyDeleted_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("admin");
            var result = aggregate.Delete("admin");

            result.IsError.ShouldBeTrue();
            result.FirstError.Code.ShouldBe("AggregateRoot.AlreadyDeleted");
        }
    }

    public class InheritanceTests
    {
        [Fact]
        public void ImplementsRequiredInterfaces()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.ShouldBeAssignableTo<IActionTrackable>();
            aggregate.ShouldBeAssignableTo<IDeletableEntity>();
            aggregate.ShouldBeAssignableTo<IVersionable>();
        }
    }
}
