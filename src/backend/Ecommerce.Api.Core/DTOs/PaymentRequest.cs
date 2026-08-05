namespace Ecommerce.Api.Core.DTOs;

public record PaymentRequest(
    string CardholderName,
    string CardNumber,
    string ExpirationDate,
    string Cvv
);
