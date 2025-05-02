using RuanFa.Shop.Application.Common.Enums.Notifications;

namespace RuanFa.Shop.Application.Common.Models.Notifications;

public class NotificationData
{
    /// <summary>
    /// Gets or sets the use case or context of the notification (e.g., "Password Reset", "Order Confirmation").
    /// This provides important context for the recipient and the notification processing.
    /// </summary>
    public required NotificationUseCase UseCase { get; set; } = NotificationUseCase.None;

    /// <summary>
    /// Gets or sets the method used for sending the notification. 
    /// Default is Email, but it could be set to other methods like SMS, Push Notification, etc.
    /// </summary>
    public NotificationSendMethod SendMethodType { get; set; } = NotificationSendMethod.Email;

    /// <summary>
    /// Gets or sets a collection of key-value pairs representing parameters and their corresponding values.
    /// These could be placeholders (e.g., "{Username}") and their actual values (e.g., "JohnDoe").
    /// This allows dynamic content to be injected into the notification.
    /// </summary>
    public Dictionary<NotificationParameter, string?> Values { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of receivers for the notification. 
    /// Depending on the <see cref="SendMethodType"/>, this could be email addresses (for email) or phone numbers (for SMS).
    /// </summary>
    public List<string>? Receivers { get; set; } = [];

    /// <summary>
    /// Gets or sets the title or subject of the notification. 
    /// This is typically used for email notifications as the subject line.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the plain text content of the notification (e.g., the body of an email or SMS message).
    /// This will be the main body text for the notification.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the HTML content of the notification, if applicable.
    /// This is primarily used for email notifications to provide rich formatting.
    /// For SMS, this field will be ignored as SMS only supports plain text.
    /// </summary>
    public string? HtmlContent { get; set; }

    /// <summary>
    /// Gets or sets the identifier or name of the user or service that initiated the notification.
    /// This could represent a system service or a user who triggered the notification.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets a list of file attachments for the notification, if applicable.
    /// This could be file paths or URLs for attachments in email notifications.
    /// For SMS, attachments are not supported and this property can be ignored.
    /// </summary>
    public List<string>? Attachments { get; set; } = [];
}
