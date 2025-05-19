using FluentValidation;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Groups.Create;

internal sealed class CreateAttributeGroupCommandValidator : AbstractValidator<CreateAttributeGroupCommand>
{
    public CreateAttributeGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage(DomainErrors.AttributeGroup.EmptyName.Description)
                .WithErrorCode(DomainErrors.AttributeGroup.EmptyName.Code)
            .MinimumLength(3)
                .WithMessage(DomainErrors.AttributeGroup.NameTooShort.Description)
                .WithErrorCode(DomainErrors.AttributeGroup.NameTooShort.Code)
            .MaximumLength(50)
                .WithMessage(DomainErrors.AttributeGroup.NameTooLong.Description)
                .WithErrorCode(DomainErrors.AttributeGroup.NameTooLong.Code);
    }
}
