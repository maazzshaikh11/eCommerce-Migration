using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(ICheckoutService checkoutService)
    {
        _checkoutService = checkoutService;
    }

    private int GetCustomerId()
    {
        var claim = User.FindFirst("customer_id");
        if (claim == null || !int.TryParse(claim.Value, out var id))
            throw new InvalidOperationException("customer_id claim missing or invalid.");
        return id;
    }

    [HttpGet("addresses")]
    public IActionResult GetAddresses()
    {
        var addresses = _checkoutService.GetCustomerAddresses(GetCustomerId());
        return Ok(addresses);
    }

    [HttpPost("addresses")]
    public IActionResult AddAddress([FromBody] AddressDto dto)
    {
        var customerId = GetCustomerId();
        var address = new Core.Models.Address
        {
            CustomerId = customerId,
            AddressType = "Shipping",
            StreetAddress = dto.StreetAddress,
            City = dto.City,
            StateProvince = dto.StateProvince,
            PostalCode = dto.PostalCode,
            Country = dto.Country,
            IsDefault = dto.IsDefault
        };
        // ponytail: using direct repository access; in production inject IAddressRepository
        var addressRepo = HttpContext.RequestServices.GetRequiredService<IAddressRepository>();
        addressRepo.Add(address);
        return Ok(new { id = address.Id });
    }

    [HttpGet("review")]
    public IActionResult GetReview()
    {
        // ponytail: review data built by SPA from cart + selected address; return empty ok
        return Ok(new { });
    }

    [HttpGet("shipping-options")]
    public IActionResult GetShippingOptions()
    {
        return Ok(_checkoutService.GetShippingOptions());
    }

    [HttpPost("validate-payment")]
    public IActionResult ValidatePayment([FromBody] PaymentRequest request)
    {
        var valid = _checkoutService.ValidatePayment(request);
        return Ok(new { valid });
    }

    [HttpPost("orders")]
    public IActionResult PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return BadRequest(new { message = "Idempotency-Key header is required." });

        try
        {
            var confirmation = _checkoutService.PlaceOrder(GetCustomerId(), request.AddressId, request.ShippingMethod ?? "Standard", idempotencyKey);
            return StatusCode(201, confirmation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("orders/{id}/confirmation")]
    public IActionResult GetConfirmation(int id)
    {
        var confirmation = _checkoutService.GetOrderConfirmation(id);
        if (confirmation == null) return NotFound();
        return Ok(confirmation);
    }
}
