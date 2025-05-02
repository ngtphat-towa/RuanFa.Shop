using ErrorOr;
using RuanFa.Shop.Application.Common.Models.Notifications;

namespace RuanFa.Shop.Infrastructure.Notifications.Emails;
public interface IEmailSenderService
{
    Task<ErrorOr<Success>> AddEmailNotificationAsync(
        EmailNotificationData notificationData,
        CancellationToken cancellationToken = default);
}
