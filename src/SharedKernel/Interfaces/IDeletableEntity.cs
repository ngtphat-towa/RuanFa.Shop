namespace RuanFa.Shop.SharedKernel.Interfaces;
public interface IDeletableEntity
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    string? DeletedBy { get; }
}
