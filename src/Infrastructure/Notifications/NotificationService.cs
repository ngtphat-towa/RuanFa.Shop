using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RuanFa.Shop.Application.Common.Constants.Notifications;
using RuanFa.Shop.Application.Common.Enums.Notifications;
using RuanFa.Shop.Application.Common.Models.Notifications;
using RuanFa.Shop.Application.Common.Notifications;
using RuanFa.Shop.Infrastructure.Data;
using RuanFa.Shop.Infrastructure.Notifications.Emails;
using RuanFa.Shop.Infrastructure.Notifications.Sms;
using Serilog;

namespace RuanFa.Shop.Infrastructure.Notifications;

internal class NotificationService(
IEmailSenderService emailSenderService,
ISmsSenderService smsSenderService,
IServiceScopeFactory serviceScopeFactory)
    : INotificationService
{
    private readonly ApplicationDbContext _dbContext = serviceScopeFactory.CreateScope()
        .ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    public async Task<ErrorOr<Success>> AddNotificationAsync(NotificationData notificationData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Step 1: Validate receivers
            if (notificationData.Receivers == null || notificationData.Receivers.Count == 0)
            {
                return Error.Validation("NotificationService.EmptyReceivers",
                    "At least one receiver must be provided.");
            }

            // Step 2: Validate contact information (e.g., Email or Phone)
            var contactInfoSet = new HashSet<string>(notificationData.Receivers, StringComparer.OrdinalIgnoreCase);

            var existingUsers = await _dbContext.Users
                .Where(m =>
                    notificationData.SendMethodType == NotificationSendMethod.Email &&
                     !string.IsNullOrEmpty(m.Email) && contactInfoSet.Contains(m.Email) ||
                    notificationData.SendMethodType == NotificationSendMethod.SMS &&
                     !string.IsNullOrEmpty(m.PhoneNumber) && contactInfoSet.Contains(m.PhoneNumber))
                .ToListAsync(cancellationToken);

            if (existingUsers.Count == 0)
            {
                return Error.NotFound(
                    "NotificationService.ContactNotFound",
                    "No valid contacts were found.");
            }

            // Step 3: Filter valid contacts based on SendMethodType
            List<string> validContacts = notificationData.SendMethodType switch
            {
                NotificationSendMethod.Email => [.. existingUsers
                    .Where(m => !string.IsNullOrEmpty(m.Email))
                    .Select(m => m.Email!)],
                NotificationSendMethod.SMS => [.. existingUsers
                    .Where(m => !string.IsNullOrEmpty(m.PhoneNumber))
                    .Select(m => m.PhoneNumber!)],
                _ => []
            };

            if (validContacts.Count == 0)
            {
                return Error.Validation(
                    "NotificationService.NoValidContacts",
                    "None of the provided contact information is valid.");
            }

            // Step 4: Apply template and replace placeholders
            string? title = notificationData.Title;
            string? plainTextContent = notificationData.Content;
            string? htmlContent = notificationData.HtmlContent;

            var notificationTemplate = NotificationUseCaseTemplatesData.Templates
                .FirstOrDefault(m => m.UserCase == notificationData.UseCase);

            if (notificationTemplate != null)
            {
                title ??= plainTextContent ?? "Notification";
                plainTextContent ??= notificationTemplate.TemplateContent;
                htmlContent ??= notificationTemplate.HtmlTemplateContent;
            }

            if (string.IsNullOrEmpty(plainTextContent) && string.IsNullOrEmpty(htmlContent))
            {
                return Error.Validation("NotificationService.RequiredContents", "Notification contents are required.");
            }

            // Step 5: Replace placeholders in content
            foreach (var paramValue in notificationData.Values)
            {
                string keyword = $"{{{paramValue.Key}}}";
                plainTextContent = plainTextContent?.Replace(keyword, paramValue.Value);
                htmlContent = htmlContent?.Replace(keyword, paramValue.Value);
            }

            // Step: Validate attachments if email
            if (notificationData.SendMethodType == NotificationSendMethod.Email &&
                notificationData.Attachments != null)
            {
                var maxSizeInBytes = 25 * 1024 * 1024; // Example: 25MB limit for attachments
                var validExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx", ".txt", ".zip"
                };

                foreach (var attachment in notificationData.Attachments)
                {
                    // If it's a file path (local file system), check the file size and extension
                    if (File.Exists(attachment))
                    {
                        var fileInfo = new FileInfo(attachment);
                        if (fileInfo.Length > maxSizeInBytes)
                        {
                            return Error.Validation(
                                "NotificationService.AttachmentSize",
                                $"Attachment {attachment} exceeds the maximum size of {maxSizeInBytes / 1024 / 1024}MB.");
                        }

                        var fileExtension = Path.GetExtension(attachment);
                        if (!validExtensions.Contains(fileExtension))
                        {
                            return Error.Validation(
                                "NotificationService.InvalidAttachmentExtension",
                                $"Attachment {attachment} has an invalid file type. Only the following extensions are allowed: {string.Join(", ", validExtensions)}.");
                        }
                    }
                    // If it's a base64 string, validate that it's a proper base64-encoded string
                    else if (attachment.StartsWith("data:"))
                    {
                        try
                        {
                            var base64Content =
                                attachment.Substring(attachment.IndexOf("base64,", StringComparison.Ordinal) +
                                                     7); // Extract base64 data
                            var buffer = Convert.FromBase64String(base64Content); // Try decoding
                            if (buffer.Length > maxSizeInBytes)
                            {
                                return Error.Validation(
                                    "NotificationService.AttachmentSize",
                                    "Base64-encoded attachment exceeds the maximum size.");
                            }
                        }
                        catch (FormatException)
                        {
                            return Error.Validation("NotificationService.InvalidBase64",
                                "Attachment is not a valid base64-encoded string.");
                        }
                    }
                    // If it's a URL, ensure it's a valid URL
                    else if (!Uri.IsWellFormedUriString(attachment, UriKind.Absolute))
                    {
                        return Error.Validation("NotificationService.InvalidAttachment",
                            "Attachment format is invalid.");
                    }
                }
            }

            // Update notification data
            notificationData.Title = title;
            notificationData.Content = plainTextContent;
            notificationData.HtmlContent = htmlContent;
            notificationData.Receivers = validContacts;
            notificationData.CreatedBy ??= "System";

            // Step 7: Send the notification
            var sendResult = notificationData.SendMethodType switch
            {
                NotificationSendMethod.Email => await emailSenderService.AddEmailNotificationAsync(
                    new EmailNotificationData
                    {
                        UseCase = notificationData.UseCase,
                        ToRecipients = validContacts,
                        Subject = title,
                        PlainTextBody = plainTextContent,
                        HtmlBody = htmlContent,
                        CreatedBy = notificationData.CreatedBy,
                        Attachments = notificationData.Attachments
                    }, cancellationToken),
                NotificationSendMethod.SMS => await smsSenderService.AddSmsNotificationAsync(
                    new SmsNotificationData
                    {
                        UseCase = notificationData.UseCase,
                        PhoneRecipients = validContacts,
                        MessageContent = plainTextContent,
                        CreatedBy = notificationData.CreatedBy,
                        Subject = title,
                    }, cancellationToken),
                _ => Error.Validation(
                    "NotificationService.InvalidSendMethod",
                    "The send method type is not supported.")
            };

            return sendResult.IsError ? (ErrorOr<Success>)sendResult.Errors : (ErrorOr<Success>)Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "NotificationService.AddNotificationAsync");
            return Error.Unexpected(
                code: "NotificationService.Internal",
                description: ex.Message);
        }
    }
}
