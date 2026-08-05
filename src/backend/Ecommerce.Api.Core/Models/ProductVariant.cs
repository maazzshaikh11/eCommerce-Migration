namespace Ecommerce.Api.Core.Models;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string VariantValue { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
}
