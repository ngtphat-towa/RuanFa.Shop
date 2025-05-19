using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Groups.Delete;

[ApiAuthorize(Permission.AttributeGroup.Delete)]
public record DeleteAttributeGroupCommand : ICommand<Deleted>
{
    public Guid Id { get; init; }
}
internal sealed class DeleteAttributeGroupCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteAttributeGroupCommand, Deleted>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Deleted>> Handle(DeleteAttributeGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.AttributeGroups
            .Include(p => p.Products)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (group is null)
        {
            return DomainErrors.AttributeGroup.NotFound();
        }

        var isInUse = group.Products.Count != 0;
        if (isInUse)
        {
            return DomainErrors.AttributeGroup.InUse;
        }

        _context.AttributeGroups.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
