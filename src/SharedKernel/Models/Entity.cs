using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;

namespace RuanFa.Shop.SharedKernel.Models;

public abstract class Entity<TId> : IAuditable, IHasDomainEvent, IEquatable<Entity<TId>> where TId : notnull
{
    #region Fields
    private readonly List<IDomainEvent> _domainEvents = [];
    #endregion

    #region Properties
    public TId Id { get; protected set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    #endregion

    #region Constructors
    protected Entity()
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }
    #endregion

    #region Domain Events
    public ErrorOr<Success> AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent is null)
            return Error.Validation("DomainEvent.Null", "Domain event cannot be null");

        _domainEvents.Add(domainEvent);
        return Result.Success;
    }

    public ErrorOr<Success> ClearDomainEvents()
    {
        _domainEvents.Clear();
        return Result.Success;
    }
    #endregion

    #region Equality
    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return GetType() == other.GetType() && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals(obj as Entity<TId>);
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => left?.Equals(right) ?? right is null;
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
    #endregion
}
