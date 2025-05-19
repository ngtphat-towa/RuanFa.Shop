using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.Entities;

public class VariantAttributeValue : Entity<Guid>
{
    #region Properties
    public Guid VariantId { get; private set; }
    public Guid AttributeId { get; private set; }
    public Guid? AttributeOptionId { get; private set; }
    public string? Value { get; private set; }
    #endregion

    #region Relationships
    public ProductVariant Variant { get; private set; } = null!;
    public CatalogAttribute Attribute { get; private set; } = null!;
    public AttributeOption? AttributeOption { get; private set; }
    #endregion

    #region Constructor
    private VariantAttributeValue() { } // For EF Core

    private VariantAttributeValue(Guid variantId, Guid attributeId, Guid? attributeOptionId, string? value, AttributeType attributeType)
    {
        Id = Guid.NewGuid();
        VariantId = variantId;
        AttributeId = attributeId;
        AttributeOptionId = attributeOptionId;
        Value = value;
        Validate(attributeType);
    }
    #endregion

    #region Factory
    public static ErrorOr<VariantAttributeValue> Create(
        Guid variantId,
        Guid attributeId,
        Guid? attributeOptionId,
        string? value,
        AttributeType attributeType)
    {
        if (variantId == Guid.Empty)
            return DomainErrors.VariantAttributeValue.InvalidVariantId;

        if (attributeId == Guid.Empty)
            return DomainErrors.VariantAttributeValue.InvalidAttributeId;

        var variantAttributeValue = new VariantAttributeValue(variantId, attributeId, attributeOptionId, value, attributeType);
        var validationResult = variantAttributeValue.Validate(attributeType);
        if (validationResult.IsError)
            return validationResult.Errors;

        variantAttributeValue.AddDomainEvent(new VariantAttributeValueCreatedEvent(
            variantAttributeValue.Id,
            variantId,
            attributeId,
            attributeOptionId,
            value));
        return variantAttributeValue;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> Update(Guid? attributeOptionId, string? value, AttributeType attributeType)
    {
        AttributeOptionId = attributeOptionId;
        Value = value;
        var validationResult = Validate(attributeType);
        if (validationResult.IsError)
            return validationResult.Errors;

        AddDomainEvent(new VariantAttributeValueUpdatedEvent(Id, VariantId, AttributeId, attributeOptionId, value));
        return Result.Updated;
    }

    private ErrorOr<Updated> Validate(AttributeType attributeType)
    {
        if (attributeType == AttributeType.Dropdown || attributeType == AttributeType.Select || attributeType == AttributeType.Swatch)
        {
            if (!AttributeOptionId.HasValue || AttributeOptionId == Guid.Empty)
                return DomainErrors.VariantAttributeValue.InvalidAttributeOptionId;
            if (!string.IsNullOrEmpty(Value))
                return DomainErrors.VariantAttributeValue.ValueNotSupportedForOptionType;
        }
        else
        {
            if (AttributeOptionId.HasValue)
                return DomainErrors.VariantAttributeValue.OptionNotSupportedForValueType;
            if (string.IsNullOrEmpty(Value))
                return DomainErrors.VariantAttributeValue.InvalidValue;

            switch (attributeType)
            {
                case AttributeType.Text:
                case AttributeType.Checkbox:
                    break;
                case AttributeType.Number:
                    if (!int.TryParse(Value, out _))
                        return DomainErrors.VariantAttributeValue.InvalidNumberFormat;
                    break;
                case AttributeType.Boolean:
                    if (!bool.TryParse(Value, out _))
                        return DomainErrors.VariantAttributeValue.InvalidBooleanFormat;
                    break;
                case AttributeType.DateTime:
                    if (!DateTime.TryParse(Value, out _))
                        return DomainErrors.VariantAttributeValue.InvalidDateTimeFormat;
                    break;
                case AttributeType.Decimal:
                    if (!decimal.TryParse(Value, out _))
                        return DomainErrors.VariantAttributeValue.InvalidDecimalFormat;
                    break;
                default:
                    return DomainErrors.VariantAttributeValue.InvalidAttributeType;
            }
        }
        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record VariantAttributeValueCreatedEvent(Guid Id, Guid VariantId, Guid AttributeId, Guid? AttributeOptionId, string? Value) : IDomainEvent;
    public record VariantAttributeValueUpdatedEvent(Guid Id, Guid VariantId, Guid AttributeId, Guid? AttributeOptionId, string? Value) : IDomainEvent;
    public record VariantAttributeValueRemovedEvent(Guid Id, Guid VariantId, Guid AttributeId) : IDomainEvent;
    #endregion
}
