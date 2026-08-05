using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Core.Interfaces;

public interface IProductRepository
{
    Product? GetById(int id);
    IEnumerable<Product> GetAll();
    IEnumerable<Product> GetProductsByCategory(int categoryId);
    IEnumerable<Product> Search(string searchTerm);
    IEnumerable<Product> GetActiveProducts();
    Product? GetProductWithVariants(int id);
    ProductVariant? GetVariantById(int id);
    void UpdateVariant(ProductVariant variant);
    void Add(Product entity);
    void Update(Product entity);
    void Delete(Product entity);
}
