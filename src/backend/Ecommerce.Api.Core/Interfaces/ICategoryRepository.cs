using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Core.Interfaces;

public interface ICategoryRepository
{
    Category? GetById(int id);
    IEnumerable<Category> GetAll();
    IEnumerable<Category> GetAllWithProducts();
    void Add(Category entity);
    void Update(Category entity);
    void Delete(Category entity);
}
