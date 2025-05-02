using ErrorOr;
using Microsoft.Extensions.Options;
using RuanFa.Shop.Application.Common.Models.Notifications;
using RuanFa.Shop.Infrastructure.Settings;
using Serilog;

namespace RuanFa.Shop.Infrastructure.Notifications.Sms;
internal class SmsSenderService(IOptions<SmsSettings> smsSettings) : ISmsSenderService
{
    private readonly SmsSettings _smsSettings = smsSettings.Value;

    public async Task<ErrorOr<Success>> AddSmsNotificationAsync(SmsNotificationData notificationData, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;
            Log.Debug("Debug.SmsSender", notificationData);

            return Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "SmsSenderService.AddSmsNotificationAsync");
            return Error.Unexpected(
                code: "SmsNotification.Internal",
                description: ex.Message);
        }
    }
}
