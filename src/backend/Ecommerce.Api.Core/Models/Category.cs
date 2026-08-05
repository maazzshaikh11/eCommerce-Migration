namespace Ecommerce.Api.Core.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public List<Product> Products { get; set; } = [];
}
