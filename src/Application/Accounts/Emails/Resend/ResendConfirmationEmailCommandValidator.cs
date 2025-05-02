using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Emails.Resend;

internal class ResendConfirmationEmailCommandValidator : AbstractValidator<ResendConfirmationEmailCommand>
{
    public ResendConfirmationEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(DomainErrors.EmailConfirmation.ResendEmailRequired.Description)
            .WithErrorCode(DomainErrors.EmailConfirmation.ResendEmailRequired.Code)
            .EmailAddress()
            .WithMessage(DomainErrors.EmailConfirmation.ResendInvalidEmailFormat.Description)
            .WithErrorCode(DomainErrors.EmailConfirmation.ResendInvalidEmailFormat.Code);
    }
}
