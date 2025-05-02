using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace RuanFa.Shop.Infrastructure.Security.Extensions;

/// <summary>
/// Provides extension methods for converting Identity framework results to application errors.
/// </summary>
public static class IdentityResultExtensions
{
    /// <summary>
    /// Converts a collection of <see cref="IdentityError"/> objects to a list of application errors.
    /// </summary>
    /// <param name="errors">The collection of Identity errors to convert.</param>
    /// <param name="code">The base error code to use if an error has no specific code.</param>
    /// <param name="type">The type of error (default is <see cref="ErrorType.Validation"/>).</param>
    /// <returns>A list of <see cref="Error"/> objects representing the Identity errors.</returns>
    public static List<Error> ToApplicationResult(
        this IEnumerable<IdentityError> errors,
        string code,
        int type = (int)ErrorType.Validation)
    {
        return errors.Select(error => Error.Custom(
            type: type,
            code: string.IsNullOrWhiteSpace(error.Code) ? $"Account.{code}" : error.Code,
            description: error.Description)).ToList();
    }
}
