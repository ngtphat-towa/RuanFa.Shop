using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Groups;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.QueryParams;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Groups.GetList;

[ApiAuthorize(Permission.AttributeGroup.Get)]
public record GetAttributeGroupsQuery : 
    AttributeGroupQueryParameters, 
    IQuery<PaginatedList<AttributeGroupListResult>>
{
}

internal sealed class GetAttributeGroupsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetAttributeGroupsQuery, PaginatedList<AttributeGroupListResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<PaginatedList<AttributeGroupListResult>>> Handle(
        GetAttributeGroupsQuery request,
        CancellationToken cancellationToken)
    {
        // Get the queryable AttributeGroups
        var paginatedListQuery = _context.AttributeGroups
            .Include(m => m.Products)
            .Include(m => m.AttributeGroupAttributes)
                .ThenInclude(m => m.Attribute)
            .AsQueryable()
            .AsNoTracking();

        // Search: Name
        if (!string.IsNullOrEmpty(request.SearchTerm))
            paginatedListQuery = paginatedListQuery
                .ApplySearch(g => string.IsNullOrEmpty(g.Name) ||
                             g.Name.Contains(request.SearchTerm));

        var paginatedList = await paginatedListQuery
            .ApplySort(
                sortBy: request.SortBy ?? "Name", 
                sortDirection: request.SortDirection ?? "asc")
            .ProjectToType<AttributeGroupListResult>(_mapper.Config)
            .CreateAsync(
                request.PageIndex,
                request.PageSize,
                cancellationToken);

        return paginatedList;
    }
}
