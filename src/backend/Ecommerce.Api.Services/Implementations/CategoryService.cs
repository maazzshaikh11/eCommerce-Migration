using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.DTOs;

namespace Ecommerce.Api.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;

    public CategoryService(ICategoryRepository categories)
    {
        _categories = categories;
    }

    public List<CategoryDto> GetCategoryTree()
    {
        var all = _categories.GetAllWithProducts().ToList();
        var roots = all.Where(c => c.ParentCategoryId == null);
        return roots.Select(c => BuildNode(c, all)).ToList();
    }

    public CategoryDto? GetById(int id)
    {
        var c = _categories.GetById(id);
        if (c == null) return null;
        return new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ThumbnailUrl = c.ThumbnailUrl,
            ParentCategoryId = c.ParentCategoryId
        };
    }

    private static CategoryDto BuildNode(Core.Models.Category category, List<Core.Models.Category> all)
    {
        var children = all
            .Where(c => c.ParentCategoryId == category.Id)
            .Select(c => BuildNode(c, all))
            .ToList();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ThumbnailUrl = category.ThumbnailUrl,
            ParentCategoryId = category.ParentCategoryId,
            Children = children.Count > 0 ? children : null
        };
    }
}
