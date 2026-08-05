using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthTestController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "Authenticated" });
}