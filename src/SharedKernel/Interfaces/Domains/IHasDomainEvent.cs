using ErrorOr;

namespace RuanFa.Shop.SharedKernel.Interfaces.Domains;

public interface IHasDomainEvent
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    ErrorOr<Success> ClearDomainEvents();

    ErrorOr<Success> AddDomainEvent(IDomainEvent domainEvent);
}
