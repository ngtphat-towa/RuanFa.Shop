using ErrorOr;
using RuanFa.Shop.Domain.Todo.Errors;
using RuanFa.Shop.SharedKernel.Models.Domains;

namespace RuanFa.Shop.Domain.Todo.ValueObjects;

public class Colour(string code) : ValueObject
{
    public static ErrorOr<Colour> Create(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return DomainErrors.Colour.EmptyCode;
        }
        var colour = new Colour(code);

        if (!SupportedColours.Contains(colour))
        {
            return DomainErrors.Colour.NotSupported(code);
        }

        return colour;
    }

    public static Colour White => new("#FFFFFF");

    public static Colour Red => new("#FF5733");

    public static Colour Orange => new("#FFC300");

    public static Colour Yellow => new("#FFFF66");

    public static Colour Green => new("#CCFF99");

    public static Colour Blue => new("#6666FF");

    public static Colour Purple => new("#9966CC");

    public static Colour Grey => new("#999999");

    public string Code { get; private set; } = string.IsNullOrWhiteSpace(code) ? "#000000" : code;

    public static implicit operator string(Colour colour)
    {
        return colour.ToString();
    }

    public static explicit operator Colour(string code)
    {
        return Create(code).Value;
    }

    public override string ToString()
    {
        return Code;
    }

    protected static IEnumerable<Colour> SupportedColours
    {
        get
        {
            yield return White;
            yield return Red;
            yield return Orange;
            yield return Yellow;
            yield return Green;
            yield return Blue;
            yield return Purple;
            yield return Grey;
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
