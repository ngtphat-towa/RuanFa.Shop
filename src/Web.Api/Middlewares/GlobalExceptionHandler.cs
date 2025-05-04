using ErrorOr;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RuanFa.Shop.Web.Api.Middlewares;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred: {ExceptionMessage}", exception.Message);

        // Map exception to appropriate Error(s)
        List<Error> errors = MapExceptionToErrors(exception);

        // Convert to ProblemDetails
        var problemDetails = CreateProblemDetails(errors, httpContext.Request.Path);

        // Set response status code and content type
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static ProblemDetails CreateProblemDetails(List<Error> errors, string path)
    {
        var firstError = errors.FirstOrDefault();
     
        int statusCode = MapErrorTypeToStatusCode(firstError.Type);

        if (firstError.Type == ErrorType.Validation)
        {
            // Group validation errors by property name (using Code as property name)
            var errorsByProperty = errors
                .GroupBy(e => e.Code.Contains('.') ? e.Code.Split('.')[0] : e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray());

            return new ValidationProblemDetails(errorsByProperty)
            {
                Title = "Validation Failed",
                Type = GetProblemTypeUri(statusCode),
                Status = statusCode,
                Instance = path
            };
        }

        return new ProblemDetails
        {
            Title = firstError.Code,
            Detail = firstError.Description,
            Status = statusCode,
            Type = GetProblemTypeUri(statusCode),
            Instance = path
        };
    }

    private static List<Error> MapExceptionToErrors(Exception exception)
    {
        // Handle FluentValidation.ValidationException if you're using FluentValidation
        if (exception.GetType().Name == "ValidationException" &&
            exception.GetType().Namespace == "FluentValidation")
        {
            // Use reflection to avoid direct dependency if not installed
            var errorsProperty = exception.GetType().GetProperty("Errors");
            if (errorsProperty != null)
            {
                var validationFailures = errorsProperty.GetValue(exception) as IEnumerable<object>;
                if (validationFailures != null)
                {
                    var errors = new List<Error>();
                    foreach (var failure in validationFailures)
                    {
                        var propertyNameProp = failure.GetType().GetProperty("PropertyName");
                        var errorMessageProp = failure.GetType().GetProperty("ErrorMessage");
                        var errorCodeProp = failure.GetType().GetProperty("ErrorCode");

                        if (propertyNameProp != null && errorMessageProp != null)
                        {
                            string propertyName = propertyNameProp.GetValue(failure)?.ToString() ?? "Unknown";
                            string errorMessage = errorMessageProp.GetValue(failure)?.ToString() ?? "Validation failed";
                            string errorCode = errorCodeProp?.GetValue(failure)?.ToString() ?? "ValidationError";

                            errors.Add(Error.Validation(
                                code: $"{propertyName}.{errorCode}",
                                description: errorMessage));
                        }
                    }

                    if (errors.Any())
                    {
                        return errors;
                    }
                }
            }
        }

        // For other exceptions, return a single error
        var singleError = exception switch
        {
            UnauthorizedAccessException => Error.Unauthorized(
                code: "Api.UnauthorizedAccess",
                description: exception.Message),

            FileNotFoundException => Error.NotFound(
                code: "Resource.NotFound",
                description: exception.Message),

            KeyNotFoundException => Error.NotFound(
                code: "Entity.NotFound",
                description: exception.Message),

            InvalidOperationException => Error.Validation(
                code: "Operation.Invalid",
                description: exception.Message),

            ArgumentException => Error.Validation(
                code: "Argument.Invalid",
                description: exception.Message),

            TimeoutException => Error.Failure(
                code: "Operation.Timeout",
                description: exception.Message),

            // Default case for unexpected exceptions
            _ => Error.Unexpected(
                code: "InternalServerError.UnknownError",
                description: exception.Message)
        };

        return new List<Error> { singleError };
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
}
