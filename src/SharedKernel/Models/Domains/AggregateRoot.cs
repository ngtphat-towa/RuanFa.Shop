using ErrorOr;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.SharedKernel.Models.Domains;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
    #region Constructors
    protected AggregateRoot() : base() { }

    protected AggregateRoot(TId id) : base(id) { }
    #endregion

    #region Error Definitions
    public static class Errors
    {
        public static Error DomainEventNull => Error.Validation(
            code: "AggregateRoot.NullEvent",
            description: "Domain event cannot be null");
    }
    #endregion

    #region DomainEvents
    public new ErrorOr<Success> AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent is null)
            return Errors.DomainEventNull;
        
        base.AddDomainEvent(domainEvent);
        return Result.Success;
    }
    #endregion
}
