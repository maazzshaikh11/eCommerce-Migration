using Ecommerce.Api.Controllers;
using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Ecommerce.Api.Tests;

public class ProductsControllerTests
{
    private static Mock<IProductService> Service()
    {
        var s = new Mock<IProductService>();
        s.Setup(x => x.GetProducts(null, null, null, null, null, 1, 12))
         .Returns(new PagedResult<ProductListResponseDto>
         {
             Items = [new ProductListResponseDto { Id = 1, Name = "A", Price = 10 }],
             CurrentPage = 1, PageSize = 12, TotalCount = 1, TotalPages = 1
         });
        s.Setup(x => x.GetProductDetail(1))
         .Returns(new ProductDetailDto { Id = 1, Name = "A", Price = 10, Description = "", ImageUrl = "", ThumbnailUrl = "", IsActive = true, CreatedDate = DateTime.UtcNow, Images = [], Variants = [] });
        s.Setup(x => x.GetProductDetail(99)).Returns((ProductDetailDto?)null);
        return s;
    }

    [Fact]
    public void Get_ReturnsOkWithPagedResult()
    {
        var ctrl = new ProductsController(Service().Object);
        var result = ctrl.Get(null, null, null, null, null, 1, 12) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void GetById_Found_ReturnsOk()
    {
        var ctrl = new ProductsController(Service().Object);
        var result = ctrl.GetById(1) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void GetById_NotFound_Returns404()
    {
        var ctrl = new ProductsController(Service().Object);
        var result = ctrl.GetById(99);
        Assert.IsType<NotFoundResult>(result);
    }
}
