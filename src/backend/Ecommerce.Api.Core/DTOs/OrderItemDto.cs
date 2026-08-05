namespace Ecommerce.Api.Core.DTOs;

public record OrderItemDto(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);
