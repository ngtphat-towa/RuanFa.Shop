using ErrorOr;
using RuanFa.Shop.Domain.Commons.Enums;
using RuanFa.Shop.Domain.Commons.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Commons.ValueObjects;

public class DescriptionData : ValueObject
{
    public DescriptionType Type { get; private set; }
    public string Value { get; private set; } = null!;

    private DescriptionData() { }

    private DescriptionData(DescriptionType type, string value)
    {
        Type = type;
        Value = value;
    }

    public static ErrorOr<DescriptionData> Create(DescriptionType type, string? value)
    {
        if (!Enum.IsDefined(typeof(DescriptionType), type))
            return DomainErrors.Description.InvalidType;

        if (string.IsNullOrWhiteSpace(value))
            return DomainErrors.Description.EmptyValue;

        if (value.Length > 4000)
            return DomainErrors.Description.ValueTooLong;

        return new DescriptionData(type, value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Type;
        yield return Value;
    }
}
