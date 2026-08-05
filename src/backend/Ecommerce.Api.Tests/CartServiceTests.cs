using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;
using Ecommerce.Api.Services.Implementations;
using Moq;

namespace Ecommerce.Api.Tests;

public class CartServiceTests
{
    private static Mock<ICartItemRepository> Repo()
    {
        var r = new Mock<ICartItemRepository>();
        var item = new CartItem
        {
            Id = 1, ProductId = 10, CustomerId = 1, Quantity = 2, UnitPrice = 5,
            CreatedDate = DateTime.UtcNow,
            Product = new Product { Id = 10, Name = "Widget", Price = 5 }
        };
        r.Setup(x => x.GetCartItemsByCustomer(1)).Returns([item]);
        r.Setup(x => x.GetById(1)).Returns(item);
        return r;
    }

    private static Mock<IProductRepository> Products()
    {
        var p = new Mock<IProductRepository>();
        p.Setup(x => x.GetById(10)).Returns(new Product { Id = 10, Name = "Widget", Price = 5 });
        p.Setup(x => x.GetById(99)).Returns(new Product { Id = 99, Name = "Gadget", Price = 10 });
        return p;
    }

    [Fact]
    public void GetCartItems_ReturnsDtos()
    {
        var svc = new CartService(Repo().Object, Products().Object);
        var items = svc.GetCartItems(1);
        Assert.Single(items);
        Assert.Equal("Widget", items[0].ProductName);
        Assert.Equal(10m, items[0].LineTotal);
    }

    [Fact]
    public void GetCartSummary_ComputesTotals()
    {
        var svc = new CartService(Repo().Object, Products().Object);
        var s = svc.GetCartSummary(1);
        Assert.Equal(2, s.ItemCount);
        Assert.Equal(10m, s.Total);
    }

    [Fact]
    public void AddToCart_NewItem_Adds()
    {
        var r = Repo();
        var p = Products();
        var svc = new CartService(r.Object, p.Object);
        svc.AddToCart(1, 99, 1, null);
        r.Verify(x => x.Add(It.Is<CartItem>(ci => ci.ProductId == 99 && ci.Quantity == 1)), Times.Once);
    }

    [Fact]
    public void AddToCart_ExistingItem_UpdatesQuantity()
    {
        var r = Repo();
        var p = Products();
        var svc = new CartService(r.Object, p.Object);
        svc.AddToCart(1, 10, 3, null);
        r.Verify(x => x.Update(It.Is<CartItem>(ci => ci.Quantity == 5)), Times.Once);
    }

    [Fact]
    public void UpdateQuantity_RemovesWhenZero()
    {
        var r = Repo();
        var svc = new CartService(r.Object, Products().Object);
        svc.UpdateQuantity(1, 1, 0);
        r.Verify(x => x.Delete(It.Is<CartItem>(ci => ci.Id == 1)), Times.Once);
    }

    [Fact]
    public void RemoveFromCart_DeletesOwnedItem()
    {
        var r = Repo();
        var svc = new CartService(r.Object, Products().Object);
        svc.RemoveFromCart(1, 1);
        r.Verify(x => x.Delete(It.Is<CartItem>(ci => ci.Id == 1)), Times.Once);
    }

    [Fact]
    public void ClearCart_ClearsAll()
    {
        var r = Repo();
        var svc = new CartService(r.Object, Products().Object);
        svc.ClearCart(1);
        r.Verify(x => x.ClearCartByCustomerId(1), Times.Once);
    }
}
