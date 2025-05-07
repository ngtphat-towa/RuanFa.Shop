using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.ValueObjects;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;
using System.Text.RegularExpressions;
using RuanFa.Shop.SharedKernel.Interfaces.Domains;

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
    public string? ShortDescription { get; private set; }
    public string? Description { get; private set; }
    public string? MetaTitle { get; private set; }
    public string? MetaKeywords { get; private set; }
    public string? MetaDescription { get; private set; }
    #endregion

    #region Relationships
    public Guid? ParentId { get; private set; }
    public Category? Parent { get; private set; }
    private readonly List<Category> _children = new();
    private readonly List<ProductCategory> _productCategories = new();
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();
    public IReadOnlyCollection<ProductCategory> ProductCategories => _productCategories.AsReadOnly();
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
        string? shortDescription,
        string? description,
        CategoryImage? image,
        string? metaTitle,
        string? metaKeywords,
        string? metaDescription,
        Guid? parentId)
    {
        Id = id;
        Name = name;
        UrlKey = urlKey;
        IsActive = isActive;
        IncludeInNav = includeInNav;
        Position = position;
        ShowProducts = showProducts;
        ShortDescription = shortDescription;
        Description = description;
        Image = image;
        MetaTitle = metaTitle;
        MetaKeywords = metaKeywords;
        MetaDescription = metaDescription;
        ParentId = parentId;
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
        string? shortDescription,
        string? description,
        CategoryImage? image,
        string? metaTitle,
        string? metaKeywords,
        string? metaDescription,
        Guid? parentId = null)
    {
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

        if (shortDescription?.Length > 500)
            return DomainErrors.Category.ShortDescriptionTooLong;

        if (description?.Length > 2000)
            return DomainErrors.Category.DescriptionTooLong;

        if (metaTitle?.Length > 60)
            return DomainErrors.Category.MetaTitleTooLong;

        if (metaKeywords?.Length > 255)
            return DomainErrors.Category.MetaKeywordsTooLong;

        if (metaDescription?.Length > 160)
            return DomainErrors.Category.MetaDescriptionTooLong;

        if (image != null)
        {
            if (string.IsNullOrWhiteSpace(image.Url) || string.IsNullOrWhiteSpace(image.Alt))
                return DomainErrors.Category.InvalidImage;
        }

        var category = new Category(
            id,
            name,
            urlKey,
            isActive,
            includeInNav,
            position,
            showProducts,
            shortDescription,
            description,
            image,
            metaTitle,
            metaKeywords,
            metaDescription,
            parentId);

        category.AddDomainEvent(new CategoryCreatedEvent(id, name, urlKey, parentId));
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
        string? shortDescription = null,
        string? description = null,
        CategoryImage? image = null,
        string? metaTitle = null,
        string? metaKeywords = null,
        string? metaDescription = null,
        Guid? parentId = null)
    {
        if (name != null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DomainErrors.Category.EmptyName;

            if (name.Length < 3)
                return DomainErrors.Category.NameTooShort;

            if (name.Length > 100)
                return DomainErrors.Category.NameTooLong;

            Name = name;
        }

        if (urlKey != null)
        {
            if (string.IsNullOrWhiteSpace(urlKey))
                return DomainErrors.Category.EmptyUrlKey;

            if (!Regex.IsMatch(urlKey, @"^[a-z0-9\-]+$"))
                return DomainErrors.Category.InvalidUrlKeyFormat;

            if (urlKey.Length > 255)
                return DomainErrors.Category.UrlKeyTooLong;

            UrlKey = urlKey;
        }

        if (isActive.HasValue)
            IsActive = isActive.Value;

        if (includeInNav.HasValue)
            IncludeInNav = includeInNav.Value;

        if (position.HasValue)
        {
            if (position < 0)
                return DomainErrors.Category.InvalidPosition;

            Position = position;
        }

        if (showProducts.HasValue)
            ShowProducts = showProducts.Value;

        if (shortDescription != null)
        {
            if (shortDescription.Length > 500)
                return DomainErrors.Category.ShortDescriptionTooLong;

            ShortDescription = shortDescription;
        }

        if (description != null)
        {
            if (description.Length > 2000)
                return DomainErrors.Category.DescriptionTooLong;

            Description = description;
        }

        if (image != null)
        {
            if (string.IsNullOrWhiteSpace(image.Url) || string.IsNullOrWhiteSpace(image.Alt))
                return DomainErrors.Category.InvalidImage;

            Image = image;
        }

        if (metaTitle != null)
        {
            if (metaTitle.Length > 60)
                return DomainErrors.Category.MetaTitleTooLong;

            MetaTitle = metaTitle;
        }

        if (metaKeywords != null)
        {
            if (metaKeywords.Length > 255)
                return DomainErrors.Category.MetaKeywordsTooLong;

            MetaKeywords = metaKeywords;
        }

        if (metaDescription != null)
        {
            if (metaDescription.Length > 160)
                return DomainErrors.Category.MetaDescriptionTooLong;

            MetaDescription = metaDescription;
        }

        if (parentId != null)
        {
            if (parentId.Value == Id)
                return DomainErrors.Category.CircularReference;

            ParentId = parentId;
        }

        AddDomainEvent(new CategoryUpdatedEvent(Id, Name, UrlKey, ParentId));
        return Result.Updated;
    }

    public ErrorOr<Success> AddProduct(Guid productId)
    {
        if (!IsActive)
            return DomainErrors.ProductCategory.InactiveCategory;

        if (productId == Guid.Empty)
            return DomainErrors.ProductCategory.InvalidProductId;

        if (ProductCategories.Any(pc => pc.ProductId == productId))
            return DomainErrors.ProductCategory.DuplicateCategory;

        var productCategoryResult = ProductCategory.Create(Id, productId);
        if (productCategoryResult.IsError)
            return productCategoryResult.Errors;

        _productCategories.Add(productCategoryResult.Value);
        AddDomainEvent(new CategoryProductAddedEvent(Id, productId));
        return Result.Success;
    }

    public ErrorOr<Success> RemoveProduct(Guid productId)
    {
        var productCategory = _productCategories.FirstOrDefault(pc => pc.ProductId == productId);
        if (productCategory == null)
            return DomainErrors.ProductCategory.CategoryNotFound;

        _productCategories.Remove(productCategory);
        AddDomainEvent(new CategoryProductRemovedEvent(Id, productId));
        return Result.Success;
    }
    #endregion

    #region Domain Events
    public record CategoryCreatedEvent(Guid CategoryId, string Name, string UrlKey, Guid? ParentId) : IDomainEvent;

    public record CategoryUpdatedEvent(Guid CategoryId, string Name, string UrlKey, Guid? ParentId) : IDomainEvent;

    public record CategoryProductAddedEvent(Guid CategoryId, Guid ProductId) : IDomainEvent;

    public record CategoryProductRemovedEvent(Guid CategoryId, Guid ProductId) : IDomainEvent;
    #endregion
}
