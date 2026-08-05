using Ecommerce.Api.Core.DTOs;

namespace Ecommerce.Api.Core.Interfaces;

public interface IOrderService
{
    OrderConfirmationDto? GetOrderById(int orderId);
    List<OrderDto> GetOrderHistory(int customerId);
}
