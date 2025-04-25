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

        // Expose protected methods for testing
        public new ErrorOr<Updated> SetCreatedBy(string? creator) => base.SetCreatedBy(creator);
        public new ErrorOr<Updated> SetUpdatedBy(string? updater) => base.SetUpdatedBy(updater);
        public new void TrackChange(string? updatedBy) => base.TrackChange(updatedBy);
        protected new void IncrementVersion() => base.IncrementVersion();
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
            aggregate.CreatedBy.ShouldBeNull();
            aggregate.UpdatedBy.ShouldBeNull();
            aggregate.IsDeleted.ShouldBeFalse();
            aggregate.DeletedAt.ShouldBeNull();
            aggregate.DeletedBy.ShouldBeNull();
            aggregate.Version.ShouldBe(1);
            aggregate.CreatedAt.ShouldNotBe(default);
        }
    }

    public class ActionTrackingTests
    {
        [Fact]
        public void SetCreatedBy_WhenNotDeleted_Succeeds()
        {
            var aggregate = new TestAggregateRoot(1);
            var creator = "test.user";

            var result = aggregate.SetCreatedBy(creator);

            result.IsError.ShouldBeFalse();
            aggregate.CreatedBy.ShouldBe(creator);
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void SetCreatedBy_WhenDeleted_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            var initialVersion = aggregate.Version;

            var result = aggregate.SetCreatedBy("new.user");

            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorType.Conflict);
            result.FirstError.Code.ShouldBe("AggregateRoot.Deleted");
            aggregate.Version.ShouldBe(initialVersion);
        }

        [Fact]
        public void SetUpdatedBy_WhenNotDeleted_Succeeds()
        {
            var aggregate = new TestAggregateRoot(1);
            var updater = "test.updater";

            var result = aggregate.SetUpdatedBy(updater);

            result.IsError.ShouldBeFalse();
            aggregate.UpdatedBy.ShouldBe(updater);
            aggregate.UpdatedAt.ShouldNotBeNull();
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void SetUpdatedBy_WhenDeleted_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            var initialVersion = aggregate.Version;

            var result = aggregate.SetUpdatedBy("new.user");

            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorType.Conflict);
            result.FirstError.Code.ShouldBe("AggregateRoot.Deleted");
            aggregate.Version.ShouldBe(initialVersion);
        }

        [Fact]
        public void TrackChange_WhenNoCreator_SetsCreatedBy()
        {
            var aggregate = new TestAggregateRoot(1);
            var user = "test.user";

            aggregate.TrackChange(user);

            aggregate.CreatedBy.ShouldBe(user);
            aggregate.UpdatedBy.ShouldBeNull();
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void TrackChange_WhenHasCreator_SetsUpdatedBy()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.SetCreatedBy("creator");
            var updater = "updater";

            aggregate.TrackChange(updater);

            aggregate.CreatedBy.ShouldBe("creator");
            aggregate.UpdatedBy.ShouldBe(updater);
            aggregate.Version.ShouldBe(3);
        }
    }

    public class DomainEventTests
    {
        [Fact]
        public void AddDomainEvent_WhenNotDeleted_Succeeds()
        {
            var aggregate = new TestAggregateRoot(1);
            var @event = new TestEvent();

            var result = aggregate.AddDomainEvent(@event);

            result.IsError.ShouldBeFalse();
            aggregate.DomainEvents.ShouldContain(@event);
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void AddDomainEvent_WhenDeleted_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            aggregate.Delete("test.user");
            var initialVersion = aggregate.Version;

            var result = aggregate.AddDomainEvent(new TestEvent());

            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorType.Conflict);
            result.FirstError.Code.ShouldBe("AggregateRoot.Deleted");
            aggregate.Version.ShouldBe(initialVersion);
        }

        [Fact]
        public void AddDomainEvent_WhenEventIsNull_Fails()
        {
            var aggregate = new TestAggregateRoot(1);
            var initialVersion = aggregate.Version;

            var result = aggregate.AddDomainEvent(null!);

            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorType.Validation);
            result.FirstError.Code.ShouldBe("AggregateRoot.NullEvent");
            aggregate.Version.ShouldBe(initialVersion);
        }

        private class TestEvent : IDomainEvent { }
    }

    public class SoftDeleteTests
    {
        [Fact]
        public void Delete_WhenNotDeleted_Succeeds()
        {
            // Arrange
            var aggregate = new TestAggregateRoot(1);
            var deleter = "test.user";
            var beforeDelete = DateTimeOffset.UtcNow;

            // Act
            var result = aggregate.Delete(deleter);

            // Assert
            if (result.IsError)
            {
                throw new XunitException($"Delete failed with error: {result.FirstError}");
            }

            result.Value.ShouldBe(Result.Deleted);
            aggregate.IsDeleted.ShouldBeTrue();
            aggregate.DeletedAt.ShouldNotBeNull();
            aggregate.DeletedAt.Value.ShouldBeGreaterThan(beforeDelete);
            aggregate.DeletedBy.ShouldBe(deleter);
            aggregate.UpdatedBy.ShouldBe(deleter);
            aggregate.Version.ShouldBe(2);
        }

        [Fact]
        public void Delete_WhenAlreadyDeleted_Fails()
        {
            // Arrange
            var aggregate = new TestAggregateRoot(1);
            var firstDeleteResult = aggregate.Delete("test.user");

            if (firstDeleteResult.IsError)
            {
                throw new XunitException($"First delete failed with error: {firstDeleteResult.FirstError}");
            }

            var initialVersion = aggregate.Version;

            // Act
            var result = aggregate.Delete("new.user");

            // Assert
            result.IsError.ShouldBeTrue();
            result.FirstError.Type.ShouldBe(ErrorType.Conflict);
            result.FirstError.Code.ShouldBe("AggregateRoot.AlreadyDeleted");
            aggregate.Version.ShouldBe(initialVersion);
        }

        [Fact]
        public void Delete_UpdatesAllRelevantProperties()
        {
            // Arrange
            var aggregate = new TestAggregateRoot(1);
            var deleter = "test.user";
            var beforeDelete = DateTimeOffset.UtcNow;

            // Act
            var result = aggregate.Delete(deleter);

            // Assert
            if (result.IsError)
            {
                throw new XunitException($"Delete failed with error: {result.FirstError}");
            }

            aggregate.IsDeleted.ShouldBeTrue();
            aggregate.DeletedAt.ShouldNotBeNull();
            aggregate.DeletedAt.Value.ShouldBeGreaterThan(beforeDelete);
            aggregate.DeletedBy.ShouldBe(deleter);
            aggregate.UpdatedBy.ShouldBe(deleter);
            aggregate.UpdatedAt.ShouldNotBeNull();
            aggregate.UpdatedAt.Value.ShouldBeGreaterThan(beforeDelete);
            aggregate.Version.ShouldBeGreaterThan(1);
        }

        [Fact]
        public void Delete_SetsUpdatedByAndDeletedByToSameUser()
        {
            // Arrange
            var aggregate = new TestAggregateRoot(1);
            var deleter = "test.user";

            // Act
            var result = aggregate.Delete(deleter);

            // Assert
            if (result.IsError)
            {
                throw new XunitException($"Delete failed with error: {result.FirstError}");
            }

            aggregate.DeletedBy.ShouldBe(deleter);
            aggregate.UpdatedBy.ShouldBe(deleter);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("test.user")]
        public void Delete_AcceptsNullAndNonNullDeleter(string? deleter)
        {
            // Arrange
            var aggregate = new TestAggregateRoot(1);

            // Act
            var result = aggregate.Delete(deleter);

            // Assert
            result.IsError.ShouldBeFalse();
            aggregate.IsDeleted.ShouldBeTrue();
            aggregate.DeletedBy.ShouldBe(deleter);
            aggregate.UpdatedBy.ShouldBe(deleter);
        }
    }


    public class InheritanceTests
    {
        [Fact]
        public void ImplementsRequiredInterfaces()
        {
            var aggregate = new TestAggregateRoot(1);

            aggregate.ShouldBeAssignableTo<Entity<int>>();
            aggregate.ShouldBeAssignableTo<IActionTrackable>();
            aggregate.ShouldBeAssignableTo<IDeletableEntity>();
            aggregate.ShouldBeAssignableTo<IVersionable>();
        }
    }
}
