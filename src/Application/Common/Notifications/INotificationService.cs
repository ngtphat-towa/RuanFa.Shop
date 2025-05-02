using ErrorOr;
using RuanFa.Shop.Application.Common.Models.Notifications;

namespace RuanFa.Shop.Application.Common.Notifications;

public interface INotificationService
{
    Task<ErrorOr<Success>> AddNotificationAsync(NotificationData notification, CancellationToken cancellationToken);
}
