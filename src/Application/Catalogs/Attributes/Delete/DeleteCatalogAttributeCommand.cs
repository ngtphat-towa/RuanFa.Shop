using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Delete;

[ApiAuthorize(Permission.Attribute.Delete)]
public sealed record DeleteCatalogAttributeCommand(Guid Id) : ICommand<Deleted>;

internal sealed class DeleteCatalogAttributeCommandHandler : ICommandHandler<DeleteCatalogAttributeCommand, Deleted>
{
    private readonly IApplicationDbContext _context;

    public DeleteCatalogAttributeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteCatalogAttributeCommand request, CancellationToken cancellationToken)
    {
        var attribute = await _context.Attributes
            .Include(a => a.AttributeOptions)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attribute == null)
        {
            return DomainErrors.CatalogAttribute.NotFound();
        }

        var isInUse = await _context.AttributeGroupAttributes
            .Include(aga => aga.AttributeGroup)
            .ThenInclude(p => p.Products)
            .AnyAsync(aga => aga.AttributeId == request.Id 
                            && aga.AttributeGroup.Products.Any(), cancellationToken);

        if (isInUse)
        {
            return DomainErrors.CatalogAttribute.InUse;
        }

        _context.Attributes.Remove(attribute);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
