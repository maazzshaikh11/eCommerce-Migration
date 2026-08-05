namespace Ecommerce.Api.Core.DTOs;

public class ProductListResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
}
