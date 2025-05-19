using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Groups.Create;

[ApiAuthorize(Permission.AttributeGroup.Create)]
public record CreateAttributeGroupCommand : ICommand<Guid>
{
    public required string Name { get; init; }
}
internal sealed class CreateAttributeGroupCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateAttributeGroupCommand, Guid>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<ErrorOr<Guid>> Handle(CreateAttributeGroupCommand request, CancellationToken cancellationToken)
    {
        var isDuplicate = await _context.AttributeGroups
            .AnyAsync(g => g.Name == request.Name, cancellationToken);

        if (isDuplicate)
        {
            return DomainErrors.AttributeGroup.DuplicateName(request.Name);
        }

        var createGroupResult = AttributeGroup.Create(request.Name);
        if (createGroupResult.IsError) return createGroupResult.Errors;

        _context.AttributeGroups.Add(createGroupResult.Value);
        await _context.SaveChangesAsync(cancellationToken);

        return createGroupResult.Value.Id;
    }
}
