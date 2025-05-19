using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Options.Update;

[ApiAuthorize(Permission.AttributeOption.Update)]
public record UpdateAttributeOptionCommand : ICommand<Updated>
{
    public required Guid Id { get; init; }
    public string? OptionText { get; init; }
}

internal sealed class UpdateAttributeOptionCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateAttributeOptionCommand, Updated>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Updated>> Handle(UpdateAttributeOptionCommand request, CancellationToken cancellationToken)
    {
        var option = await _context.AttributeOptions
            .Include(o => o.Attribute)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (option == null)
        {
            return DomainErrors.AttributeOption.NotFound;
        }

        if (request.OptionText != null)
        {
            var updateTextResult = option.UpdateOptionText(request.OptionText);
            if (updateTextResult.IsError)
            {
                return updateTextResult.Errors;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
