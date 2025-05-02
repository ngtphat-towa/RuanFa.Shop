using ErrorOr;
using FluentEmail.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using RuanFa.Shop.Application.Common.Models.Notifications;
using RuanFa.Shop.Infrastructure.Settings;
using Serilog;

namespace RuanFa.Shop.Infrastructure.Notifications.Emails;
internal class EmailSenderService(IOptions<EmailSettings> emailSettings, IFluentEmail fluentEmail) : IEmailSenderService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;
    private readonly IFluentEmail _fluentEmail = fluentEmail;

    public async Task<ErrorOr<Success>> AddEmailNotificationAsync(EmailNotificationData notificationData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate required fields
            if (notificationData.ToRecipients == null || notificationData.ToRecipients.Count == 0)
            {
                return Error.Validation(
                    code: "EmailNotification.InvalidRecipients",
                    description: "Recipients list cannot be empty.");
            }

            if (string.IsNullOrEmpty(notificationData.Subject))
            {
                return Error.Validation(
                    code: "EmailNotification.MissingSubject",
                    description: "Subject is required.");
            }

            if (string.IsNullOrEmpty(notificationData.PlainTextBody) && string.IsNullOrEmpty(notificationData.HtmlBody))
            {
                return Error.Validation(
                    code: "EmailNotification.MissingBody",
                    description: "At least one body (plain text or HTML) is required.");
            }

            // Validate attachment files exist
            if (notificationData.Attachments != null && notificationData.Attachments.Count != 0)
            {
                var missingAttachments = notificationData.Attachments.Where(a => !File.Exists(a)).ToList();
                if (missingAttachments.Count != 0)
                {
                    return Error.Validation(
                        code: "EmailNotification.InvalidAttachments",
                        description:
                        $"The following attachments were not found: {string.Join(", ", missingAttachments)}");
                }
            }

            // Create the email message with From address
            var email = _fluentEmail
                .SetFrom(_emailSettings.FromEmail, _emailSettings.FromName)
                .To(notificationData.ToRecipients.Select(m => new FluentEmail.Core.Models.Address(m)))
                .Subject(notificationData.Subject)
                .PlaintextAlternativeBody(notificationData.PlainTextBody)
                .Body(notificationData.HtmlBody);

            // Add attachments with MIME type detection
            if (notificationData.Attachments?.Any() ?? false)
            {
                var contentTypeProvider = new FileExtensionContentTypeProvider();
                foreach (var attachmentPath in notificationData.Attachments)
                {
                    var attachmentBytes = await File.ReadAllBytesAsync(attachmentPath, cancellationToken);
                    email.Attach(new FluentEmail.Core.Models.Attachment
                    {
                        Filename = Path.GetFileName(attachmentPath),
                        Data = new MemoryStream(attachmentBytes),
                        ContentType = contentTypeProvider.TryGetContentType(
                            attachmentPath, out var contentType)
                            ? contentType
                            : "application/octet-stream"
                    });
                }
            }

            // Send the email
            var sendResult = await email.SendAsync();

            if (!sendResult.Successful)
            {
                Log.Error("Failed to send email notification. Errors: {Errors}", sendResult.ErrorMessages);
                return Error.Unexpected(
                    code: "EmailNotification.SendFailed",
                    description: $"Failed to send email: {string.Join(", ", sendResult.ErrorMessages)}");
            }

            Log.Information("Email notification sent successfully to {Recipients}", notificationData.ToRecipients);
            return Result.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An unexpected error occurred while sending email notification");
            return Error.Unexpected(
                code: "EmailNotification.InternalError",
                description: $"An internal error occurred: {ex.Message}");
        }
    }
}
