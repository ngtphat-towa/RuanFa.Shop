namespace RuanFa.Shop.Infrastructure.Accounts.Models;
/// <summary>
/// Represents the response from Facebook token validation.
/// </summary>
public record FacebookTokenValidationResponse
{
    /// <summary>
    /// Gets or sets the validation data.
    /// </summary>
    public FacebookTokenValidationData? Data { get; init; }
}

/// <summary>
/// Represents the data from Facebook token validation.
/// </summary>
public record FacebookTokenValidationData
{
    /// <summary>
    /// Gets or sets the app ID.
    /// </summary>
    public string? AppId { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the token is valid.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Gets or sets the error details, if any.
    /// </summary>
    public FacebookTokenError? Error { get; init; }
}

/// <summary>
/// Represents an error from Facebook token validation.
/// </summary>
public record FacebookTokenError
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string? Message { get; init; }
}

/// <summary>
/// Represents user data retrieved from Facebook.
/// </summary>
public record FacebookUserData
{
    /// <summary>
    /// Gets or sets the user's ID.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Gets or sets the user's email.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets or sets the user's profile picture.
    /// </summary>
    public FacebookPicture? Picture { get; init; }
}

/// <summary>
/// Represents the picture data from Facebook.
/// </summary>
public record FacebookPicture
{
    /// <summary>
    /// Gets or sets the picture data.
    /// </summary>
    public FacebookPictureData? Data { get; init; }
}

/// <summary>
/// Represents the picture data details from Facebook.
/// </summary>
public record FacebookPictureData
{
    /// <summary>
    /// Gets or sets the picture URL.
    /// </summary>
    public string? Url { get; init; }
}
