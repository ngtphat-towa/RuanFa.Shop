using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Options;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.QueryParams;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Extensions.Queries;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;
using RuanFa.Shop.SharedKernel.Models.Wrappers;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Options.GetList;

[ApiAuthorize(Permission.Attribute.Get)]
public record GetAttributeOptionsQuery : AttributeOptionQueryParameters, IQuery<PaginatedList<AttributeOptionListResult>>;

internal sealed class GetAttributeOptionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetAttributeOptionsQuery, PaginatedList<AttributeOptionListResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<PaginatedList<AttributeOptionListResult>>> Handle(
        GetAttributeOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.AttributeOptions
            .Include(o => o.Attribute)
            .Include(o => o.VariantAttributeOptions)
            .AsQueryable()
            .AsNoTracking()
;
        // Apply filters
        if (request.AttributeId.HasValue)
        {
            query = query.Where(o => o.AttributeId == request.AttributeId.Value);
        }
        if (!string.IsNullOrEmpty(request.AttributeCode))
        {
            query = query.Where(o => o.Code == request.AttributeCode);
        }
        if (request.Type.HasValue)
        {
            query = query.Where(o => o.Attribute.Type == request.Type.Value);
        }
        if (request.IsRequired.HasValue)
        {
            query = query.Where(o => o.Attribute.IsRequired == request.IsRequired.Value);
        }
        if (request.DisplayOnFrontend.HasValue)
        {
            query = query.Where(o => o.Attribute.DisplayOnFrontend == request.DisplayOnFrontend.Value);
        }
        if (request.IsFilterable.HasValue)
        {
            query = query.Where(o => o.Attribute.IsFilterable == request.IsFilterable.Value);
        }

        var paginatedList = await query
            .ApplySearch(o => string.IsNullOrEmpty(request.SearchTerm) ||
                             string.IsNullOrEmpty(o.OptionText) ||
                             string.IsNullOrEmpty(o.Code) ||
                             o.OptionText.Contains(request.SearchTerm) ||
                             o.Code.Contains(request.SearchTerm))
            .ApplySort(request.SortBy, request.SortDirection)
            .ProjectToType<AttributeOptionListResult>(_mapper.Config)
            .CreateAsync(
                request.PageIndex,
                request.PageSize,
                cancellationToken);

        return paginatedList;
    }
}
