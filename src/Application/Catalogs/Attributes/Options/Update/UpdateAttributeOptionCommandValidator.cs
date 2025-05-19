using FluentValidation;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Application.Catalogs.Attributes.Options.Update;
internal sealed class UpdateAttributeOptionCommandValidator : AbstractValidator<UpdateAttributeOptionCommand>
{
    public UpdateAttributeOptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
                .WithMessage(DomainErrors.AttributeOption.InvalidId.Description)
                .WithErrorCode(DomainErrors.AttributeOption.InvalidId.Code);

        RuleFor(x => x.OptionText)
            .NotEmpty()
                .WithMessage(DomainErrors.AttributeOption.EmptyOptionText.Description)
                .WithErrorCode(DomainErrors.AttributeOption.EmptyOptionText.Code)
            .MaximumLength(100)
                .WithMessage(DomainErrors.AttributeOption.OptionTextTooLong.Description)
                .WithErrorCode(DomainErrors.AttributeOption.OptionTextTooLong.Code)
            .When(x => x.OptionText != null);
    }
}
