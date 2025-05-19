using ErrorOr;
using Microsoft.EntityFrameworkCore;
using RuanFa.Shop.Application.Common.Data;
using RuanFa.Shop.Application.Common.Security.Authorization.Attributes;
using RuanFa.Shop.Application.Common.Security.Permissions;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;
using RuanFa.Shop.SharedKernel.Interfaces.Messaging;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Update;

[ApiAuthorize(Permission.Attribute.Update)]
public sealed record UpdateCatalogAttributeCommand : ICommand<Updated>
{
    public required Guid Id { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }
    public AttributeType? Type { get; init; }
    public bool? IsRequired { get; init; }
    public bool? DisplayOnFrontend { get; init; }
    public int? SortOrder { get; init; }
    public bool? IsFilterable { get; init; }
    public List<string>? Options { get; init; }
    public List<Guid>? GroupIds { get; init; }
}

internal sealed class UpdateCatalogAttributeCommandHandler(IApplicationDbContext context) 
    : ICommandHandler<UpdateCatalogAttributeCommand, Updated>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateCatalogAttributeCommand request, CancellationToken cancellationToken)
    {
        var attribute = await context.Attributes
            .Include(a => a.AttributeOptions)
            .Include(a => a.AttributeGroupAttributes)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (attribute == null)
        {
            return DomainErrors.CatalogAttribute.NotFound();
        }

        if (request.Code != null && request.Code != attribute.Code)
        {
            var exists = await context.Attributes
                .AnyAsync(a => a.Code == request.Code && a.Id != request.Id, cancellationToken);

            if (exists)
            {
                return DomainErrors.CatalogAttribute.DuplicateCode(request.Code);
            }
        }

        var result = attribute.Update(
            attributeCode: request.Code,
            attributeName: request.Name,
            type: request.Type,
            isRequired: request.IsRequired,
            displayOnFrontend: request.DisplayOnFrontend,
            sortOrder: request.SortOrder,
            isFilterable: request.IsFilterable);

        if (result.IsError)
        {
            return result.Errors;
        }

        if (request.Options != null)
        {
            if (attribute.Type != AttributeType.Dropdown && attribute.Type != AttributeType.Swatch)
            {
                return DomainErrors.CatalogAttribute.OptionsNotSupportedForType;
            }

            var currentOptions = attribute.AttributeOptions?.Select(o => o.OptionText).ToList() 
                ?? new List<string>();
            var newOptions = request.Options;

            var optionsToAdd = newOptions.Except(currentOptions).ToList();
            foreach (var optionText in optionsToAdd)
            {
                var optionResult = attribute.AddOption(optionText);
                if (optionResult.IsError)
                {
                    return optionResult.Errors;
                }
            }

            var optionsToRemove = currentOptions.Except(newOptions).ToList();
            foreach (var optionText in optionsToRemove)
            {
                var option = attribute.AttributeOptions?.FirstOrDefault(o => o.OptionText == optionText);
                if (option != null)
                {
                    var removeResult = attribute.RemoveOption(option.Id);
                    if (removeResult.IsError)
                    {
                        return removeResult.Errors;
                    }
                }
            }
        }

        if (request.GroupIds != null)
        {
            var currentGroupIds = attribute.AttributeGroupAttributes?
                .Select(aga => aga.AttributeGroupId)
                .ToList() ?? new List<Guid>();
            var newGroupIds = request.GroupIds;

            var groups = await context.AttributeGroups
                .Where(g => newGroupIds.Contains(g.Id))
                .ToListAsync(cancellationToken);

            if (groups.Count != newGroupIds.Count)
            {
                var foundIds = groups.Select(g => g.Id).ToList();
                var missingIds = newGroupIds.Except(foundIds).ToList();

                return DomainErrors.AttributeGroup.NotFound(missingIds);
            }

            var groupIdsToAdd = newGroupIds.Except(currentGroupIds).ToList();
            foreach (var groupId in groupIdsToAdd)
            {
                var groupAttributeResult = Domain.Catalogs.Entities.AttributeGroupAttribute.Create(groupId, attribute.Id);
                if (groupAttributeResult.IsError)
                {
                    return groupAttributeResult.Errors;
                }

                context.AttributeGroupAttributes.Add(groupAttributeResult.Value);
            }

            var groupIdsToRemove = currentGroupIds.Except(newGroupIds).ToList();
            if (groupIdsToRemove.Any())
            {
                var associationsToRemove = await context.AttributeGroupAttributes
                    .Where(aga => aga.AttributeId == attribute.Id && groupIdsToRemove.Contains(aga.AttributeGroupId))
                    .ToListAsync(cancellationToken);

                if (associationsToRemove.Any())
                {
                    context.AttributeGroupAttributes.RemoveRange(associationsToRemove);
                }
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
