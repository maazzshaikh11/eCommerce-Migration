namespace Ecommerce.Api.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? ParentCategoryId { get; set; }
    public List<CategoryDto>? Children { get; set; }
}
