namespace RuanFa.Shop.Application.Common.Enums.Notifications;

public enum NotificationUseCase
{
    None,
    // System notifications
    SystemActiveEmail,               // Account activation email
    SystemResetPassword,            // Password reset email
    System2faOtp,                   // Two-factor authentication OTP
    SystemOrderConfirmation,        // Order confirmation email
    SystemOrderShipped,             // Order shipped email
    SystemOrderFailed,              // Order failed email
    SystemAccountUpdate,            // Account update email
    SystemPromotionEmail,           // Promotion email

    // User notifications
    UserWelcomeEmail,               // Welcome email to the user
    UserProfileUpdateEmail,         // Profile update notification
    UserPasswordChangeNotification, // Password change notification

    // Payment notifications
    PaymentSuccessEmail,            // Payment success notification
    PaymentFailureEmail,            // Payment failure notification
    PaymentRefundNotification,      // Payment refund notification

    // Marketing notifications
    MarketingNewsletter,            // Newsletter email
    MarketingDiscountOffer,         // Discount or sales offer email
    MarketingSurvey,                // Customer survey invitation     
}
