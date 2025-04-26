using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces;

namespace RuanFa.Shop.SharedKernel.Models;

public abstract class Entity<TId> :
    IAuditableEntity,
    IHasDomainEvent,
    IEquatable<Entity<TId>>
    where TId : notnull
{
    #region Private Fields
    private readonly List<IDomainEvent> _domainEvents = [];
    #endregion

    #region Public Properties
    public TId Id { get; protected set; } = default!;

    // Auditing
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Domain Events
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    #endregion

    #region Constructors
    protected Entity()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }

    protected Entity(TId id) : this()
    {
        Id = id;
    }
    #endregion

    #region Errors
    public static class Errors
    {
        public static Error DomainEventNull => Error.Validation(
            code: "Entity.DomainEventNull",
            description: "Domain event cannot be null.");

        public static Error InvalidModification => Error.Validation(
            code: "Entity.InvalidModification",
            description: "Cannot modify the entity in its current state.");
    }
    #endregion

    // in Entity.cs, update the ClearDomainEvents method
    #region Domain Event Methods
    public ErrorOr<Success> AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent is null)
            return Errors.DomainEventNull;

        if (!CanModify())
            return Errors.InvalidModification;

        _domainEvents.Add(domainEvent);
        OnStateChanged();
        return Result.Success;
    }

    public ErrorOr<Success> ClearDomainEvents()
    {
        if (!CanModify())
            return Errors.InvalidModification;

        _domainEvents.Clear();
        OnStateChanged();
        return Result.Success;
    }
    #endregion


    #region Auditing Methods
    public ErrorOr<Updated> SetCreatedBy(string? creator)
    {
        if (!CanModify())
            return Errors.InvalidModification;

        if (CreatedBy != creator)
        {
            CreatedBy = creator;
            OnStateChanged();
        }

        return Result.Updated;
    }

    public ErrorOr<Updated> SetUpdatedBy(string? updater)
    {
        if (!CanModify())
            return Errors.InvalidModification;

        if (UpdatedBy != updater)
        {
            UpdatedBy = updater;
            OnStateChanged();
        }

        return Result.Updated;
    }
    #endregion

    #region State Management
    protected virtual bool CanModify() => true;

    protected virtual void OnStateChanged()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    protected virtual ErrorOr<Updated> UpdateModificationTime()
    {
        if (!CanModify())
            return Errors.InvalidModification;

        OnStateChanged();
        return Result.Updated;
    }
    #endregion

    #region Equality Members
    public virtual bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return GetType() == other.GetType() && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        => !(left == right);
    #endregion
}
