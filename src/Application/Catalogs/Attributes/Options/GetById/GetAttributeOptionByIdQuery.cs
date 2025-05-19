using ErrorOr;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Attributes.Models.Options;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Options.GetById;


[ApiAuthorize(Permission.AttributeOption.Get)]
public record GetAttributeOptionByIdQuery: IQuery<AttributeOptionResult>
{
    public Guid Id { get; set; }
}
internal sealed class GetAttributeOptionByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IQueryHandler<GetAttributeOptionByIdQuery, AttributeOptionResult>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<ErrorOr<AttributeOptionResult>> Handle(GetAttributeOptionByIdQuery request, CancellationToken cancellationToken)
    {
        var option = await _context.AttributeOptions
            .ProjectToType<AttributeOptionResult>(_mapper.Config)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (option == null)
        {
            return DomainErrors.AttributeOption.NotFound;
        }

        return option;
    }
}
