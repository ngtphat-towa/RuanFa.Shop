namespace RuanFa.Shop.SharedKernel.Interfaces;
public interface IHasDomainEvent
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();

    void AddDomainEvent(IDomainEvent domainEvent);
}
