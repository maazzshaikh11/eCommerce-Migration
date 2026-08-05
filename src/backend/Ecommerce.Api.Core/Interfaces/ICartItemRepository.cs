using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Core.Interfaces;

public interface ICartItemRepository
{
    CartItem? GetById(int id);
    IEnumerable<CartItem> GetCartItemsByCustomer(int customerId);
    void Add(CartItem entity);
    void Update(CartItem entity);
    void Delete(CartItem entity);
    void ClearCartByCustomerId(int customerId);
}
