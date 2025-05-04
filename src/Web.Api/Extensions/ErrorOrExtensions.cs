using ErrorOr;
using Microsoft.AspNetCore.Mvc;
namespace RuanFa.Shop.Web.Api.Extensions;
public static class ErrorOrExtensions
{
    #region TypedResults Extensions
    public static IResult ToTypedResult<T>(this ErrorOr<T> result)
    {
        return result.Match(
            TypedResults.Ok,
            errors => ToProblemDetails(errors));
    }
    public static IResult ToTypedResultCreated<T>(this ErrorOr<T> result, string locationUrl)
    {
        return result.Match(
            value => TypedResults.Created(locationUrl, value),
            errors => ToProblemDetails(errors));
    }
    public static IResult ToTypedResultNoContent(this ErrorOr<Updated> result)
    {
        return result.Match(
            _ => TypedResults.NoContent(),
            errors => ToProblemDetails(errors));
    }
    public static IResult ToTypedResultDeleted(this ErrorOr<Deleted> result)
    {
        return result.Match(
            _ => TypedResults.NoContent(),
            errors => ToProblemDetails(errors));
    }
    #endregion
    #region ProblemDetails Conversions
    public static IResult ToProblemDetails(List<Error> errors)
    {
        var firstError = errors[0];
        if (firstError.Type == ErrorType.Validation)
        {
            // Group validation errors by property name (using Code as property name)
            var errorsByProperty = errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray());

            return Results.ValidationProblem(
                errorsByProperty,
                title: "Validation Failed",
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.1");
        }
        var statusCode = MapErrorTypeToStatusCode(firstError.Type);
        return Results.Problem(
            title: firstError.Code,
            detail: firstError.Description,
            statusCode: statusCode,
            type: GetProblemTypeUri(statusCode));
    }
    public static IActionResult ToProblemDetailsActionResult(this List<Error> errors)
    {
        var firstError = errors[0];
        var statusCode = MapErrorTypeToStatusCode(firstError.Type);
        if (firstError.Type == ErrorType.Validation)
        {
            // Group validation errors by property name (using Code as property name)
            var errorsByProperty = errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray());

            return new BadRequestObjectResult(new ValidationProblemDetails(errorsByProperty)
            {
                Title = "Validation Failed",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = statusCode
            });
        }
        var problemDetails = new ProblemDetails
        {
            Title = firstError.Code,
            Detail = firstError.Description,
            Status = statusCode,
            Type = GetProblemTypeUri(statusCode)
        };
        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
    private static int MapErrorTypeToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Failure => StatusCodes.Status500InternalServerError,
        ErrorType.Unexpected => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
    };
    private static string GetProblemTypeUri(int statusCode) =>
        $"https://httpstatuses.com/{statusCode}";
    #endregion
}
