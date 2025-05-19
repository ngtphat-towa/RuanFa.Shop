using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Options.Create;

[ApiAuthorize(Permission.AttributeOption.Create)]
public record CreateAttributeOptionCommand : ICommand<Guid>
{
    public required Guid AttributeId { get; init; }
    public required string AttributeCode { get; init; }
    public required string OptionText { get; init; }
}

internal sealed class CreateAttributeOptionCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateAttributeOptionCommand, Guid>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Guid>> Handle(CreateAttributeOptionCommand request, CancellationToken cancellationToken)
    {
        var attribute = await _context.Attributes
            .FirstOrDefaultAsync(a => a.Id == request.AttributeId, cancellationToken);

        if (attribute == null)
        {
            return DomainErrors.CatalogAttribute.NotFound();
        }

        if (attribute.Type != Domain.Catalogs.Enums.AttributeType.Dropdown &&
            attribute.Type != Domain.Catalogs.Enums.AttributeType.Swatch)
        {
            return DomainErrors.CatalogAttribute.OptionsNotSupportedForType;
        }

        var createOptionResult = AttributeOption.Create(
            attributeId: request.AttributeId,
            code: request.AttributeCode,
            optionText: request.OptionText);

        if (createOptionResult.IsError)
        {
            return createOptionResult.Errors;
        }

        _context.AttributeOptions.Add(createOptionResult.Value);
        await _context.SaveChangesAsync(cancellationToken);

        return createOptionResult.Value.Id;
    }
}
