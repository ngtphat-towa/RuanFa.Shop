using FluentValidation;
using RuanFa.Shop.Domain.Accounts.Errors;
using RuanFa.Shop.Domain.Accounts.ValueObjects;

namespace RuanFa.Shop.Application.Accounts.Validation;

internal class FashionPreferencesValidator : AbstractValidator<FashionPreference>
{
    public FashionPreferencesValidator()
    {
        RuleFor(x => x.ClothingSize)
            .NotEmpty()
            .WithMessage(DomainErrors.FashionPreferences.ClothingSizeRequired.Description)
            .WithErrorCode(DomainErrors.FashionPreferences.ClothingSizeRequired.Code);

        RuleFor(x => x.FavoriteCategories)
            .NotEmpty()
            .WithMessage(DomainErrors.FashionPreferences.FavoriteCategoriesRequired.Description)
            .WithErrorCode(DomainErrors.FashionPreferences.FavoriteCategoriesRequired.Code)
            .Must(categories => categories.All(c => !string.IsNullOrEmpty(c)))
            .WithMessage(DomainErrors.FashionPreferences.InvalidCategoryName.Description)
            .WithErrorCode(DomainErrors.FashionPreferences.InvalidCategoryName.Code);
    }
}
