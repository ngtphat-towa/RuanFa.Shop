using ErrorOr;

namespace RuanFa.Shop.Domain.Todo.Errors;

public static partial class DomainErrors
{
    public static class Colour
    {
        public static Error NotSupported(string colorCode) =>
                 Error.Failure(
                     code: "Color.NotSupported",
                     description: $"The color with code {colorCode} is not supported.");
        public static Error EmptyCode =>
               Error.Failure(
                   code: "Color.EmptyCode",
                   description: $"The color with with empty code.");
    }
}
