using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Groups;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Groups.GetById;

[ApiAuthorize(Permission.AttributeGroup.Get)]
public record GetAttributeGroupByIdQuery(Guid Id) : IQuery<AttributeGroupResult>;

internal sealed class GetAttributeGroupByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetAttributeGroupByIdQuery, AttributeGroupResult>
{
    public async Task<ErrorOr<AttributeGroupResult>> Handle(GetAttributeGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await context.AttributeGroups
            .Include(m => m.Products)
            .Include(m => m.AttributeGroupAttributes)
                .ThenInclude(m => m.Attribute)
            .ProjectToType<AttributeGroupResult>(mapper.Config)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (group is null)
        {
            return DomainErrors.AttributeGroup.NotFound();
        }

        return group;
    }
}
