using Ecommerce.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly DatabaseHelper _db;

    public HealthController(DatabaseHelper db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var result = _db.ExecuteScalar("SELECT 1");
            return Ok(new { status = "Healthy", db = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "Unhealthy", error = ex.Message });
        }
    }
}