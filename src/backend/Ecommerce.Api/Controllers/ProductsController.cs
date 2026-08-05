using Ecommerce.Api.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products)
    {
        _products = products;
    }

    [HttpGet]
    public IActionResult Get(
        [FromQuery] int? categoryId,
        [FromQuery] string? searchQuery,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortBy,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var result = _products.GetProducts(categoryId, searchQuery, minPrice, maxPrice, sortBy, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var product = _products.GetProductDetail(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("featured")]
    public IActionResult Featured([FromQuery] int count = 6)
    {
        return Ok(_products.GetFeaturedProducts(count));
    }
}
