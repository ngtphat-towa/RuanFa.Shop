using RuanFa.Shop.Application.Common.Enums.Notifications;

namespace RuanFa.Shop.Application.Common.Models.Notifications;

public record SmsNotificationData
{
    public required NotificationUseCase UseCase { get; set; }
    public List<string>? PhoneRecipients { get; set; } = [];
    public string? Subject { get; set; }
    public string? MessageContent { get; set; }
    public string? CreatedBy { get; set; }
}
