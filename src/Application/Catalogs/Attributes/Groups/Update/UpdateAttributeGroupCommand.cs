using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Groups.Update;

[ApiAuthorize(Permission.AttributeGroup.Update)]
public record UpdateAttributeGroupCommand : ICommand<Updated>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

internal sealed class UpdateAttributeGroupCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateAttributeGroupCommand, Updated>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Updated>> Handle(UpdateAttributeGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _context.AttributeGroups
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (group is null)
        {
            return DomainErrors.AttributeGroup.InvalidId;
        }

        group.Update(request.Name);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
