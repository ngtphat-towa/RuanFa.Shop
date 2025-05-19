using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;

namespace RuanFa.Shop.Application.Accounts.Authentication.RefreshToken;

internal class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage(DomainErrors.Authentication.RefreshTokenRequired.Description)
            .WithErrorCode(DomainErrors.Authentication.RefreshTokenRequired.Code)
            .Must(IsValidRefreshTokenFormat)
            .WithMessage(DomainErrors.Authentication.InvalidRefreshToken.Description)
            .WithErrorCode(DomainErrors.Authentication.InvalidRefreshToken.Code);
    }

    private bool IsValidRefreshTokenFormat(string refreshToken)
    {
        // Basic format validation - adjust according to your token format
        return !string.IsNullOrWhiteSpace(refreshToken) &&
               refreshToken.Length >= 32 &&
               refreshToken.Length <= 256;
    }
}
