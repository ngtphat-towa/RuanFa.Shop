using System.Text.RegularExpressions;
using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;
using RuanFa.Shop.Domain.Commons.ValueObjects;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Catalogs.AggregateRoots;

public class Category : AggregateRoot<Guid>
{
    #region Properties
    public string Name { get; private set; } = null!;
    public string UrlKey { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public bool IncludeInNav { get; private set; }
    public short? Position { get; private set; }
    public bool ShowProducts { get; private set; }

    public CategoryImage? Image { get; private set; }
    public DescriptionData? ShortDescription { get; private set; }
    public DescriptionData? Description { get; private set; }
    public SeoMeta? SeoMeta { get; private set; }
    #endregion

    #region Relationships
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; }

    private readonly List<Category> _children = new();
    private readonly List<Product> _products = new();

    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
    #endregion

    #region Constructors
    private Category() { } // For EF Core

    private Category(
        Guid id,
        string name,
        string urlKey,
        bool isActive,
        bool includeInNav,
        short? position,
        bool showProducts,
        Guid? parentId = null,
        CategoryImage? image = null,
        DescriptionData? shortDesc = null,
        DescriptionData? description = null,
        SeoMeta? seoMeta = null)
    {
        Id = id;
        Name = name;
        UrlKey = urlKey;
        IsActive = isActive;
        IncludeInNav = includeInNav;
        Position = position;
        ShowProducts = showProducts;
        ParentId = parentId;
        Image = image;
        ShortDescription = shortDesc;
        Description = description;
        SeoMeta = seoMeta;
    }
    #endregion

    #region Factory
    public static ErrorOr<Category> Create(
        Guid id,
        string name,
        string urlKey,
        bool isActive,
        bool includeInNav,
        short? position,
        bool showProducts,
        Guid? parentId = null,
        CategoryImage? image = null,
        DescriptionData? shortDesc = null,
        DescriptionData? description = null,
        SeoMeta? seoMeta = null)
    {
        // --- Basic validation ---
        if (id == Guid.Empty)
            return DomainErrors.Category.InvalidId;

        if (string.IsNullOrWhiteSpace(name))
            return DomainErrors.Category.EmptyName;

        if (name.Length < 3)
            return DomainErrors.Category.NameTooShort;

        if (name.Length > 100)
            return DomainErrors.Category.NameTooLong;

        if (string.IsNullOrWhiteSpace(urlKey))
            return DomainErrors.Category.EmptyUrlKey;

        if (!Regex.IsMatch(urlKey, @"^[a-z0-9\-]+$"))
            return DomainErrors.Category.InvalidUrlKeyFormat;

        if (urlKey.Length > 255)
            return DomainErrors.Category.UrlKeyTooLong;

        if (parentId.HasValue && parentId.Value == id)
            return DomainErrors.Category.CircularReference;

        if (position.HasValue && position < 0)
            return DomainErrors.Category.InvalidPosition;

        // --- All optional VOs (image, descriptions, seoMeta) are assumed pre-validated ---
        var category = new Category(
            id,
            name,
            urlKey,
            isActive,
            includeInNav,
            position,
            showProducts,
            parentId,
            image,
            shortDesc,
            description,
            seoMeta);

        category.AddDomainEvent(new CategoryCreatedEvent(
            category.Id,
            category.Name,
            category.UrlKey,
            category.ParentId));

        return category;
    }
    #endregion

    #region Operations
    public ErrorOr<Updated> Update(
        string? name = null,
        string? urlKey = null,
        bool? isActive = null,
        bool? includeInNav = null,
        short? position = null,
        bool? showProducts = null,
        Guid? parentId = null,
        CategoryImage? image = null,
        DescriptionData? shortDesc = null,
        DescriptionData? description = null,
        SeoMeta? seoMeta = null)
    {
        // --- Name & UrlKey ---
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DomainErrors.Category.EmptyName;
            if (name.Length < 3)
                return DomainErrors.Category.NameTooShort;
            if (name.Length > 100)
                return DomainErrors.Category.NameTooLong;
            Name = name;
        }

        if (urlKey is not null)
        {
            if (string.IsNullOrWhiteSpace(urlKey))
                return DomainErrors.Category.EmptyUrlKey;
            if (!Regex.IsMatch(urlKey, @"^[a-z0-9\-]+$"))
                return DomainErrors.Category.InvalidUrlKeyFormat;
            if (urlKey.Length > 255)
                return DomainErrors.Category.UrlKeyTooLong;
            UrlKey = urlKey;
        }

        // --- Other simple flags ---
        if (isActive.HasValue) IsActive = isActive.Value;
        if (includeInNav.HasValue) IncludeInNav = includeInNav.Value;
        if (showProducts.HasValue) ShowProducts = showProducts.Value;

        if (position.HasValue)
        {
            if (position < 0)
                return DomainErrors.Category.InvalidPosition;
            Position = position;
        }

        // --- Optional VOs ---
        if (image is not null) Image = image;
        if (shortDesc is not null) ShortDescription = shortDesc;
        if (description is not null) Description = description;
        if (seoMeta is not null) SeoMeta = seoMeta;

        // --- Parent linkage ---
        if (parentId.HasValue)
        {
            if (parentId.Value == Id)
                return DomainErrors.Category.CircularReference;
            ParentId = parentId;
        }

        AddDomainEvent(new CategoryUpdatedEvent(
            Id,
            Name,
            UrlKey,
            ParentId));

        return Result.Updated;
    }
    #endregion

    #region Domain Events
    public record CategoryCreatedEvent(Guid CategoryId, string Name, string UrlKey, Guid? ParentId)
        : IDomainEvent;

    public record CategoryUpdatedEvent(Guid CategoryId, string Name, string UrlKey, Guid? ParentId)
        : IDomainEvent;
    #endregion
}
