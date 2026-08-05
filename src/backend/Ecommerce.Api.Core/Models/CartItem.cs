namespace Ecommerce.Api.Core.Models;

public class CartItem
{
    public int Id { get; set; }
    public string? SessionId { get; set; }
    public int ProductId { get; set; }
    public int? VariantId { get; set; }
    public int? CustomerId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedDate { get; set; }
    public Product? Product { get; set; }
    public ProductVariant? Variant { get; set; }
}
