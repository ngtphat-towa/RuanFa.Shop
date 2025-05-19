using FluentValidation;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Options.Create;
internal sealed class CreateAttributeOptionCommandValidator : AbstractValidator<CreateAttributeOptionCommand>
{
    public CreateAttributeOptionCommandValidator()
    {
        RuleFor(x => x.AttributeId)
            .NotEqual(Guid.Empty)
                .WithMessage(DomainErrors.AttributeOption.InvalidAttributeId.Description)
                .WithErrorCode(DomainErrors.AttributeOption.InvalidAttributeId.Code);

        RuleFor(x => x.OptionText)
            .NotEmpty()
                .WithMessage(DomainErrors.AttributeOption.EmptyOptionText.Description)
                .WithErrorCode(DomainErrors.AttributeOption.EmptyOptionText.Code)
            .MaximumLength(100)
                .WithMessage(DomainErrors.AttributeOption.OptionTextTooLong.Description)
                .WithErrorCode(DomainErrors.AttributeOption.OptionTextTooLong.Code);
    }
}
