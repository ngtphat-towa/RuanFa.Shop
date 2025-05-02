using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Emails.Update;

internal class UpdateEmailCommandValidator : AbstractValidator<UpdateEmailCommand>
{
    public UpdateEmailCommandValidator()
    {

        // NewEmail validation
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .WithMessage(DomainErrors.EmailUpdate.NewEmailRequired.Description)
            .WithErrorCode(DomainErrors.EmailUpdate.NewEmailRequired.Code)
            .EmailAddress()
            .WithMessage(DomainErrors.EmailUpdate.InvalidEmailFormat.Description)
            .WithErrorCode(DomainErrors.EmailUpdate.InvalidEmailFormat.Code)
            .When(x => !string.IsNullOrEmpty(x.NewEmail));
    }
}
