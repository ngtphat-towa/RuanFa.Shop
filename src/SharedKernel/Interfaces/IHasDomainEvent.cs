using ErrorOr;

namespace RuanFa.Shop.SharedKernel.Interfaces;
public interface IHasDomainEvent
{
   public IReadOnlyList<IDomainEvent> DomainEvents { get; }
   public ErrorOr<Success> ClearDomainEvents();
   public ErrorOr<Success> AddDomainEvent(IDomainEvent domainEvent);
}
