namespace Ecommerce.Api.Core.DTOs;

public record CartSummaryDto(
    int ItemCount,
    decimal SubTotal,
    decimal Total
);
