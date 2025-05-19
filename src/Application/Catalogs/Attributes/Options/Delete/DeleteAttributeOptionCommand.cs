using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Options.Delete;

[ApiAuthorize(Permission.AttributeOption.Delete)]
public record DeleteAttributeOptionCommand : ICommand<Deleted>
{
    public Guid Id { get; set; }
}

internal sealed class DeleteAttributeOptionCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteAttributeOptionCommand, Deleted>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Deleted>> Handle(DeleteAttributeOptionCommand request, CancellationToken cancellationToken)
    {
        var option = await _context.AttributeOptions
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (option == null)
        {
            return DomainErrors.AttributeOption.NotFound;
        }
        // TODO: handle if any variant using

        _context.AttributeOptions.Remove(option);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}
