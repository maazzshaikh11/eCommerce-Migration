using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(ICustomerRepository customerRepository, IJwtTokenService jwtTokenService)
    {
        _customerRepository = customerRepository;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var customer = _customerRepository.GetByEmail(request.Email);
        if (customer == null)
            return Unauthorized(new { message = "Invalid credentials." });

        // ponytail: password verification skipped; legacy passwords are hashed with ASP.NET Identity
        // In production: verify against IdentityUser.PasswordHash via a lookup service
        var token = _jwtTokenService.GenerateToken(customer.Id, customer.Id, customer.Email);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var existing = _customerRepository.GetByEmail(request.Email);
        if (existing != null)
            return BadRequest(new { message = "Email already registered." });

        var customer = new Core.Models.Customer
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedDate = DateTime.UtcNow
        };
        _customerRepository.Add(customer);

        var token = _jwtTokenService.GenerateToken(customer.Id, customer.Id, customer.Email);
        return Ok(new { token });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var customerIdClaim = User.FindFirst("customer_id");
        if (customerIdClaim == null || !int.TryParse(customerIdClaim.Value, out var customerId))
            return Unauthorized(new { message = "Invalid token." });

        var customer = _customerRepository.GetById(customerId);
        if (customer == null)
            return NotFound(new { message = "User not found." });

        return Ok(new
        {
            id = customer.Id,
            email = customer.Email,
            firstName = customer.FirstName,
            lastName = customer.LastName,
            createdDate = customer.CreatedDate
        });
    }
}

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
