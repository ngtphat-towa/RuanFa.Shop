using FluentValidation;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Update;

internal class UpdateCatalogAttributeCommandValidator : AbstractValidator<UpdateCatalogAttributeCommand>
{
    public UpdateCatalogAttributeCommandValidator()
    {
        // Id validation
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
                .WithMessage(DomainErrors.CatalogAttribute.InvalidId.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.InvalidId.Code);

        // Code validation (only when provided)
        RuleFor(x => x.Code)
            .NotEmpty()
                .WithMessage(DomainErrors.CatalogAttribute.EmptyCode.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.EmptyCode.Code)
            .MinimumLength(3)
                .WithMessage(DomainErrors.CatalogAttribute.CodeTooShort.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.CodeTooShort.Code)
            .MaximumLength(50)
                .WithMessage(DomainErrors.CatalogAttribute.CodeTooLong.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.CodeTooLong.Code)
            .Matches(@"^[a-zA-Z0-9\-_]+$")
                .WithMessage(DomainErrors.CatalogAttribute.InvalidCodeFormat.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.InvalidCodeFormat.Code)
            .When(x => x.Code != null);

        // Name validation (only when provided)
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage(DomainErrors.CatalogAttribute.EmptyName.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.EmptyName.Code)
            .MaximumLength(100)
                .WithMessage(DomainErrors.CatalogAttribute.NameTooLong.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.NameTooLong.Code)
            .When(x => x.Name != null);

        // Type validation (only when provided)
        RuleFor(x => x.Type)
            .NotEqual(AttributeType.None)
                .WithMessage(DomainErrors.CatalogAttribute.InvalidType.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.InvalidType.Code)
            .IsInEnum()
                .WithMessage(DomainErrors.CatalogAttribute.InvalidType.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.InvalidType.Code)
            .When(x => x.Type != null);

        // SortOrder validation (only when provided)
        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0)
                .WithMessage(DomainErrors.CatalogAttribute.InvalidSortOrder.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.InvalidSortOrder.Code)
            .When(x => x.SortOrder != null);

        // Options validation (only when provided)
        RuleFor(x => x.Options)
            .NotEmpty()
            .When(x => (x.Type == AttributeType.Dropdown || x.Type == AttributeType.Swatch) && x.Options != null)
                .WithMessage(DomainErrors.CatalogAttribute.TypeRequiresOptions.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.TypeRequiresOptions.Code);

        RuleFor(x => x.Options)
            .Empty()
            .When(x => x.Type != AttributeType.Dropdown && x.Type != AttributeType.Swatch && x.Options != null && x.Options.Any())
                .WithMessage(DomainErrors.CatalogAttribute.OptionsNotSupportedForType.Description)
                .WithErrorCode(DomainErrors.CatalogAttribute.OptionsNotSupportedForType.Code);

        // Validate that all GroupIds are valid GUIDs and not empty (only when provided)
        RuleForEach(x => x.GroupIds)
            .NotEqual(Guid.Empty)
                .WithMessage(DomainErrors.AttributeGroupAttribute.InvalidGroupId.Description)
                .WithErrorCode(DomainErrors.AttributeGroupAttribute.InvalidGroupId.Code)
            .When(x => x.GroupIds != null);
    }
}
