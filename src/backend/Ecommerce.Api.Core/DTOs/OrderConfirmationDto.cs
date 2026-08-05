namespace Ecommerce.Api.Core.DTOs;

public record OrderConfirmationDto(
    int OrderId,
    string OrderNumber,
    DateTime OrderDate,
    string OrderStatus,
    decimal SubTotal,
    decimal ShippingTotal,
    decimal TaxTotal,
    decimal GrandTotal,
    string? ShippingAddressFormatted,
    List<OrderItemDto> Items
);
