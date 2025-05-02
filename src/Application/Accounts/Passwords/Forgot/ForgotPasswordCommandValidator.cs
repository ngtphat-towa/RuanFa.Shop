using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Passwords.Forgot
{
    internal class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(DomainErrors.PasswordRecovery.EmailRequired.Description)
                .WithErrorCode(DomainErrors.PasswordRecovery.EmailRequired.Code)
                .EmailAddress()
                .WithMessage(DomainErrors.PasswordRecovery.InvalidEmailFormat.Description)
                .WithErrorCode(DomainErrors.PasswordRecovery.InvalidEmailFormat.Code);
        }
    }
}
