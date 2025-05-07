using ErrorOr;
using RuanFa.Shop.Domain.Attributes.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Attributes.Entities;

public class AttributeOption : Entity<Guid>
{
    #region Properties

    public string AttributeCode { get; private set; } = null!;
    public string OptionText { get; private set; } = null!;

    #endregion

    #region Relationships

    public Guid AttributeId { get; private set; }
    public CatalogAttribute Attribute { get; private set; } = null!;

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
            return DomainErrors.AttributeOption.InvalidAttributeCode;

        if (string.IsNullOrWhiteSpace(optionText))
            return DomainErrors.AttributeOption.EmptyText;

        return new AttributeOption(attributeId, attributeCode, optionText);
    }

    #endregion

    #region Methods

    public ErrorOr<Updated> UpdateOptionText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return DomainErrors.AttributeOption.EmptyText;

        OptionText = newText.Trim();
        return Result.Updated;
    }

    #endregion
}
