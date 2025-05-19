using FluentValidation;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Groups.Update;

internal class UpdateAttributeGroupCommandValidator : AbstractValidator<UpdateAttributeGroupCommand>
{
    public UpdateAttributeGroupCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
                .WithMessage(DomainErrors.AttributeGroup.InvalidId.Description)
                .WithErrorCode(DomainErrors.AttributeGroup.InvalidId.Code);

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
