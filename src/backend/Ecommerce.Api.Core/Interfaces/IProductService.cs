using Ecommerce.Api.Core.DTOs;

namespace Ecommerce.Api.Core.Interfaces;

public interface IProductService
{
    PagedResult<ProductListResponseDto> GetProducts(int? categoryId, string? keyword, decimal? minPrice, decimal? maxPrice, string? sortBy, int page, int pageSize);
    ProductDetailDto? GetProductDetail(int id);
    List<ProductListResponseDto> GetFeaturedProducts(int count = 6);
}
