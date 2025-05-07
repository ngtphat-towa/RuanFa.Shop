using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;
using System.Text.RegularExpressions;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class AttributeOption : Entity<Guid>
{
    #region Properties
    public string AttributeCode { get; private set; } = null!;
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

    private AttributeOption(Guid attributeId, string attributeCode, string optionText)
    {
        Id = Guid.NewGuid();
        AttributeId = attributeId;
        AttributeCode = attributeCode.Trim();
        OptionText = optionText.Trim();
    }
    #endregion

    #region Factory
    public static ErrorOr<AttributeOption> Create(
        Guid attributeId,
        string attributeCode,
        string optionText)
    {
        if (attributeId == Guid.Empty)
            return DomainErrors.AttributeOption.InvalidAttributeId;

        if (string.IsNullOrWhiteSpace(attributeCode))
            return DomainErrors.AttributeOption.EmptyAttributeCode;

        if (attributeCode.Length < 3)
            return DomainErrors.AttributeOption.AttributeCodeTooShort;

        if (attributeCode.Length > 50)
            return DomainErrors.AttributeOption.AttributeCodeTooLong;

        if (!Regex.IsMatch(attributeCode, @"^[a-zA-Z0-9\-_]+$"))
            return DomainErrors.AttributeOption.InvalidAttributeCodeFormat;

        if (string.IsNullOrWhiteSpace(optionText))
            return DomainErrors.AttributeOption.EmptyOptionText;

        if (optionText.Length > 100)
            return DomainErrors.AttributeOption.OptionTextTooLong;

        var option = new AttributeOption(attributeId, attributeCode, optionText);
        option.AddDomainEvent(new AttributeOptionCreatedEvent(option.Id, attributeId, attributeCode, optionText));
        return option;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> UpdateAttributeCode(string newCode)
    {
        if (string.IsNullOrWhiteSpace(newCode))
            return DomainErrors.AttributeOption.EmptyAttributeCode;

        if (newCode.Length < 3)
            return DomainErrors.AttributeOption.AttributeCodeTooShort;

        if (newCode.Length > 50)
            return DomainErrors.AttributeOption.AttributeCodeTooLong;

        if (!Regex.IsMatch(newCode, @"^[a-zA-Z0-9\-_]+$"))
            return DomainErrors.AttributeOption.InvalidAttributeCodeFormat;

        AttributeCode = newCode.Trim();
        AddDomainEvent(new AttributeOptionUpdatedEvent(Id, AttributeId, newCode, OptionText));
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateOptionText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return DomainErrors.AttributeOption.EmptyOptionText;

        if (newText.Length > 100)
            return DomainErrors.AttributeOption.OptionTextTooLong;

        OptionText = newText.Trim();
        AddDomainEvent(new AttributeOptionUpdatedEvent(Id, AttributeId, AttributeCode, newText));
        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record AttributeOptionCreatedEvent(Guid OptionId, Guid AttributeId, string AttributeCode, string OptionText) : IDomainEvent;

    public record AttributeOptionUpdatedEvent(Guid OptionId, Guid AttributeId, string AttributeCode, string OptionText) : IDomainEvent;
    #endregion
}
