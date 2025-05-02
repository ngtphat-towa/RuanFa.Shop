namespace RuanFa.Shop.Infrastructure.Accounts.Models;
/// <summary>
/// Represents the payload extracted from a social provider's token.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="IsVerified">Indicates if the email is verified by the provider.</param>
/// <param name="ProviderId">The user's unique ID from the provider.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="PictureUrl">The URL to the user's profile picture.</param>
public record SocialPayload(
    string Email,
    bool IsVerified,
    string? ProviderId,
    string? FirstName,
    string? LastName,
    string? PictureUrl);
