using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.SharedKernel.Models.Domains;
using System.Text.RegularExpressions;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

namespace RuanFa.Shop.Domain.Catalogs.AggregateRoots;

public class CatalogAttribute : AggregateRoot<Guid>
{
    #region Properties
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public AttributeType Type { get; private set; } = AttributeType.None;
    public bool IsRequired { get; private set; }
    public bool DisplayOnFrontend { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsFilterable { get; private set; }
    #endregion

    #region Relationships
    private readonly List<AttributeOption> _attributeOptions = new();
    private readonly List<AttributeGroupAttribute> _attributeGroupAttributes = new();
    public IReadOnlyCollection<AttributeOption> AttributeOptions => _attributeOptions.AsReadOnly();
    public IReadOnlyCollection<AttributeGroupAttribute> AttributeGroupAttributes => _attributeGroupAttributes.AsReadOnly();
    #endregion

    #region Constructor
    private CatalogAttribute() { } // For EF Core

    private CatalogAttribute(
        string code,
        string name,
        AttributeType type,
        bool isRequired,
        bool displayOnFrontend,
        int sortOrder,
        bool isFilterable)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        Type = type;
        IsRequired = isRequired;
        DisplayOnFrontend = displayOnFrontend;
        SortOrder = sortOrder;
        IsFilterable = isFilterable;
    }
    #endregion

    #region Factory
    public static ErrorOr<CatalogAttribute> Create(
        string attributeCode,
        string attributeName,
        AttributeType type,
        bool isRequired = false,
        bool displayOnFrontend = false,
        int sortOrder = 0,
        bool isFilterable = false)
    {
        if (string.IsNullOrWhiteSpace(attributeCode))
            return DomainErrors.CatalogAttribute.EmptyCode;

        if (attributeCode.Length < 3)
            return DomainErrors.CatalogAttribute.CodeTooShort;

        if (attributeCode.Length > 50)
            return DomainErrors.CatalogAttribute.CodeTooLong;

        if (!Regex.IsMatch(attributeCode, @"^[a-zA-Z0-9\-_]+$"))
            return DomainErrors.CatalogAttribute.InvalidCodeFormat;

        if (string.IsNullOrWhiteSpace(attributeName))
            return DomainErrors.CatalogAttribute.EmptyName;

        if (attributeName.Length > 100)
            return DomainErrors.CatalogAttribute.NameTooLong;

        if (!Enum.IsDefined(typeof(AttributeType), type) || type == AttributeType.None)
            return DomainErrors.CatalogAttribute.InvalidType;

        if (sortOrder < 0)
            return DomainErrors.CatalogAttribute.InvalidSortOrder;

        var attribute = new CatalogAttribute(
            code: attributeCode,
            name: attributeName,
            type: type,
            isRequired: isRequired,
            displayOnFrontend: displayOnFrontend,
            sortOrder: sortOrder,
            isFilterable: isFilterable);

        attribute.AddDomainEvent(new CatalogAttributeCreatedEvent(attribute.Id, attributeCode, attributeName, type));
        return attribute;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> Update(
        string? attributeCode = null,
        string? attributeName = null,
        AttributeType? type = null,
        bool? isRequired = null,
        bool? displayOnFrontend = null,
        int? sortOrder = null,
        bool? isFilterable = null)
    {
        if (attributeCode != null)
        {
            if (string.IsNullOrWhiteSpace(attributeCode))
                return DomainErrors.CatalogAttribute.EmptyCode;

            if (attributeCode.Length < 3)
                return DomainErrors.CatalogAttribute.CodeTooShort;

            if (attributeCode.Length > 50)
                return DomainErrors.CatalogAttribute.CodeTooLong;

            if (!Regex.IsMatch(attributeCode, @"^[a-zA-Z0-9\-_]+$"))
                return DomainErrors.CatalogAttribute.InvalidCodeFormat;

            Code = attributeCode;
        }

        if (attributeName != null)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
                return DomainErrors.CatalogAttribute.EmptyName;

            if (attributeName.Length > 100)
                return DomainErrors.CatalogAttribute.NameTooLong;

            Name = attributeName;
        }

        if (type != null)
        {
            if (!Enum.IsDefined(typeof(AttributeType), type.Value) || type == AttributeType.None)
                return DomainErrors.CatalogAttribute.InvalidType;

            if (!_attributeOptions.Any() && (type == AttributeType.Dropdown || type == AttributeType.Swatch))
                return DomainErrors.CatalogAttribute.TypeRequiresOptions;

            Type = type.Value;
        }

        if (isRequired.HasValue)
            IsRequired = isRequired.Value;

        if (displayOnFrontend.HasValue)
            DisplayOnFrontend = displayOnFrontend.Value;

        if (sortOrder.HasValue)
        {
            if (sortOrder < 0)
                return DomainErrors.CatalogAttribute.InvalidSortOrder;

            SortOrder = sortOrder.Value;
        }

        if (isFilterable.HasValue)
            IsFilterable = isFilterable.Value;

        AddDomainEvent(new CatalogAttributeUpdatedEvent(Id, Code, Name, Type));
        return Result.Updated;
    }

    public ErrorOr<AttributeOption> AddOption(string optionText)
    {
        if ((Type != AttributeType.Dropdown && Type != AttributeType.Swatch) && _attributeOptions.Any())
            return DomainErrors.CatalogAttribute.OptionsNotSupportedForType;

        var optionResult = AttributeOption.Create(
            attributeId: Id,
            attributeCode: Code,
            optionText: optionText);

        if (optionResult.IsError)
            return optionResult.Errors;

        var option = optionResult.Value;
        _attributeOptions.Add(option);
        AddDomainEvent(new CatalogAttributeOptionAddedEvent(Id, option.Id, optionText));
        return option;
    }

    public ErrorOr<Updated> RemoveOption(Guid optionId)
    {
        var option = _attributeOptions.FirstOrDefault(o => o.Id == optionId);
        if (option == null)
            return DomainErrors.CatalogAttribute.OptionNotFound;

        _attributeOptions.Remove(option);
        AddDomainEvent(new CatalogAttributeOptionRemovedEvent(Id, optionId));
        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record CatalogAttributeCreatedEvent(Guid AttributeId, string Code, string Name, AttributeType Type) : IDomainEvent;

    public record CatalogAttributeUpdatedEvent(Guid AttributeId, string Code, string Name, AttributeType Type) : IDomainEvent;

    public record CatalogAttributeOptionAddedEvent(Guid AttributeId, Guid OptionId, string OptionText) : IDomainEvent;

    public record CatalogAttributeOptionRemovedEvent(Guid AttributeId, Guid OptionId) : IDomainEvent;
    #endregion
}
