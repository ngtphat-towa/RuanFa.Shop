using ErrorOr;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Categorizations.Errors;
using RuanFa.Shop.Domain.Categorizations.ValueObjects;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Categorizations
{
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

        // Relationships
        public Guid? ParentId { get; private set; }
        public Category? Parent { get; private set; }
        public ICollection<Category> Children { get; private set; } = new List<Category>();
        public ICollection<ProductCategory> ProductCategories { get; private set; } = new List<ProductCategory>();
        #endregion

        #region Constructors
        private Category() { }

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
            // Validate name
            if (string.IsNullOrWhiteSpace(name))
                return DomainErrors.Category.EmptyName;

            if (name.Length < 3)
                return DomainErrors.Category.NameTooShort;

            // Validate URL key
            if (string.IsNullOrWhiteSpace(urlKey))
                return DomainErrors.Category.EmptyUrlKey;

            if (!Uri.IsWellFormedUriString(urlKey, UriKind.RelativeOrAbsolute))
                return DomainErrors.Category.InvalidUrlKey;

            // Validate parent reference
            if (parentId.HasValue && parentId.Value == id)
                return DomainErrors.Category.CircularReference;

            // Validate position
            if (position.HasValue && position < 0)
                return DomainErrors.Category.InvalidPosition;

            return new Category(
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
                parentId
            );
        }
        #endregion

        #region Operations
        public ErrorOr<Success> UpdateContent(
            string name,
            string urlKey,
            string? shortDescription,
            string? description,
            CategoryImage? image)
        {
            // Validate name
            if (string.IsNullOrWhiteSpace(name))
                return DomainErrors.Category.EmptyName;

            if (name.Length < 3)
                return DomainErrors.Category.NameTooShort;

            // Validate URL key
            if (string.IsNullOrWhiteSpace(urlKey))
                return DomainErrors.Category.EmptyUrlKey;

            if (!Uri.IsWellFormedUriString(urlKey, UriKind.RelativeOrAbsolute))
                return DomainErrors.Category.InvalidUrlKey;

            Name = name;
            UrlKey = urlKey;
            ShortDescription = shortDescription;
            Description = description;
            Image = image;

            return Result.Success;
        }

        public ErrorOr<Success> UpdateSeoData(
            string? metaTitle,
            string? metaKeywords,
            string? metaDescription)
        {
            // Basic validation if needed
            if (metaTitle != null && metaTitle.Length > 60)
                return DomainErrors.Category.MetaTitleTooLong;

            if (metaDescription != null && metaDescription.Length > 160)
                return DomainErrors.Category.MetaDescriptionTooLong;

            MetaTitle = metaTitle;
            MetaKeywords = metaKeywords;
            MetaDescription = metaDescription;

            return Result.Success;
        }

        public ErrorOr<Success> SetParent(Guid? parentId)
        {
            if (parentId.HasValue && parentId.Value == Id)
                return DomainErrors.Category.CircularReference;

            ParentId = parentId;
            return Result.Success;
        }

        public ErrorOr<Success> UpdateDisplaySettings(
            bool includeInNav,
            short? position,
            bool showProducts)
        {
            if (position.HasValue && position < 0)
                return DomainErrors.Category.InvalidPosition;

            IncludeInNav = includeInNav;
            Position = position;
            ShowProducts = showProducts;

            return Result.Success;
        }

        public ErrorOr<Success> SetActiveStatus(bool active)
        {
            IsActive = active;
            return Result.Success;
        }

        public ErrorOr<Success> AddProduct(Guid productId)
        {
            if (!IsActive)
                return DomainErrors.ProductCategory.InactiveCategory;

            if (ProductCategories.Any(pc => pc.ProductId == productId))
                return DomainErrors.ProductCategory.DuplicateLink;

            var productCategory = ProductCategory.Create( Id, productId);
            if (productCategory.IsError)
                return productCategory.Errors;

            ProductCategories.Add(productCategory.Value);
            return Result.Success;
        }

        public ErrorOr<Success> RemoveProduct(Guid productId)
        {
            var productCategory = ProductCategories.FirstOrDefault(pc => pc.ProductId == productId);

            if (productCategory == null)
                return DomainErrors.ProductCategory.LinkNotFound;

            ProductCategories.Remove(productCategory);
            return Result.Success;
        }
        #endregion
    }
}
