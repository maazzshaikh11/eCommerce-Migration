using System.Security.Claims;
using Ecommerce.Api.Controllers;
using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Ecommerce.Api.Tests;

public class CartControllerTests
{
    private static CartController Controller(int customerId)
    {
        var s = new Mock<ICartService>();
        s.Setup(x => x.GetCartItems(customerId)).Returns([
            new CartItemDto(1, 10, "Widget", null, null, 5m, 2, 10m)
        ]);
        s.Setup(x => x.GetCartSummary(customerId)).Returns(new CartSummaryDto(2, 10m, 10m));

        var ctrl = new CartController(s.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("customer_id", customerId.ToString())], "Test"));
        ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return ctrl;
    }

    [Fact]
    public void Get_ReturnsCartItems()
    {
        var ctrl = Controller(1);
        var result = ctrl.Get() as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void GetSummary_ReturnsSummary()
    {
        var ctrl = Controller(1);
        var result = ctrl.GetSummary() as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void AddItem_ReturnsOk()
    {
        var ctrl = Controller(1);
        var result = ctrl.AddItem(new AddCartItemRequest(10, null, 1)) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void AddItem_InvalidQuantity_Returns400()
    {
        var ctrl = Controller(1);
        var result = ctrl.AddItem(new AddCartItemRequest(10, null, 0));
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void UpdateItem_ReturnsOk()
    {
        var ctrl = Controller(1);
        var result = ctrl.UpdateItem(1, 3) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void RemoveItem_ReturnsOk()
    {
        var ctrl = Controller(1);
        var result = ctrl.RemoveItem(1) as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void Clear_ReturnsOk()
    {
        var ctrl = Controller(1);
        var result = ctrl.Clear() as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }
}
