using RuanFa.Shop.Domain.Accounts.Entities;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Domain.Accounts.Events;

public record UserProfileCreatedEvent(UserProfile Profile) : IDomainEvent;
public record UserProfileUpdatedEvent(UserProfile Profile) : IDomainEvent;
