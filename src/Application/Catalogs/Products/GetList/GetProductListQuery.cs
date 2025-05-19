using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Products.Models.Products;
using RuanFa.Shop.Application.Catalogs.Products.Models.QueryParams;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Catalogs.Products.GetList;
public record GetProductListQuery :
    ProductListQueryParams,
    IQuery<PaginatedList<ProductListResult>>
{
}
internal sealed class GetProductListQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetProductListQuery, PaginatedList<ProductListResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<PaginatedList<ProductListResult>>> Handle(
        GetProductListQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Product> productsQuery = _context.Products
            .Include(m => m.Group)
            .Include(m => m.Category)
            .Include(m => m.ProductCollections)
                .ThenInclude(m => m.Collection)
            .Include(m => m.Variants)
                .ThenInclude(m => m.VariantAttributeValues)
                    .ThenInclude(m => m.AttributeOption)
            .AsQueryable()
            .AsNoTracking();

        // Search: Name, Sku, Description
        if (!string.IsNullOrEmpty(request.SearchTerm))
            productsQuery = productsQuery
                .ApplySearch(product =>
                    product.Name != null &&
                    product.Name.Contains(request.SearchTerm) ||
                    product.Descriptions != null && product.Descriptions.Any(m => m.Value.Contains(request.SearchTerm)) ||
                    product.Variants.Any(variant => variant.Sku.Contains(request.SearchTerm)) ||
                    product.Sku != null && product.Sku.Contains(request.SearchTerm));

        // Filter: TaxClass
        if (request.TaxClass != null && request.TaxClass.Length > 0)
            productsQuery = productsQuery
                .Where(p => request.TaxClass.Contains(p.TaxClass));

        // Filter: Status
        if (request.Status != null && request.Status.Length > 0)
            productsQuery = productsQuery
                .Where(p => request.Status.Contains(p.Status));

        // Filter: IsFeatured
        if (request.IsFeatured.HasValue)
            productsQuery = productsQuery
                .Where(p => p.IsFeatured == request.IsFeatured.Value);

        // Filter: Variants.IsVisible
        if (request.IsVisible.HasValue)
            productsQuery = productsQuery
                .Where(p => p.Variants.Any(v => v.IsVisible == request.IsVisible.Value));

        // Filter: Variants.IsActive
        if (request.IsActive.HasValue)
            productsQuery = productsQuery
                .Where(p => p.Variants.Any(v => v.IsActive == request.IsActive.Value));

        // Filter: GroupId
        if (request.GroupId != null && request.GroupId.Length > 0)
            productsQuery = productsQuery
                .Where(p => request.GroupId.Contains(p.GroupId));

        // Filter: CategoryId
        if (request.CategoryId != null && request.CategoryId.Length > 0)
            productsQuery = productsQuery
                .Where(p => p.CategoryId.HasValue && request.CategoryId.Contains(p.CategoryId.Value));

        // Filter: Attributes
        if (request.Attributes != null && request.Attributes.Any())
        {
            foreach (var attributeFilter in request.Attributes)
            {
                if (attributeFilter.Value != null && attributeFilter.Value.Any())
                {
                    // Find products with variants that match the selected attribute values
                    productsQuery = productsQuery.Where(p =>
                        p.Variants.Any(v =>
                            v.VariantAttributeValues.Any(vav =>
                                vav.AttributeId == attributeFilter.Id &&
                                // Value-based attibute
                                (!string.IsNullOrEmpty(vav.Value) && attributeFilter.Value.Contains(vav.Value) ||
                                // Option-based attibute
                                vav.AttributeOption != null &&
                                attributeFilter.Value.Contains(vav.AttributeOption.OptionValue))
                            )
                        )
                    );
                }
            }
        }

        var paginatedList = await productsQuery
            .ApplySort(
                sortBy: request.SortBy ?? "Name",
                sortDirection: request.SortDirection ?? "asc")
            .ProjectToType<ProductListResult>(_mapper.Config)
            .CreateAsync(
                request.PageIndex ?? 1,
                request.PageSize ?? 10,
                cancellationToken);

        return paginatedList;
    }
}
