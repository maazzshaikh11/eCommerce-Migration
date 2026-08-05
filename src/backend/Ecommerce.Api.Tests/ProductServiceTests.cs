using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;
using Ecommerce.Api.Services.Implementations;
using Moq;

namespace Ecommerce.Api.Tests;

public class ProductServiceTests
{
    private static Mock<IProductRepository> Products()
    {
        var p = new Mock<IProductRepository>();
        p.Setup(x => x.GetActiveProducts()).Returns([
            new Product { Id = 1, Name = "A", Price = 10, ThumbnailUrl = "", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Product { Id = 2, Name = "B", Price = 20, ThumbnailUrl = "", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Product { Id = 3, Name = "C", Price = 30, ThumbnailUrl = "", IsActive = true, CreatedDate = DateTime.UtcNow },
        ]);
        return p;
    }

    private static Mock<ICategoryRepository> Categories() => new();

    [Fact]
    public void GetProducts_NoFilter_ReturnsPaged()
    {
        var svc = new ProductService(Products().Object, Categories().Object);
        var result = svc.GetProducts(null, null, null, null, null, 1, 2);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public void GetProducts_SearchKeyword_Filters()
    {
        var svc = new ProductService(Products().Object, Categories().Object);
        var result = svc.GetProducts(null, "A", null, null, null, 1, 10);
        Assert.Single(result.Items);
        Assert.Equal("A", result.Items[0].Name);
    }

    [Fact]
    public void GetProducts_SortByPriceAsc_Orders()
    {
        var svc = new ProductService(Products().Object, Categories().Object);
        var result = svc.GetProducts(null, null, null, null, "price_asc", 1, 10);
        Assert.Equal(10m, result.Items[0].Price);
    }

    [Fact]
    public void GetProductDetail_Found_ReturnsDto()
    {
        var p = Products();
        p.Setup(x => x.GetProductWithVariants(1)).Returns(new Product
        {
            Id = 1, Name = "A", Price = 10, CategoryId = 1,
            Images = [], Variants = [], CreatedDate = DateTime.UtcNow
        });
        var c = Categories();
        c.Setup(x => x.GetById(1)).Returns(new Category { Id = 1, Name = "Cat" });
        var svc = new ProductService(p.Object, c.Object);
        var dto = svc.GetProductDetail(1);
        Assert.NotNull(dto);
        Assert.Equal("A", dto.Name);
        Assert.Equal("Cat", dto.CategoryName);
    }

    [Fact]
    public void GetProductDetail_NotFound_ReturnsNull()
    {
        var p = Products();
        p.Setup(x => x.GetProductWithVariants(99)).Returns((Product?)null);
        var svc = new ProductService(p.Object, Categories().Object);
        Assert.Null(svc.GetProductDetail(99));
    }
}
