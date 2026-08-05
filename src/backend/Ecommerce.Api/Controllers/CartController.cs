using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
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
        var items = _cartService.GetCartItems(GetCustomerId());
        return Ok(items);
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        var summary = _cartService.GetCartSummary(GetCustomerId());
        return Ok(summary);
    }

    [HttpPost("items")]
    public IActionResult AddItem([FromBody] AddCartItemRequest request)
    {
        if (request.Quantity < 1 || request.Quantity > 999)
            return BadRequest(new { message = "Quantity must be between 1 and 999." });

        _cartService.AddToCart(GetCustomerId(), request.ProductId, request.Quantity, request.VariantId);
        var summary = _cartService.GetCartSummary(GetCustomerId());
        return Ok(new { success = true, itemCount = summary.ItemCount, cartTotal = summary.Total });
    }

    [HttpPut("items/{id}")]
    public IActionResult UpdateItem(int id, [FromBody] int quantity)
    {
        if (quantity < 0 || quantity > 999)
            return BadRequest(new { message = "Quantity must be between 0 and 999." });

        _cartService.UpdateQuantity(GetCustomerId(), id, quantity);
        var summary = _cartService.GetCartSummary(GetCustomerId());
        return Ok(new { success = true, itemCount = summary.ItemCount, cartTotal = summary.Total });
    }

    [HttpDelete("items/{id}")]
    public IActionResult RemoveItem(int id)
    {
        _cartService.RemoveFromCart(GetCustomerId(), id);
        var summary = _cartService.GetCartSummary(GetCustomerId());
        return Ok(new { success = true, itemCount = summary.ItemCount, cartTotal = summary.Total });
    }

    [HttpDelete]
    public IActionResult Clear()
    {
        _cartService.ClearCart(GetCustomerId());
        return Ok(new { success = true });
    }
}
