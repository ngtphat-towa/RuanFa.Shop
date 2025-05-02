using Mapster;
using RuanFa.Shop.Application.Common.Models.Notifications;

namespace RuanFa.Shop.Application.Common.Mappings;

public class NotificationProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<NotificationData, EmailNotificationData>()
            .Map(dest => dest.UseCase, src => src.UseCase)
            .Map(dest => dest.ToRecipients, src => src.Receivers)
            .Map(dest => dest.Subject, src => src.Title)
            .Map(dest => dest.PlainTextBody, src => src.Content)
            .Map(dest => dest.HtmlBody, src => src.HtmlContent)
            .Map(dest => dest.CreatedBy, src => src.CreatedBy)
            .Map(dest => dest.Attachments, src => src.Attachments);

        config.NewConfig<NotificationData, SmsNotificationData>()
            .Map(dest => dest.UseCase, src => src.UseCase)
            .Map(dest => dest.Subject, src => src.Title)
            .Map(dest => dest.PhoneRecipients, src => src.Receivers)
            .Map(dest => dest.MessageContent, src => src.Content)
            .Map(dest => dest.CreatedBy, src => src.CreatedBy);
    }
}
