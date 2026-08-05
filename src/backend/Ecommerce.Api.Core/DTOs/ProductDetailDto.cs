namespace Ecommerce.Api.Core.DTOs;

public class ProductDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CategoryName { get; set; }
    public List<ProductImageDto> Images { get; set; } = [];
    public List<ProductVariantDto> Variants { get; set; } = [];
}

public class ProductImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

public class ProductVariantDto
{
    public int Id { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string VariantValue { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
}
