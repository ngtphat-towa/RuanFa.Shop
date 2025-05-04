using RuanFa.Shop.Domain.Commons.Enums;
using RuanFa.Shop.Domain.Commons.ValueObjects;

namespace RuanFa.Shop.Tests.Shared.Constants;
public static partial class DataConstants
{
    public static class Addresses
    {
        public const string AddressLine1 = "123 Main St";
        public const string AddressLine2 = "5456 Main St";
        public const string City = "New York";
        public const string State = "NY";
        public const string Country = "USA";
        public const string PostalCode = "10001";
    }

    public static class UserAddresses
    {
        public const string DeliveryInstructions = "Leave at front desk";
        public const string BoutiquePickupLocation = "Boutique A";

        public static readonly UserAddress ShippingAddress = UserAddress.Create(
            addressLine1: Addresses.AddressLine1,
            addressLine2: Addresses.AddressLine1,
            city: Addresses.City,
            state: Addresses.State,
            country: Addresses.Country,
            postalCode: Addresses.PostalCode,
            boutiquePickupLocation: BoutiquePickupLocation,
            deliveryInstructions: DeliveryInstructions,
            type: AddressType.Shipping,
            isDefault: true);

        public static readonly UserAddress BillingAddress = UserAddress.Create(
            addressLine1: Addresses.AddressLine1,
            addressLine2: Addresses.AddressLine1,
            city: Addresses.City,
            state: Addresses.State,
            country: Addresses.Country,
            postalCode: Addresses.PostalCode,
            boutiquePickupLocation: BoutiquePickupLocation,
            deliveryInstructions: DeliveryInstructions,
            type: AddressType.Billing,
            isDefault: false);
    }
}
