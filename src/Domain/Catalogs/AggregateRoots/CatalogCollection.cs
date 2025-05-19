using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using System.Text.RegularExpressions;

namespace RuanFa.Shop.Domain.Catalogs.AggregateRoots;

public class CatalogCollection : AggregateRoot<Guid>
{
    #region Properties
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public CollectionType Type { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsFeatured { get; private set; }
    public int? DisplayOrder { get; private set; }
    #endregion

    #region Relationships
    private readonly List<ProductCollection> _productCollections = new();
    public IReadOnlyCollection<ProductCollection> ProductCollections => _productCollections.AsReadOnly();
    #endregion

    #region Constructor
    private CatalogCollection() { } // For EF Core

    private CatalogCollection(
        string name,
        string slug,
        CollectionType type,
        string? description,
        string? imageUrl,
        bool isActive,
        bool isFeatured,
        int? displayOrder)
    {
        Id = Guid.NewGuid();
        Name = name;
        Slug = slug;
        Type = type;
        Description = description;
        ImageUrl = imageUrl;
        IsActive = isActive;
        IsFeatured = isFeatured;
        DisplayOrder = displayOrder;
    }
    #endregion

    #region Factory
    public static ErrorOr<CatalogCollection> Create(
        string name,
        string slug,
        CollectionType type,
        string? description = null,
        string? imageUrl = null,
        bool isActive = true,
        bool isFeatured = false,
        int? displayOrder = null)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 100)
            return DomainErrors.CatalogCollection.InvalidName;

        if (string.IsNullOrWhiteSpace(slug) || slug.Length < 3 || slug.Length > 100 || !Regex.IsMatch(slug, @"^[a-z0-9-]+$"))
            return DomainErrors.CatalogCollection.InvalidSlug;

        if (!Enum.IsDefined(typeof(CollectionType), type))
            return DomainErrors.CatalogCollection.InvalidType;

        if (description != null && description.Length > 500)
            return DomainErrors.CatalogCollection.InvalidDescription;

        if (imageUrl != null && !Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            return DomainErrors.CatalogCollection.InvalidImageUrl;

        if (displayOrder.HasValue && displayOrder.Value < 0)
            return DomainErrors.CatalogCollection.InvalidDisplayOrder;

        var collection = new CatalogCollection(
            name: name,
            slug: slug,
            type: type,
            description: description,
            imageUrl: imageUrl,
            isActive: isActive,
            isFeatured: isFeatured,
            displayOrder: displayOrder);

        collection.AddDomainEvent(new CatalogCollectionCreatedEvent(collection.Id, name, slug, type));
        return collection;
    }
    #endregion

    #region Methods
    public ErrorOr<Updated> Update(
        string? name = null,
        string? slug = null,
        CollectionType? type = null,
        string? description = null,
        string? imageUrl = null,
        bool? isActive = null,
        bool? isFeatured = null,
        int? displayOrder = null)
    {
        if (name != null)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 100)
                return DomainErrors.CatalogCollection.InvalidName;
            Name = name;
        }

        if (slug != null)
        {
            if (string.IsNullOrWhiteSpace(slug) || slug.Length < 3 || slug.Length > 100 || !Regex.IsMatch(slug, @"^[a-z0-9-]+$"))
                return DomainErrors.CatalogCollection.InvalidSlug;
            Slug = slug;
        }

        if (type.HasValue)
        {
            if (!Enum.IsDefined(typeof(CollectionType), type.Value))
                return DomainErrors.CatalogCollection.InvalidType;
            Type = type.Value;
        }

        if (description != null)
        {
            if (description.Length > 500)
                return DomainErrors.CatalogCollection.InvalidDescription;
            Description = description;
        }

        if (imageUrl != null)
        {
            if (!string.IsNullOrEmpty(imageUrl) && !Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                return DomainErrors.CatalogCollection.InvalidImageUrl;
            ImageUrl = imageUrl;
        }

        if (isActive.HasValue)
        {
            IsActive = isActive.Value;
        }

        if (isFeatured.HasValue)
        {
            IsFeatured = isFeatured.Value;
        }

        if (displayOrder.HasValue)
        {
            if (displayOrder.Value < 0)
                return DomainErrors.CatalogCollection.InvalidDisplayOrder;
            DisplayOrder = displayOrder.Value;
        }

        AddDomainEvent(new CatalogCollectionUpdatedEvent(Id, Name, Slug, Type));
        return Result.Updated;
    }

    public ErrorOr<ProductCollection> AddProduct(Guid productId)
    {
        if (productId == Guid.Empty)
            return DomainErrors.Product.InvalidId;

        if (_productCollections.Any(pc => pc.ProductId == productId))
            return DomainErrors.ProductCollection.ProductAlreadyInCollection;

        var productCollectionResult = ProductCollection.Create(Id, productId);
        if (productCollectionResult.IsError)
            return productCollectionResult.Errors;

        _productCollections.Add(productCollectionResult.Value);
        AddDomainEvent(new ProductAddedToCollectionEvent(Id, productId));
        return productCollectionResult.Value;
    }

    public ErrorOr<Updated> RemoveProduct(Guid productId)
    {
        var productCollection = _productCollections.FirstOrDefault(pc => pc.ProductId == productId);
        if (productCollection == null)
            return DomainErrors.ProductCollection.ProductNotInCollection;

        _productCollections.Remove(productCollection);
        AddDomainEvent(new ProductRemovedFromCollectionEvent(Id, productId));
        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record CatalogCollectionCreatedEvent(Guid CollectionId, string Name, string Slug, CollectionType Type) : IDomainEvent;
    public record CatalogCollectionUpdatedEvent(Guid CollectionId, string Name, string Slug, CollectionType Type) : IDomainEvent;
    public record ProductAddedToCollectionEvent(Guid CollectionId, Guid ProductId) : IDomainEvent;
    public record ProductRemovedFromCollectionEvent(Guid CollectionId, Guid ProductId) : IDomainEvent;
    #endregion
}
