using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.DTOs;

namespace Ecommerce.Api.Core.Interfaces;

public interface ICategoryService
{
    List<CategoryDto> GetCategoryTree();
    CategoryDto? GetById(int id);
}
