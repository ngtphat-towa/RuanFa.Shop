using RuanFa.Shop.Application.Common.Enums.Notifications;
using RuanFa.Shop.Application.Common.Models.Notifications;

namespace RuanFa.Shop.Application.Common.Extensions.Notifications;

public static class NotificationDataBuilder
{
    public static NotificationData WithUseCase(NotificationUseCase useCase = NotificationUseCase.None)
    {
        NotificationData notificationData = new()
        {
            UseCase = useCase
        };
        return notificationData;
    }
    public static NotificationData WithUseCase(this NotificationData notificationData, NotificationUseCase useCase = NotificationUseCase.None)
    {
        notificationData.UseCase = useCase;
        return notificationData;
    }

    public static NotificationData WithSendMethodType(this NotificationData notificationData, NotificationSendMethod sendMethodType)
    {
        notificationData.SendMethodType = sendMethodType;
        return notificationData;
    }

    public static NotificationData AddParam(this NotificationData notificationData, NotificationParameter parameter, string? value)
    {
        notificationData.Values.Add(parameter, value);
        return notificationData;
    }
    public static NotificationData AddParam(this NotificationData notificationData, Dictionary<NotificationParameter, string?> values)
    {
        foreach (var item in values)
        {
            notificationData.Values.Add(item.Key, item.Value);
        }
        return notificationData;
    }

    public static NotificationData WithReceivers(this NotificationData notificationData, List<string> receivers)
    {
        if (receivers.Count == 0)
        {
            return notificationData;
        }

        notificationData.Receivers ??= [];
        notificationData.Receivers.AddRange(receivers);
        return notificationData;
    }
    public static NotificationData WithReceiver(this NotificationData notificationData, string receiver)
    {
        notificationData.Receivers ??= [];
        notificationData.Receivers.Add(receiver);
        return notificationData;
    }

    public static NotificationData WithTitle(this NotificationData notificationData, string title)
    {
        notificationData.Title = title;
        return notificationData;
    }

    public static NotificationData WithContent(this NotificationData notificationData, string content)
    {
        notificationData.Content = content;
        return notificationData;
    }

    public static NotificationData WithHtmlContent(this NotificationData notificationData, string htmlContent)
    {
        notificationData.HtmlContent = htmlContent;
        return notificationData;
    }

    public static NotificationData WithCreatedBy(this NotificationData notificationData, string createdBy)
    {
        notificationData.CreatedBy = createdBy;
        return notificationData;
    }

    public static NotificationData WithAttachments(this NotificationData notificationData, List<string> attachments)
    {
        notificationData.Attachments = attachments;
        return notificationData;
    }


    public static NotificationData CreateNotificationData(NotificationUseCase userCase, List<string> receivers, Dictionary<NotificationParameter, string> parameters)
    {
        var notificationData = new NotificationData
        {
            UseCase = userCase,
            Receivers = receivers
        };

        // Add parameters to the notification data
        foreach (var param in parameters)
        {
            notificationData.Values.Add(param.Key, param.Value);
        }

        return notificationData;
    }
}
