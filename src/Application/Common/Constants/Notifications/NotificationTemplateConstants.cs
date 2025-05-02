using RuanFa.Shop.Application.Common.Enums.Notifications;

namespace RuanFa.Shop.Application.Common.Constants.Notifications;

public class NotificationTemplateValue
{
    public NotificationUseCase UserCase { get; init; }
    public NotificationSendMethod SendMethodType { get; init; } = NotificationSendMethod.Email;
    public NotificationTemplateFormat TemplateFormatType { get; init; } = NotificationTemplateFormat.Default;
    public List<NotificationParameter> ParamValues { get; init; } = [];
    public required string Name { get; init; }
    public string? TemplateContent { get; init; }
    public string? HtmlTemplateContent { get; init; }
    public string? Description { get; init; }
}
public static class NotificationUseCaseTemplatesData
{
    public static readonly List<NotificationTemplateValue> Templates =
        [
            // Account Activation Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.SystemActiveEmail,
                Name = "Account Activation Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Dear {UserName},\n\nThank you for signing up with us. Please click the link below to activate your account and get started:\n\n{ActiveUrl}\n\nIf you didn't sign up, please ignore this message.",
                HtmlTemplateContent = "<html><body><p>Dear {UserName},</p><p>Thank you for signing up with us. Please click the link below to activate your account and get started:</p><p><a href='{ActiveUrl}'>Activate Account</a></p><p>If you didn't sign up, please ignore this message.</p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserName,
                    NotificationParameter.ActiveUrl,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users when they register on the platform, asking them to verify their email and activate their account."
            },

            // Password Reset Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.SystemResetPassword,
                Name = "Password Reset Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Dear {UserName},\n\nWe received a request to reset your password. To reset it, please click the link below:\n\n{ResetPasswordUrl}\n\nIf you didn't request this, you can ignore this email.",
                HtmlTemplateContent = "<html><body><p>Dear {UserName},</p><p>We received a request to reset your password. To reset it, please click the link below:</p><p><a href='{ResetPasswordUrl}'>Reset Password</a></p><p>If you didn't request this, you can ignore this email.</p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.ResetPasswordUrl,
                    NotificationParameter.UserName
                ],
                Description = "Sent to users when they request a password reset, including a link to reset their password."
            },

            // Two-Factor Authentication OTP
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.System2faOtp,
                Name = "Two-Factor Authentication OTP",
                TemplateFormatType = NotificationTemplateFormat.Default,
                SendMethodType = NotificationSendMethod.SMS,
                TemplateContent = "Hello {UserFullName},\n\nYour OTP code for two-factor authentication is {OtpCode}. Please use this code to complete your login process.\n\nIf you didn't request this, please contact support immediately.",
                HtmlTemplateContent = "Hello {UserFullName},<br><br>Your OTP code for two-factor authentication is {OtpCode}. Please use this code to complete your login process.<br><br>If you didn't request this, please contact support immediately.",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.OtpCode,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users when they log in or perform sensitive actions that require additional security, providing a one-time password (OTP) for verification."
            },

            // Order Confirmation Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.SystemOrderConfirmation,
                Name = "Order Confirmation Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nYour order has been successfully placed! Here are your order details:\n\nOrder ID: {OrderId}\nTotal: {OrderTotal}\n\nWe will notify you once your order is shipped.\n\nThank you for shopping with us!",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>Your order has been successfully placed! Here are your order details:</p><p><b>Order ID:</b> {OrderId}</p><p><b>Total:</b> {OrderTotal}</p><p>We will notify you once your order is shipped.</p><p>Thank you for shopping with us!</p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.OrderId,
                    NotificationParameter.OrderTotal,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users after a successful order placement to confirm their purchase and provide them with order details."
            },

            // Order Shipped Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.SystemOrderShipped,
                Name = "Order Shipped Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nYour order with ID {OrderId} has been shipped!\n\nYou can track your shipment using the link below:\n\n{OrderTrackingUrl}\n\nThank you for shopping with us!",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>Your order with ID {OrderId} has been shipped!</p><p>You can track your shipment using the link below:</p><p><a href='{OrderTrackingUrl}'>Track Order</a></p><p>Thank you for shopping with us!</p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.OrderId,
                    NotificationParameter.OrderTrackingUrl,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users once their order has been shipped, providing tracking information to follow the shipment status."
            },

            // Order Failed Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.SystemOrderFailed,
                Name = "Order Failed Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nWe regret to inform you that your order with ID {OrderId} has failed.\n\nStatus: {OrderStatus}\n\nPlease contact support if you have any questions or need assistance.",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>We regret to inform you that your order with ID {OrderId} has failed.</p><p>Status: {OrderStatus}</p><p>Please contact support if you have any questions or need assistance.</p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.OrderId,
                    NotificationParameter.OrderStatus,
                    NotificationParameter.SystemName
                ],
                Description = "Sent when an order fails to process, notifying the user and asking them to contact support."
            },

            // Account Update Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.SystemAccountUpdate,
                Name = "Account Update Email",
                TemplateFormatType = NotificationTemplateFormat.Default,
                TemplateContent = "Hello {UserFullName},\n\nWe wanted to let you know that your account information has been successfully updated. If you didn't make this change, please contact our support team immediately.",
                HtmlTemplateContent = "Hello {UserFullName},<br><br>We wanted to let you know that your account information has been successfully updated. If you didn't make this change, please contact our support team immediately.",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users when their account information has been updated, to keep them informed of any changes."
            },

            // Payment Success Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.PaymentSuccessEmail,
                Name = "Payment Success Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nWe're pleased to inform you that your payment of {PaymentAmount} was successfully processed for order ID: {OrderId}.\n\nThank you for your purchase!",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>We're pleased to inform you that your payment of {PaymentAmount} was successfully processed for order ID: {OrderId}.</p><p>Thank you for your purchase!</p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.PaymentAmount,
                    NotificationParameter.OrderId,
                    NotificationParameter.SystemName
                ],
                Description = "Sent after a successful payment to confirm the transaction and provide users with payment details."
            },

            // Payment Failure Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.PaymentFailureEmail,
                Name = "Payment Failure Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nUnfortunately, your payment of {PaymentAmount} failed. Please review your payment details and try again.\n\nStatus: {PaymentStatus}\n\nIf you need assistance, feel free to contact our support team.",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>Unfortunately, your payment of {PaymentAmount} failed. Please review your payment details and try again.</p><p>Status: {PaymentStatus}</p><p>If you need assistance, feel free to contact our support team.</p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.PaymentAmount,
                    NotificationParameter.PaymentStatus,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users when their payment fails, explaining the situation and encouraging them to try again."
            },

            // Payment Refund Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.PaymentRefundNotification,
                Name = "Payment Refund Notification",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nYour payment of {PaymentAmount} has been successfully refunded for order ID: {OrderId}.\n\nIf you have any questions, please contact our support team.",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>Your payment of {PaymentAmount} has been successfully refunded for order ID: {OrderId}.</p><p>If you have any questions, please contact our support team.</p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.PaymentAmount,
                    NotificationParameter.OrderId,
                    NotificationParameter.SystemName
                ],
                Description = "Sent when a payment is refunded, providing users with details about the refund and order status."
            },

            // Marketing Newsletter
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.MarketingNewsletter,
                Name = "Marketing Newsletter",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nCheck out our latest updates and offers in our monthly newsletter!\n\nVisit our website: {SiteUrl}",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>Check out our latest updates and offers in our monthly newsletter!</p><p>Visit our website: <a href='{SiteUrl}'>Click Here</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.SiteUrl,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users who are subscribed to the newsletter, sharing company news, updates, and promotions."
            },

            // Marketing Discount Offer
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.MarketingDiscountOffer,
                Name = "Marketing Discount Offer",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nEnjoy {PromoDiscount} off with promo code {PromoCode}. Don't miss out – shop now!\n\nVisit our shop: {PromoUrl}",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>Enjoy {PromoDiscount} off with promo code {PromoCode}. Don't miss out – shop now!</p><p><a href='{PromoUrl}'>Visit Our Shop</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.PromoCode,
                    NotificationParameter.PromoDiscount,
                    NotificationParameter.PromoUrl,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users to promote a special discount offer, encouraging them to make a purchase with a promotional code."
            },

            // Marketing Survey Email
            new NotificationTemplateValue
            {
                UserCase = NotificationUseCase.MarketingSurvey,
                Name = "Marketing Survey Email",
                TemplateFormatType = NotificationTemplateFormat.Html,
                TemplateContent = "Hello {UserFullName},\n\nWe would love your feedback to improve our services. Please take a moment to complete our survey.\n\n{SurveyUrl}",
                HtmlTemplateContent = "<html><body><p>Hello {UserFullName},</p><p>We would love your feedback to improve our services. Please take a moment to complete our survey.</p><p><a href='{SurveyUrl}'>Take Survey</a></p></body></html>",
                ParamValues =
                [
                    NotificationParameter.UserFullName,
                    NotificationParameter.SurveyUrl,
                    NotificationParameter.SystemName
                ],
                Description = "Sent to users for collecting feedback, encouraging them to participate in a survey to help improve the service."
            }
        ];
}
