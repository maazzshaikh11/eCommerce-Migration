using Ecommerce.Api.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private int GetCustomerId()
    {
        var claim = User.FindFirst("customer_id");
        if (claim == null || !int.TryParse(claim.Value, out var id))
            throw new InvalidOperationException("customer_id claim missing or invalid.");
        return id;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var orders = _orderService.GetOrderHistory(GetCustomerId());
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var order = _orderService.GetOrderById(id);
        if (order == null) return NotFound();
        return Ok(order);
    }
}
