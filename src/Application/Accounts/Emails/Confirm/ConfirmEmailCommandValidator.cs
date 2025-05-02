using FluentValidation;
using RuanFa.Shop.Application.Accounts.Emails.Confirm;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Authentication;

internal class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        // UserId validation
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(DomainErrors.EmailConfirmation.UserIdRequired.Description)
            .WithErrorCode(DomainErrors.EmailConfirmation.UserIdRequired.Code)
            .Must(IsValidUserId)
            .WithMessage(DomainErrors.EmailConfirmation.InvalidUserIdFormat.Description)
            .WithErrorCode(DomainErrors.EmailConfirmation.InvalidUserIdFormat.Code);

        // Confirmation Code validation
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(DomainErrors.EmailConfirmation.ConfirmationCodeRequired.Description)
            .WithErrorCode(DomainErrors.EmailConfirmation.ConfirmationCodeRequired.Code)
            .Length(6, 100)
            .WithMessage(DomainErrors.EmailConfirmation.InvalidConfirmationCodeFormat.Description)
            .WithErrorCode(DomainErrors.EmailConfirmation.InvalidConfirmationCodeFormat.Code);

        // Changed Email validation (optional)
        When(x => !string.IsNullOrEmpty(x.ChangedEmail), () =>
        {
            RuleFor(x => x.ChangedEmail)
                .EmailAddress()
                .WithMessage(DomainErrors.EmailConfirmation.InvalidEmailFormat.Description)
                .WithErrorCode(DomainErrors.EmailConfirmation.InvalidEmailFormat.Code);
        });
    }

    private bool IsValidUserId(string userId)
    {
        // Basic GUID format validation - adjust according to your user ID format
        return Guid.TryParse(userId, out _);
    }
}
