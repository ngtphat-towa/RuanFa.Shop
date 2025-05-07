using ErrorOr;
using RuanFa.Shop.Domain.Attributes.Entities;
using RuanFa.Shop.Domain.Attributes.Enums;
using RuanFa.Shop.Domain.Attributes.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Attributes;

public class CatalogAttribute : AggregateRoot<Guid>
{
    #region Properties

    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public AttributeType Type { get; private set; } = AttributeType.None!;
    public bool IsRequired { get; private set; }
    public bool DisplayOnFrontend { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsFilterable { get; private set; }
    #endregion

    #region Relationships
    public ICollection<AttributeOption> AttributeOptions { get; private set; } = new List<AttributeOption>();
    public ICollection<AttributeGroupAttribute> AttributeGroupAttributes { get; private set; } = new List<AttributeGroupAttribute>();

    #endregion

    #region Constructor

    private CatalogAttribute()
    {
        // Required by EF Core
    }

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
        // Validations
        if (string.IsNullOrWhiteSpace(attributeCode))
            return DomainErrors.CatalogAttribute.InvalidCode;

        if (attributeCode.Length > 50)
            return DomainErrors.CatalogAttribute.CodeTooLong;

        if (string.IsNullOrWhiteSpace(attributeName))
            return DomainErrors.CatalogAttribute.InvalidName;

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

        return attribute;
    }


    #endregion

    #region Methods

    public ErrorOr<Updated> Update(
      string? attributeName = null,
      AttributeType? type = null,
      bool? isRequired = null,
      bool? displayOnFrontend = null,
      int? sortOrder = null,
      bool? isFilterable = null)
    {
        if (attributeName is not null)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
                return DomainErrors.CatalogAttribute.InvalidName;

            if (attributeName.Length > 100)
                return DomainErrors.CatalogAttribute.NameTooLong;

            Name = attributeName;
        }

        if (type is not null)
        {
            if (!Enum.IsDefined(typeof(AttributeType), type.Value) || type == AttributeType.None)
                return DomainErrors.CatalogAttribute.InvalidType;

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

        return Result.Updated;
    }


    #endregion
}
