using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.DTOs;

namespace Ecommerce.Api.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;

    public ProductService(IProductRepository products, ICategoryRepository categories)
    {
        _products = products;
        _categories = categories;
    }

    public PagedResult<ProductListResponseDto> GetProducts(int? categoryId, string? keyword, decimal? minPrice, decimal? maxPrice, string? sortBy, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 12;

        IEnumerable<Core.Models.Product> query = categoryId.HasValue
            ? GetProductsByCategoryTree(categoryId.Value)
            : _products.GetActiveProducts();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLowerInvariant();
            query = query.Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (minPrice.HasValue) query = query.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(p => p.Price <= maxPrice.Value);

        query = sortBy?.ToLowerInvariant() switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "name" => query.OrderBy(p => p.Name),
            _ => query.OrderByDescending(p => p.Id)
        };

        var filtered = query.ToList();
        var total = filtered.Count;
        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductListResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ThumbnailUrl = p.ThumbnailUrl,
                CategoryName = categoryId.HasValue ? _categories.GetById(categoryId.Value)?.Name : null
            })
            .ToList();

        return new PagedResult<ProductListResponseDto>
        {
            Items = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    private IEnumerable<Core.Models.Product> GetProductsByCategoryTree(int rootCategoryId)
    {
        var allCategories = _categories.GetAll().ToList();
        var targetIds = new HashSet<int> { rootCategoryId };
        var queue = new Queue<int>(targetIds);
        while (queue.Count > 0)
        {
            var parentId = queue.Dequeue();
            foreach (var child in allCategories.Where(c => c.ParentCategoryId == parentId))
            {
                if (targetIds.Add(child.Id)) queue.Enqueue(child.Id);
            }
        }
        // ponytail: in-memory tree walk; if catalog grows large, move to recursive SQL CTE
        return _products.GetAll().Where(p => p.IsActive && targetIds.Contains(p.CategoryId));
    }

    public ProductDetailDto? GetProductDetail(int id)
    {
        var p = _products.GetProductWithVariants(id);
        if (p == null) return null;

        return new ProductDetailDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            ThumbnailUrl = p.ThumbnailUrl,
            IsActive = p.IsActive,
            CreatedDate = p.CreatedDate,
            CategoryName = _categories.GetById(p.CategoryId)?.Name,
            Images = p.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                IsPrimary = i.IsPrimary,
                DisplayOrder = i.DisplayOrder
            }).ToList(),
            Variants = p.Variants.Select(v => new ProductVariantDto
            {
                Id = v.Id,
                VariantName = v.VariantName,
                VariantValue = v.VariantValue,
                PriceAdjustment = v.PriceAdjustment,
                StockQuantity = v.StockQuantity
            }).ToList()
        };
    }

    public List<ProductListResponseDto> GetFeaturedProducts(int count = 6)
    {
        return _products.GetActiveProducts()
            .Take(count)
            .Select(p => new ProductListResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ThumbnailUrl = p.ThumbnailUrl
            })
            .ToList();
    }
}
