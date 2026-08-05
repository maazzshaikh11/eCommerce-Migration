using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Core.Interfaces;

public interface ICheckoutService
{
    OrderConfirmationDto PlaceOrder(int customerId, int? addressId, string shippingMethod, string idempotencyKey);
    bool ValidateAddress(string streetAddress, string city, string stateProvince, string postalCode, string country);
    List<AddressDto> GetCustomerAddresses(int customerId);
    OrderConfirmationDto? GetOrderConfirmation(int orderId);
    List<ShippingOptionDto> GetShippingOptions();
    bool ValidatePayment(PaymentRequest request);
}
