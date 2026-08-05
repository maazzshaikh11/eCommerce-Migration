using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;

namespace Ecommerce.Api.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public OrderConfirmationDto? GetOrderById(int orderId)
    {
        var order = _orderRepository.GetOrderWithItems(orderId);
        if (order == null) return null;
        return new OrderConfirmationDto(
            order.Id,
            order.OrderNumber,
            order.OrderDate,
            order.OrderStatus,
            order.SubTotal,
            order.ShippingTotal,
            order.TaxTotal,
            order.GrandTotal,
            null,
            order.OrderItems.Select(oi => new OrderItemDto(oi.ProductName, oi.Quantity, oi.UnitPrice, oi.LineTotal)).ToList()
        );
    }

    public List<OrderDto> GetOrderHistory(int customerId)
    {
        return _orderRepository.GetOrdersByCustomer(customerId)
            .Select(o => new OrderDto(o.Id, o.OrderNumber, o.OrderDate, o.OrderStatus, o.SubTotal, o.ShippingTotal, o.TaxTotal, o.GrandTotal))
            .ToList();
    }
}
