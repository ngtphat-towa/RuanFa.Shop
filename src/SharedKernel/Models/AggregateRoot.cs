using ErrorOr;
using RuanFa.Shop.SharedKernel.Errors;
using RuanFa.Shop.SharedKernel.Interfaces;

namespace RuanFa.Shop.SharedKernel.Models;

public abstract class AggregateRoot<TId> :
    Entity<TId>,
    IDeletableEntity,
    IVersionable
    where TId : notnull
{
    #region Private Fields
    private bool _isDeleted;
    private string? _deletedBy;
    private DateTimeOffset? _deletedAt;
    private int _version = 1;
    #endregion

    #region Constructors
    protected AggregateRoot() : base() { }

    protected AggregateRoot(TId id) : base(id) { }
    #endregion

    #region Public Properties
    // IDeletableEntity implementation
    public bool IsDeleted
    {
        get => _isDeleted;
        set
        {
            var result = SetDeletedState(value);
            if (result.IsError)
                throw new InvalidOperationException($"Failed to set deleted state: {result.FirstError.Description}");
        }
    }

    public DateTimeOffset? DeletedAt
    {
        get => _deletedAt;
        set
        {
            var result = SetDeletedAt(value);
            if (result.IsError)
                throw new InvalidOperationException($"Failed to set deleted at: {result.FirstError.Description}");
        }
    }

    public string? DeletedBy
    {
        get => _deletedBy;
        set
        {
            var result = SetDeletedBy(value);
            if (result.IsError)
                throw new InvalidOperationException($"Failed to set deleted by: {result.FirstError.Description}");
        }
    }

    // IVersionable implementation
    public int Version
    {
        get => _version;
        set
        {
            var result = SetVersion(value);
            if (result.IsError)
                throw new InvalidOperationException($"Failed to set version: {result.FirstError.Description}");
        }
    }
    #endregion

    #region Public Methods
    public new ErrorOr<Success> AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent is null)
            return Entity<TId>.Errors.DomainEventNull;

        if (!CanModify())
            return Entity<TId>.Errors.InvalidModification;

        var result = base.AddDomainEvent(domainEvent);
        if (result.IsError)
            return result.FirstError;

        IncrementVersion();
        return Result.Success;
    }

    public ErrorOr<Deleted> Delete(string? deletedBy)
    {
        if (_isDeleted)
            return DomainErrors.AggregateRoot.AlreadyDeleted;

        if (!CanModify())
            return Entity<TId>.Errors.InvalidModification;

        var setDeletedResult = SetDeletedState(true);
        if (setDeletedResult.IsError)
            return setDeletedResult.FirstError;

        var setDeletedByResult = SetDeletedBy(deletedBy);
        if (setDeletedByResult.IsError)
            return setDeletedByResult.FirstError;

        var setDeletedAtResult = SetDeletedAt(DateTimeOffset.UtcNow);
        if (setDeletedAtResult.IsError)
            return setDeletedAtResult.FirstError;

        var updateResult = SetUpdatedBy(deletedBy);
        if (updateResult.IsError)
            return updateResult.FirstError;

        return Result.Deleted;
    }

    public new ErrorOr<Success> ClearDomainEvents()
    {
        if (!CanModify())
            return Entity<TId>.Errors.InvalidModification;

        var result = base.ClearDomainEvents();
        if (result.IsError)
            return result.FirstError;

        IncrementVersion();
        return Result.Success;
    }
    #endregion

    #region Protected Methods
    protected override bool CanModify() => !_isDeleted && base.CanModify();

    protected override void OnStateChanged()
    {
        base.OnStateChanged();
        IncrementVersion();
    }

    protected ErrorOr<Updated> SetDeletedState(bool value)
    {
        if (!CanModify() && !value)
            return Entity<TId>.Errors.InvalidModification;

        if (_isDeleted != value)
        {
            _isDeleted = value;
            OnStateChanged();
        }

        return Result.Updated;
    }

    protected ErrorOr<Updated> SetDeletedAt(DateTimeOffset? value)
    {
        if (!CanModify())
            return Entity<TId>.Errors.InvalidModification;

        if (_deletedAt != value)
        {
            _deletedAt = value;
            OnStateChanged();
        }

        return Result.Updated;
    }

    protected ErrorOr<Updated> SetDeletedBy(string? value)
    {
        if (!CanModify())
            return Entity<TId>.Errors.InvalidModification;

        if (_deletedBy != value)
        {
            _deletedBy = value;
            OnStateChanged();
        }

        return Result.Updated;
    }

    protected ErrorOr<Updated> SetVersion(int value)
    {
        if (!CanModify())
            return Entity<TId>.Errors.InvalidModification;

        if (value <= _version)
            return Error.Validation("Version.InvalidValue", "New version must be greater than current version");

        _version = value;
        OnStateChanged();
        return Result.Updated;
    }

    protected virtual void IncrementVersion()
    {
        var result = SetVersion(_version + 1);
        if (result.IsError)
        {
            throw new InvalidOperationException($"Failed to increment version: {result.FirstError.Description}");
        }
        _version++;
    }
    #endregion
}
