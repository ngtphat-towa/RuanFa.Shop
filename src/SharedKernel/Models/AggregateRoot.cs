using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces;

namespace RuanFa.Shop.SharedKernel.Models;

public abstract class AggregateRoot<TId> : 
    Entity<TId>, 
    IActionTrackable, 
    IDeletableEntity,
    IVersionable
    where TId : notnull
{
    private bool _isDeleted;
    private string? _createdBy;
    private string? _updatedBy;
    private string? _deletedBy;
    private DateTimeOffset? _deletedAt;
 
    protected AggregateRoot() : base()
    {
        Version = 1;
    }

    protected AggregateRoot(TId id) : base(id)
    {
        Version = 1;
    }

    // Version tracking
    public int Version { get; private set; }

    // IActionTrackable implementation
    public string? CreatedBy
    {
        get => _createdBy;
        set => SetCreatedBy(value);
    }

    public string? UpdatedBy
    {
        get => _updatedBy;
        set => SetUpdatedBy(value);
    }

    // IDeletableEntity implementation
    public bool IsDeleted => _isDeleted;
    public DateTimeOffset? DeletedAt => _deletedAt;
    public string? DeletedBy => _deletedBy;

    public static class Errors
    {
        public static Error AggregateDeleted => Error.Conflict(
            code: "AggregateRoot.Deleted",
            description: "Cannot modify a deleted aggregate");

        public static Error DomainEventNull => Error.Validation(
            code: "AggregateRoot.NullEvent",
            description: "Domain event cannot be null");

        public static Error AlreadyDeleted => Error.Conflict(
            code: "AggregateRoot.AlreadyDeleted",
            description: "Aggregate is already deleted");
    }

    protected ErrorOr<Updated> SetCreatedBy(string? creator)
    {
        if (_isDeleted)
            return Errors.AggregateDeleted;

        if (_createdBy != creator)
        {
            _createdBy = creator;
            IncrementVersion();
        }

        return Result.Updated;
    }

    protected ErrorOr<Updated> SetUpdatedBy(string? updater)
    {
        if (_isDeleted)
            return Errors.AggregateDeleted;

        if (_updatedBy != updater)
        {
            _updatedBy = updater;
            UpdateModificationTime();
            IncrementVersion();
        }

        return Result.Updated;
    }

    public new ErrorOr<Success> AddDomainEvent(IDomainEvent domainEvent)
    {
        if (_isDeleted)
            return Errors.AggregateDeleted;

        if (domainEvent is null)
            return Errors.DomainEventNull;

        base.AddDomainEvent(domainEvent);
        IncrementVersion();
        return Result.Success;
    }

    public ErrorOr<Deleted> Delete(string? deletedBy)
    {
        if (_isDeleted)
            return Errors.AlreadyDeleted;

        _isDeleted = true;
        _deletedBy = deletedBy;
        _deletedAt = DateTimeOffset.UtcNow;
        _updatedBy = deletedBy;

        UpdateModificationTime();
        IncrementVersion();

        return Result.Deleted;
    }

    protected virtual void IncrementVersion()
    {
        Version++;
    }

    // Helper method to track all changes
    protected void TrackChange(string? updatedBy)
    {
        if (_createdBy is null)
        {
            SetCreatedBy(updatedBy);
        }
        else
        {
            SetUpdatedBy(updatedBy);
        }
    }
}
