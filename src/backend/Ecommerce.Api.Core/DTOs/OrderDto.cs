namespace Ecommerce.Api.Core.DTOs;

public record OrderDto(
    int Id,
    string OrderNumber,
    DateTime OrderDate,
    string OrderStatus,
    decimal SubTotal,
    decimal ShippingTotal,
    decimal TaxTotal,
    decimal GrandTotal
);
