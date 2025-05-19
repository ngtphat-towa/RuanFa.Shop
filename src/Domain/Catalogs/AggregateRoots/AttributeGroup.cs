using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.AggregateRoots;

public class AttributeGroup : AggregateRoot<Guid>
{
    #region Properties
    public string Name { get; private set; } = null!;
    #endregion

    #region Relationships
    private readonly List<Product> _products = new();
    private readonly List<AttributeGroupAttribute> _attributeGroupAttributes = new();

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    public IReadOnlyCollection<AttributeGroupAttribute> AttributeGroupAttributes => _attributeGroupAttributes.AsReadOnly();
    #endregion

    #region Constructor
    private AttributeGroup()
    {
        // For EF Core
    }

    private AttributeGroup(string groupName)
    {
        Id = Guid.NewGuid();
        Name = groupName;
    }
    #endregion

    #region Factory
    public static ErrorOr<AttributeGroup> Create(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            return DomainErrors.AttributeGroup.EmptyName;
        }
        if (newName.Length < 3)
        {
            return DomainErrors.AttributeGroup.NameTooShort;
        }
        if (newName.Length > 100)
        {
            return DomainErrors.AttributeGroup.NameTooLong;
        }


        return new AttributeGroup(newName);
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> Update(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            return DomainErrors.AttributeGroup.EmptyName;
        }

        if (newName.Length < 3)
        {
            return DomainErrors.AttributeGroup.NameTooShort;
        }
        if (newName.Length > 100)
        {
            return DomainErrors.AttributeGroup.NameTooLong;
        }

        Name = newName;
        AddDomainEvent(new AttributeGroupNameUpdatedEvent(Id, newName));
        return Result.Updated;
    }

    public ErrorOr<AttributeGroupAttribute> AddAttribute(Guid attributeId)
    {
        if (attributeId == Guid.Empty)
        {
            return DomainErrors.AttributeGroupAttribute.InvalidAttributeId;
        }

        if (_attributeGroupAttributes.Any(aga => aga.AttributeId == attributeId))
        {
            return DomainErrors.AttributeGroupAttribute.DuplicateAttribute;
        }

        var attributeGroupAttributeResult = AttributeGroupAttribute.Create(Id, attributeId);
        if (attributeGroupAttributeResult.IsError)
        {
            return attributeGroupAttributeResult.Errors;
        }

        var attributeGroupAttribute = attributeGroupAttributeResult.Value;
        _attributeGroupAttributes.Add(attributeGroupAttribute);
        AddDomainEvent(new AttributeAddedToGroupEvent(Id, attributeId));
        return attributeGroupAttribute;
    }

    public ErrorOr<Updated> RemoveAttribute(Guid attributeId)
    {
        var attributeGroupAttribute = _attributeGroupAttributes.FirstOrDefault(aga => aga.AttributeId == attributeId);
        if (attributeGroupAttribute == null)
        {
            return DomainErrors.CatalogAttribute.NotFound();
        }

        _attributeGroupAttributes.Remove(attributeGroupAttribute);
        AddDomainEvent(new AttributeRemovedFromGroupEvent(Id, attributeId));
        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record AttributeGroupNameUpdatedEvent(Guid AttributeGroupId, string NewName) : IDomainEvent;

    public record AttributeAddedToGroupEvent(Guid AttributeGroupId, Guid AttributeId) : IDomainEvent;

    public record AttributeRemovedFromGroupEvent(Guid AttributeGroupId, Guid AttributeId) : IDomainEvent;
    #endregion
}
