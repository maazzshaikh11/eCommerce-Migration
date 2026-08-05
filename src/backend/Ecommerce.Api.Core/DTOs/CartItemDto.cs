namespace Ecommerce.Api.Core.DTOs;

public record CartItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string? VariantName,
    string? ImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal
);
