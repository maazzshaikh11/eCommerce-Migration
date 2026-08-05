namespace Ecommerce.Api.Core.DTOs;

public record AddressDto(
    int Id,
    string StreetAddress,
    string City,
    string? StateProvince,
    string? PostalCode,
    string Country,
    bool IsDefault
);
