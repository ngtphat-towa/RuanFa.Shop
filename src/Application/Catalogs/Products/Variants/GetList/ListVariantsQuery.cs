using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Products.Models.Products;
using RuanFa.Shop.Application.Catalogs.Products.Models.QueryParams;
using RuanFa.Shop.Application.Catalogs.Products.Models.Varaiants;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Catalogs.Products.Variants.GetList;

public sealed record ListVariantsQuery : VariantListQueryParams, IQuery<PaginatedList<VariantListResult>>;

internal sealed class ListVariantsQueryHandler 
    : IQueryHandler<ListVariantsQuery, PaginatedList<VariantListResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ListVariantsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ErrorOr<PaginatedList<VariantListResult>>> Handle(ListVariantsQuery request, CancellationToken cancellationToken)
    {
        // Build base query, including attribute values (and Option)
        var query = _context.Variants
            .Include(m => m.Product)
            .Include(m => m.VariantImages)
            .Include(v => v.VariantAttributeValues)
                .ThenInclude(vav => vav.Attribute)
            .Include(v => v.VariantAttributeValues)
                .ThenInclude(vav => vav.AttributeOption)
            .AsQueryable();

        // Search: Name, Sku, Description
        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query
                .ApplySearch(variant =>
                    variant.Product.Name != null &&
                    variant.Product.Name.Contains(request.SearchTerm) ||
                    variant.Product.Descriptions != null && variant.Product.Descriptions.Any(m => m.Value.Contains(request.SearchTerm)) ||
                    variant.Sku != null && variant.Sku.Contains(request.SearchTerm) ||
                    variant.Product.Sku != null && variant.Product.Sku.Contains(request.SearchTerm));

        // Filter: Product
        if (request.ProductId.HasValue)
            query = query.Where(v => v.ProductId == request.ProductId.Value);

        // Filter: Attributes
        if (request.Attributes != null && request.Attributes.Any())
        {
            foreach (var attributeFilter in request.Attributes)
            {
                if (attributeFilter.Value != null && attributeFilter.Value.Any())
                {
                    // Find products with variants that match the selected attribute values
                    query = query.Where(v => v.VariantAttributeValues.Any(vav =>
                                vav.AttributeId == attributeFilter.Id &&
                                // Value-based attibute
                                (!string.IsNullOrEmpty(vav.Value) && attributeFilter.Value.Contains(vav.Value) ||
                                // Option-based attibute
                                vav.AttributeOption != null &&
                                attributeFilter.Value.Contains(vav.AttributeOption.OptionValue))
                            )
                    );
                }
            }
        }

        var variants = await query
         .ApplySort(
             sortBy: request.SortBy ?? "Name",
             sortDirection: request.SortDirection ?? "asc")
         .ProjectToType<VariantListResult>(_mapper.Config)
         .CreateAsync(
             request.PageIndex ?? 1,
             request.PageSize ?? 10,
             cancellationToken);

        return variants;
    }
}
