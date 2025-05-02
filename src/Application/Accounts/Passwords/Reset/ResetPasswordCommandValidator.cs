using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Passwords.Reset
{
    internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            // Email validation
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(DomainErrors.PasswordReset.EmailRequired.Description)
                .WithErrorCode(DomainErrors.PasswordReset.EmailRequired.Code)
                .EmailAddress()
                .WithMessage(DomainErrors.PasswordReset.InvalidEmailFormat.Description)
                .WithErrorCode(DomainErrors.PasswordReset.InvalidEmailFormat.Code);

            // Reset token validation
            RuleFor(x => x.ResetToken)
                .NotEmpty()
                .WithMessage(DomainErrors.PasswordReset.ResetTokenRequired.Description)
                .WithErrorCode(DomainErrors.PasswordReset.ResetTokenRequired.Code)
                .Length(6, 100)
                .WithMessage(DomainErrors.PasswordReset.InvalidResetTokenFormat.Description)
                .WithErrorCode(DomainErrors.PasswordReset.InvalidResetTokenFormat.Code);

            // New password validation
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(DomainErrors.PasswordReset.NewPasswordRequired.Description)
                .WithErrorCode(DomainErrors.PasswordReset.NewPasswordRequired.Code)
                .MinimumLength(6)
                .WithMessage(DomainErrors.PasswordReset.NewPasswordTooShort.Description)
                .WithErrorCode(DomainErrors.PasswordReset.NewPasswordTooShort.Code)
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{6,}$")
                .WithMessage(DomainErrors.PasswordReset.InvalidPasswordFormat.Description)
                .WithErrorCode(DomainErrors.PasswordReset.InvalidPasswordFormat.Code);
        }
    }
}
