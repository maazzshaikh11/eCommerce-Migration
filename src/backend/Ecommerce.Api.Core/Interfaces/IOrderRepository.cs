using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Core.Interfaces;

public interface IOrderRepository
{
    Order? GetById(int id);
    IEnumerable<Order> GetOrdersByCustomer(int customerId);
    Order? GetOrderWithItems(int id);
    Order? FindByIdempotencyKey(string idempotencyKey);
    void Add(Order entity);
    void Update(Order entity);
    void Delete(Order entity);
}
