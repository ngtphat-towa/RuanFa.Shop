using ErrorOr;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Products.Models.Products;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Products.GetById;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductResult>;

internal sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ProductResult>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Group)
            .Include(p => p.Images)
            .Include(p => p.ProductCollections)
                .ThenInclude(pc => pc.Collection)
            .Include(p => p.Variants)
            .ThenInclude(v => v.VariantAttributeValues)
                    .ThenInclude(vav => vav.Attribute)
                .ThenInclude(v => v.VariantAttributeValues)
                    .ThenInclude(vav => vav.AttributeOption)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            return DomainErrors.Product.NotFound;
        }

        var result = _mapper.Map<ProductResult>(product);
        return result;
    }
}
