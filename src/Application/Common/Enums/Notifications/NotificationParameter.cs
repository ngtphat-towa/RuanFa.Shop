namespace RuanFa.Shop.Application.Common.Enums.Notifications;

public enum NotificationParameter
{
    // System-related parameters
    SystemName,                // Name of the system or application
    SupportEmail,              // Support email address for customer service
    SupportPhone,              // Support phone number for customer service

    // User-related parameters
    UserName,                  // Username of the user
    UserEmail,                 // Email address of the user
    UserFullName,              // Full name of the user
    UserFirstName,             // First name of the user
    UserLastName,              // Last name of the user
    UserProfileUrl,            // URL to the user's profile page
    OtpCode,                   // 2FA OTP

    // Order-related parameters
    OrderId,                   // Unique identifier for the order
    OrderDate,                 // Date the order was placed
    OrderTotal,                // Total cost of the order
    OrderStatus,               // Status of the order (e.g., Shipped, Delivered, etc.)
    OrderTrackingNumber,       // Tracking number for the order shipment
    OrderTrackingUrl,          // URL to track the shipment of the order
    OrderItems,                // List of items in the order

    // Payment-related parameters
    PaymentStatus,             // Status of the payment (e.g., Successful, Failed)
    PaymentAmount,             // Amount paid for the order
    PaymentMethod,             // Payment method used (e.g., Credit Card, PayPal)

    // Link-related parameters
    ActiveUrl,                 // URL used for activating the user's account or application
    ResetPasswordUrl,          // URL for resetting the user's password
    UnsubscribeUrl,            // URL for unsubscribing from marketing emails
    SurveyUrl,                 // URL  for marketing survey

    // Time-related parameters
    CreatedDateTime,           // Date and time when the user or order was created
    ExpiryDateTime,            // Expiry date and time for a promotion or offer
    DeliveryDate,              // Expected or actual delivery date for the order

    // Promotional parameters
    PromoCode,                 // The promotional code applied to the order
    PromoDiscount,             // Discount amount applied to the order via promo code
    PromoUrl,                  // URL for the promotional offer or sale page

    // General parameters
    SiteUrl,                   // Base URL of the e-commerce website
    UnsubscribeLink,           // Link to unsubscribe from marketing emails
    CustomerSupportLink        // Link to customer support page
}
