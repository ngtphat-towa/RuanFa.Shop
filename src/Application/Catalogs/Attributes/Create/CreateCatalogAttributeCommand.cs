using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.AggregateRoots;
using RuanFa.Shop.Domain.Catalogs.Entities;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Create;

[ApiAuthorize(Permission.Attribute.Create)]
public sealed record CreateCatalogAttributeCommand : ICommand<Guid>
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required AttributeType Type { get; init; }
    public bool IsRequired { get; init; } = false;
    public bool DisplayOnFrontend { get; init; } = false;
    public int SortOrder { get; init; } = 0;
    public bool IsFilterable { get; init; } = false;
    public List<Guid>? GroupIds { get; init; }
    public List<string>? Options { get; init; }
}

internal sealed class CreateCatalogAttributeCommandHandler : ICommandHandler<CreateCatalogAttributeCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCatalogAttributeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateCatalogAttributeCommand request, CancellationToken cancellationToken)
    {
        // Check if attribute with same code already exists
        var exists = await _context.Attributes
            .AnyAsync(a => a.Code == request.Code, cancellationToken);

        if (exists)
        {
            return DomainErrors.CatalogAttribute.DuplicateCode(request.Code);
        }

        // Create the catalog attribute
        var result = CatalogAttribute.Create(
            code: request.Code,
            name: request.Name,
            type: request.Type,
            isRequired: request.IsRequired,
            displayOnFrontend: request.DisplayOnFrontend,
            sortOrder: request.SortOrder,
            isFilterable: request.IsFilterable);

        if (result.IsError)
            return result.Errors;

        var attribute = result.Value;

        // Add options if provided (for dropdown or swatch types)
        if (request.Options != null && request.Options.Any())
        {
            if (request.Type != AttributeType.Dropdown && request.Type != AttributeType.Swatch)
            {
                return DomainErrors.CatalogAttribute.OptionsNotSupportedForType;
            }

            foreach (var optionText in request.Options)
            {
                var optionResult = attribute.AddOption(optionText);
                if (optionResult.IsError)
                    return optionResult.Errors;
            }
        }
        else if (request.Type == AttributeType.Dropdown || request.Type == AttributeType.Swatch)
        {
            return DomainErrors.CatalogAttribute.TypeRequiresOptions;
        }

        // Associate with attribute groups if provided
        if (request.GroupIds != null && request.GroupIds.Any())
        {
            var groups = await _context.AttributeGroups
                .Where(g => request.GroupIds.Contains(g.Id))
                .ToListAsync(cancellationToken);

            if (groups.Count != request.GroupIds.Count)
            {
                var foundIds = groups.Select(g => g.Id).ToList();
                var missingIds = request.GroupIds.Except(foundIds).ToList();
                if (missingIds.Any())
                {
                    return DomainErrors.AttributeGroup.NotFound();
                }
            }

            foreach (var group in groups)
            {
                var groupAttributeResult = AttributeGroupAttribute.Create(group.Id, attribute.Id);
                if (groupAttributeResult.IsError)
                    return groupAttributeResult.Errors;

                _context.AttributeGroupAttributes.Add(groupAttributeResult.Value);
            }
        }

        // Add attribute to the context and save
        _context.Attributes.Add(attribute);
        await _context.SaveChangesAsync(cancellationToken);

        return attribute.Id;
    }
}
