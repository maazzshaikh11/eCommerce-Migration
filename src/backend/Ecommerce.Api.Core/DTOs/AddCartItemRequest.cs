namespace Ecommerce.Api.Core.DTOs;

public record AddCartItemRequest(
    int ProductId,
    int? VariantId,
    int Quantity
);
