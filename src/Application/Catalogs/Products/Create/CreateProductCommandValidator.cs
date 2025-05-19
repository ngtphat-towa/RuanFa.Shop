using FluentValidation;
using RuanFa.Shop.Domain.Catalogs.Enums;
using RuanFa.Shop.Domain.Catalogs.Errors;

namespace RuanFa.Shop.Application.Catalogs.Products.Create;

internal class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        // --- General Product Validation ---
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithErrorCode(DomainErrors.Product.InvalidName.Code)
                .WithMessage(DomainErrors.Product.InvalidName.Description)
            .MinimumLength(3)
                .WithErrorCode(DomainErrors.Product.InvalidName.Code)
                .WithMessage(DomainErrors.Product.InvalidName.Description)
            .MaximumLength(100)
                .WithErrorCode(DomainErrors.Product.InvalidName.Code)
                .WithMessage(DomainErrors.Product.InvalidName.Description);

        RuleFor(x => x.Sku)
            .NotEmpty()
                .WithErrorCode(DomainErrors.Product.InvalidSku.Code)
                .WithMessage(DomainErrors.Product.InvalidSku.Description)
            .MinimumLength(3)
                .WithErrorCode(DomainErrors.Product.InvalidSku.Code)
                .WithMessage(DomainErrors.Product.InvalidSku.Description)
            .MaximumLength(50)
                .WithErrorCode(DomainErrors.Product.InvalidSku.Code)
                .WithMessage(DomainErrors.Product.InvalidSku.Description)
            .Matches(@"^[a-zA-Z0-9\-_]+$")
                .WithErrorCode(DomainErrors.Product.InvalidSku.Code)
                .WithMessage(DomainErrors.Product.InvalidSku.Description);

        RuleFor(x => x.VariantSku)
            .NotEmpty()
                .WithErrorCode(DomainErrors.ProductVariant.InvalidSku.Code)
                .WithMessage(DomainErrors.ProductVariant.InvalidSku.Description)
            .MinimumLength(3)
                .WithErrorCode(DomainErrors.ProductVariant.InvalidSku.Code)
                .WithMessage(DomainErrors.ProductVariant.InvalidSku.Description)
            .MaximumLength(50)
                .WithErrorCode(DomainErrors.ProductVariant.InvalidSku.Code)
                .WithMessage(DomainErrors.ProductVariant.InvalidSku.Description)
            .Matches(@"^[a-zA-Z0-9\-_]+$")
                .WithErrorCode(DomainErrors.ProductVariant.InvalidSku.Code)
                .WithMessage(DomainErrors.ProductVariant.InvalidSku.Description)
             .When(m => !string.IsNullOrEmpty(m.VariantSku));

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0)
                .WithErrorCode(DomainErrors.Product.InvalidBasePrice.Code)
                .WithMessage(DomainErrors.Product.InvalidBasePrice.Description);

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0).When(x => x.SalePrice.HasValue)
                .WithErrorCode(DomainErrors.Product.InvalidSalePrice.Code)
                .WithMessage(DomainErrors.Product.InvalidSalePrice.Description);

        RuleFor(x => x)
            .Must(x => !x.SalePrice.HasValue || x.SalePrice <= x.BasePrice)
                .WithErrorCode(DomainErrors.Product.SalePriceExceedsBasePrice.Code)
                .WithMessage(DomainErrors.Product.SalePriceExceedsBasePrice.Description);


        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0)
                .WithErrorCode(DomainErrors.Product.InvalidWeight.Code)
                .WithMessage(DomainErrors.Product.InvalidWeight.Description);

        RuleFor(x => x.GroupId)
            .NotEqual(Guid.Empty)
                .WithErrorCode(DomainErrors.Product.InvalidGroupId.Code)
                .WithMessage(DomainErrors.Product.InvalidGroupId.Description);

        RuleFor(x => x.TaxClass)
            .IsInEnum()
                .WithErrorCode(DomainErrors.Product.InvalidTaxClass.Code)
                .WithMessage(DomainErrors.Product.InvalidTaxClass.Description);

        RuleFor(x => x.Status)
            .IsInEnum()
                .WithErrorCode(DomainErrors.Product.InvalidStatus.Code)
                .WithMessage(DomainErrors.Product.InvalidStatus.Description);

        // --- Default Variant Validation ---
        RuleFor(x => x.PriceOffset)
            .InclusiveBetween(-10000m, 10000m)
                .WithErrorCode(DomainErrors.ProductVariant.InvalidPriceOffset.Code)
                .WithMessage(DomainErrors.ProductVariant.InvalidPriceOffset.Description);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
                .WithErrorCode(DomainErrors.ProductVariant.InvalidStockQuantity.Code)
                .WithMessage(DomainErrors.ProductVariant.InvalidStockQuantity.Description);

        RuleFor(x => x.LowStockThreshold)
            .GreaterThanOrEqualTo(0)
                .WithErrorCode(DomainErrors.ProductVariant.InvalidLowStockThreshold.Code)
                .WithMessage(DomainErrors.ProductVariant.InvalidLowStockThreshold.Description);

        // --- Attribute Values Validation ---
        RuleForEach(x => x.AttributeValues)
            .ChildRules(av =>
            {
                // 1. ID & Type
                av.RuleFor(x => x.AttributeId)
                    .NotEqual(Guid.Empty)
                    .WithErrorCode(DomainErrors.VariantAttributeValue.InvalidAttributeId.Code)
                    .WithMessage(DomainErrors.VariantAttributeValue.InvalidAttributeId.Description);

                av.RuleFor(x => x.AttributeType)
                    .IsInEnum()
                    .WithErrorCode(DomainErrors.VariantAttributeValue.InvalidAttributeType.Code)
                    .WithMessage(DomainErrors.VariantAttributeValue.InvalidAttributeType.Description);

                // 2. Option-based vs. Value-based
                var optionTypes = new[]
                {
                    AttributeType.Dropdown,
                    AttributeType.Select,
                    AttributeType.Swatch
                };

                // -- Option-based attributes --
                av.When(x => optionTypes.Contains(x.AttributeType), () =>
                {
                    av.RuleFor(x => x.AttributeOptionId)
                        .NotNull()
                        .Must(id => id != Guid.Empty)
                        .WithErrorCode(DomainErrors.VariantAttributeValue.InvalidAttributeOptionId.Code)
                        .WithMessage(DomainErrors.VariantAttributeValue.InvalidAttributeOptionId.Description);

                    av.RuleFor(x => x.Value)
                        .Null()
                        .WithErrorCode(DomainErrors.VariantAttributeValue.ValueNotSupportedForOptionType.Code)
                        .WithMessage(DomainErrors.VariantAttributeValue.ValueNotSupportedForOptionType.Description);
                });

                // -- Value-based attributes --
                av.When(x => !optionTypes.Contains(x.AttributeType) && x.AttributeType != AttributeType.None, () =>
                {
                    av.RuleFor(x => x.AttributeOptionId)
                        .Null()
                        .WithErrorCode(DomainErrors.VariantAttributeValue.OptionNotSupportedForValueType.Code)
                        .WithMessage(DomainErrors.VariantAttributeValue.OptionNotSupportedForValueType.Description);

                    av.RuleFor(x => x.Value)
                        .NotEmpty()
                        .WithErrorCode(DomainErrors.VariantAttributeValue.InvalidValue.Code)
                        .WithMessage(DomainErrors.VariantAttributeValue.InvalidValue.Description);

                    // Per-type format checks
                    av.When(x => x.AttributeType == AttributeType.Number, () =>
                        av.RuleFor(x => x.Value)
                            .Must(v => int.TryParse(v, out _))
                            .WithErrorCode(DomainErrors.VariantAttributeValue.InvalidNumberFormat.Code)
                            .WithMessage(DomainErrors.VariantAttributeValue.InvalidNumberFormat.Description)
                    );
                    av.When(x => x.AttributeType == AttributeType.Boolean, () =>
                        av.RuleFor(x => x.Value)
                            .Must(v => bool.TryParse(v, out _))
                            .WithErrorCode(DomainErrors.VariantAttributeValue.InvalidBooleanFormat.Code)
                            .WithMessage(DomainErrors.VariantAttributeValue.InvalidBooleanFormat.Description)
                    );
                    av.When(x => x.AttributeType == AttributeType.DateTime, () =>
                        av.RuleFor(x => x.Value)
                            .Must(v => DateTime.TryParse(v, out _))
                            .WithErrorCode(DomainErrors.VariantAttributeValue.InvalidDateTimeFormat.Code)
                            .WithMessage(DomainErrors.VariantAttributeValue.InvalidDateTimeFormat.Description)
                    );
                    av.When(x => x.AttributeType == AttributeType.Decimal, () =>
                        av.RuleFor(x => x.Value)
                            .Must(v => decimal.TryParse(v, out _))
                            .WithErrorCode(DomainErrors.VariantAttributeValue.InvalidDecimalFormat.Code)
                            .WithMessage(DomainErrors.VariantAttributeValue.InvalidDecimalFormat.Description)
                    );
                });
            })
            .When(x => x.AttributeValues != null);

        // No duplicates of the same AttributeId
        RuleFor(x => x.AttributeValues)
            .Must(vals => vals == null || vals.GroupBy(v => v.AttributeId).All(g => g.Count() == 1))
            .WithErrorCode(DomainErrors.VariantAttributeValue.DuplicateAttributeValue.Code)
            .WithMessage(DomainErrors.VariantAttributeValue.DuplicateAttributeValue.Description)
            .When(x => x.AttributeValues != null);

        // --- Product Images Validation ---
        RuleFor(x => x.Images)
            .Must(imgs => imgs == null || imgs.Count(i => i.IsDefault) <= 1)
                .WithErrorCode(DomainErrors.ProductImage.MultipleDefaultImages.Code)
                .WithMessage(DomainErrors.ProductImage.MultipleDefaultImages.Description)
            .When(x => x.Images != null);

        RuleForEach(x => x.Images)
            .ChildRules(img =>
            {
                img.RuleFor(x => x.Url)
                    .NotEmpty()
                        .WithErrorCode(DomainErrors.ProductImage.EmptyImageUrl.Code)
                        .WithMessage(DomainErrors.ProductImage.EmptyImageUrl.Description)
                    .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        .WithErrorCode(DomainErrors.ProductImage.InvalidImageUrl("Invalid absolute URI").Code)
                        .WithMessage(DomainErrors.ProductImage.InvalidImageUrl("The URL is not a valid absolute URI.").Description);

                img.RuleFor(x => x.Alt)
                    .NotEmpty()
                        .WithErrorCode(DomainErrors.ProductImage.EmptyAltText.Code)
                        .WithMessage(DomainErrors.ProductImage.EmptyAltText.Description)
                    .MaximumLength(125)
                        .WithErrorCode(DomainErrors.ProductImage.AltTextTooLong.Code)
                        .WithMessage(DomainErrors.ProductImage.AltTextTooLong.Description);

                img.RuleFor(x => x.ImageType)
                    .IsInEnum()
                        .WithErrorCode(DomainErrors.ProductImage.InvalidImageType.Code)
                        .WithMessage(DomainErrors.ProductImage.InvalidImageType.Description);
            })
            .When(x => x.Images != null);
    }
}
