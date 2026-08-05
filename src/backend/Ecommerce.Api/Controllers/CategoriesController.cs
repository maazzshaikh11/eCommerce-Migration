using Ecommerce.Api.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories)
    {
        _categories = categories;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_categories.GetCategoryTree());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var category = _categories.GetById(id);
        if (category == null) return NotFound();
        return Ok(category);
    }
}
