using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Attributes;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.GetById;

[ApiAuthorize(Permission.Attribute.Get)]
public record GetCatalogAttributeByIdQUery : IQuery<AttributeResult>
{
    public Guid Id { get; set; }
}

internal sealed class GetCatalogAttributeByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper)
    : IQueryHandler<GetCatalogAttributeByIdQUery, AttributeResult>
{
    public async Task<ErrorOr<AttributeResult>> Handle(
        GetCatalogAttributeByIdQUery request,
        CancellationToken cancellationToken)
    {
        var group = await context.Attributes
            .Include(m => m.AttributeOptions)
            .Include(m => m.AttributeGroupAttributes)
                .ThenInclude(m => m.AttributeGroup)
            .ProjectToType<AttributeResult>(mapper.Config)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);

        if (group is null)
        {
            return DomainErrors.CatalogAttribute.NotFound();
        }

        return group;
    }
}
