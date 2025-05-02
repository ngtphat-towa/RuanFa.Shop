using ErrorOr;
using RuanFa.Shop.Application.Common.Models.Notifications;

namespace RuanFa.Shop.Infrastructure.Notifications.Sms;
public interface ISmsSenderService
{
    public Task<ErrorOr<Success>> AddSmsNotificationAsync(SmsNotificationData notificationData, CancellationToken cancellationToken = default);
}
