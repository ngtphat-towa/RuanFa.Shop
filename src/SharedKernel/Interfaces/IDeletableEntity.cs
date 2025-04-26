namespace RuanFa.Shop.SharedKernel.Interfaces;
public interface IDeletableEntity
{
    bool IsDeleted { get; set; }
    DateTimeOffset? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}
