namespace Ecommerce.Api.Core.DTOs;

public record PlaceOrderRequest(
    int? AddressId,
    string? ShippingMethod,
    string? IdempotencyKey = null
);
