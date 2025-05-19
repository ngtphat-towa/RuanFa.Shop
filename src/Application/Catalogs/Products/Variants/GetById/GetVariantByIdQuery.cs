using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Catalogs.Products.Models.Products;
using RuanFa.Shop.Application.Catalogs.Products.Models.Varaiants;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Products.Variants.GetById;

public sealed record GetVariantByIdQuery(Guid VariantId) : IQuery<VariantResult>;

internal sealed class GetVariantByIdQueryHandler : IQueryHandler<GetVariantByIdQuery, VariantResult>
{
    private readonly IApplicationDbContext _context;

    public GetVariantByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<VariantResult>> Handle(GetVariantByIdQuery request, CancellationToken cancellationToken)
    {
        // Load the variant and include its attribute values (plus the related AttributeOption)
        var variant = await _context.Variants
            .Include(v => v.VariantAttributeValues)
                .ThenInclude(vav => vav.AttributeOption)
            .FirstOrDefaultAsync(v => v.Id == request.VariantId, cancellationToken);

        if (variant == null)
            return DomainErrors.ProductVariant.NotFound;

        // Load images for this variant
        var images = await _context.ProductImages
            .Where(pi => pi.VariantId == variant.Id)
            .ToListAsync(cancellationToken);

        // Map to DTO
        var dto = new VariantResult
        {
            Id = variant.Id,
            Sku = variant.Sku,
            PriceOffset = variant.PriceOffset,
            StockQuantity = variant.StockQuantity,
            LowStockThreshold = variant.LowStockThreshold,
            IsActive = variant.IsActive,
            IsVisible = variant.IsVisible,
            ProductId = variant.ProductId,
            AttributeValues = variant.VariantAttributeValues
                .Select(vav => new VariantAttibuteValueResult
                {
                    Id = vav.AttributeId,
                    Type = (int)vav.Attribute.Type,
                    Name = vav.Attribute.Name,
                    OptionId = vav.AttributeOptionId,
                    Value = vav.Value,
                })
                .ToList(),
            Images = images
                .Select(pi => new ProductImageResult
                {
                    Id = pi.Id,
                    VariantId = pi.VariantId,
                    Url = pi.Image.Url,
                    Alt = pi.Image.Alt,
                    IsDefault = pi.IsDefault,
                    ImageType = (int)pi.Image.ImageType
                })
                .ToList()
        };

        return dto;
    }
}
