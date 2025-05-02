using RuanFa.Shop.Application.Common.Enums.Notifications;

namespace RuanFa.Shop.Application.Common.Models.Notifications;

public record EmailNotificationData
{
    public required NotificationUseCase UseCase { get; set; }
    public List<string>? ToRecipients { get; set; } 
    public string? Subject { get; set; }
    public string? PlainTextBody { get; set; }
    public string? HtmlBody { get; set; }
    public string? CreatedBy { get; set; }
    public List<string>? Attachments { get; set; }
}
