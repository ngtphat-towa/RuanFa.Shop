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

        RuleFor(x => x.Code)
            .NotEmpty()
                .WithMessage(DomainErrors.AttributeOption.EmptyCode.Description)
                .WithErrorCode(DomainErrors.AttributeOption.EmptyCode.Code)
            .MinimumLength(3)
                .WithMessage(DomainErrors.AttributeOption.CodeTooShort.Description)
                .WithErrorCode(DomainErrors.AttributeOption.CodeTooShort.Code)
            .MaximumLength(50)
                .WithMessage(DomainErrors.AttributeOption.CodeTooLong.Description)
                .WithErrorCode(DomainErrors.AttributeOption.CodeTooLong.Code)
            .Matches(@"^[a-zA-Z0-9\-_]+$")
                .WithMessage(DomainErrors.AttributeOption.InvalidCodeFormat.Description)
                .WithErrorCode(DomainErrors.AttributeOption.InvalidCodeFormat.Code)
            .When(x => x.Code != null);

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
