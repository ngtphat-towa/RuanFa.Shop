using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class AttributeOption : Entity<Guid>
{
    #region Properties
    public string OptionText { get; private set; } = null!;
    #endregion

    #region Relationships
    public Guid AttributeId { get; private set; }
    public CatalogAttribute Attribute { get; private set; } = null!;
    private readonly List<VariantAttributeOption> _variantAttributeOptions = new();
    public IReadOnlyCollection<VariantAttributeOption> VariantAttributeOptions => _variantAttributeOptions.AsReadOnly();
    #endregion

    #region Constructors
    private AttributeOption() { } // EF Core

    private AttributeOption(Guid attributeId, string optionText)
    {
        Id = Guid.NewGuid();
        AttributeId = attributeId;
        OptionText = optionText.Trim();
    }
    #endregion

    #region Factory
    public static ErrorOr<AttributeOption> Create(
        Guid attributeId,
        string optionText)
    {
        if (attributeId == Guid.Empty)
            return DomainErrors.AttributeOption.InvalidAttributeId;

        if (string.IsNullOrWhiteSpace(optionText))
            return DomainErrors.AttributeOption.EmptyOptionText;

        if (optionText.Length > 100)
            return DomainErrors.AttributeOption.OptionTextTooLong;

        var option = new AttributeOption(attributeId, optionText);
        option.AddDomainEvent(new AttributeOptionCreatedEvent(option.Id, attributeId, optionText));
        return option;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> UpdateOptionText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return DomainErrors.AttributeOption.EmptyOptionText;

        if (newText.Length > 100)
            return DomainErrors.AttributeOption.OptionTextTooLong;

        OptionText = newText.Trim();
        AddDomainEvent(new AttributeOptionUpdatedEvent(Id, AttributeId, newText));
        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record AttributeOptionCreatedEvent(Guid OptionId, Guid AttributeId, string OptionText) : IDomainEvent;

    public record AttributeOptionUpdatedEvent(Guid OptionId, Guid AttributeId, string OptionText) : IDomainEvent;
    #endregion
}
