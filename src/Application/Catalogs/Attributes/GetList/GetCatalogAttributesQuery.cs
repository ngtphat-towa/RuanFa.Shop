using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Attributes;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.QueryParams;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Catalogs.Attributes.GetList;

[ApiAuthorize(Permission.Attribute.Get)]
public record GetCatalogAttributesQuery :
    CatalogAttributeQueryParameters,
    IQuery<PaginatedList<CatalogAttributeListResult>>
{
}

internal sealed class GetCatalogAttributesQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetCatalogAttributesQuery, PaginatedList<CatalogAttributeListResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<PaginatedList<CatalogAttributeListResult>>> Handle(
        GetCatalogAttributesQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<CatalogAttribute> paginatedListQuery = _context.Attributes
            .Include(m => m.AttributeOptions)
            .Include(m => m.AttributeGroupAttributes)
                .ThenInclude(m => m.AttributeGroup)
            .AsQueryable()
            .AsNoTracking();

        // Search: Name
        if (!string.IsNullOrEmpty(request.SearchTerm))
            paginatedListQuery = paginatedListQuery
                .ApplySearch(g => string.IsNullOrEmpty(g.Name) ||
                             g.Name.Contains(request.SearchTerm));
        // Filter: IsFilterable
        if (request.IsFilterable.HasValue)
            paginatedListQuery = paginatedListQuery
                .Where(m => m.IsFilterable == request.IsFilterable);
        // Filter: IsRequired
        if (request.IsRequired.HasValue)
            paginatedListQuery = paginatedListQuery
                .Where(m => m.IsRequired == request.IsRequired);
        // Filter: IsRequired
        if (request.DisplayOnFrontend.HasValue)
            paginatedListQuery = paginatedListQuery
                .Where(m => m.DisplayOnFrontend == request.DisplayOnFrontend);
        // Filter: IsRequired
        if (request.Type != null && request.Type.Count != 0)
            paginatedListQuery = paginatedListQuery
                .Where(m => request.Type.Contains(m.Type));

        var paginatedList = await paginatedListQuery
            .ApplyFilters(request.Filters)
            .ApplySort(
                sortBy: request.SortBy ?? "SortOrder",
                sortDirection: request.SortDirection ?? "asc")
            .ProjectToType<CatalogAttributeListResult>(_mapper.Config)
            .CreateAsync(
                request.PageIndex,
                request.PageSize,
                cancellationToken);

        return paginatedList;
    }
}
