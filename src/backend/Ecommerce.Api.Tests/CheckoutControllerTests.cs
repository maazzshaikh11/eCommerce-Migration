using System.Security.Claims;
using Ecommerce.Api.Controllers;
using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Ecommerce.Api.Tests;

public class CheckoutControllerTests
{
    private static CheckoutController Controller(int customerId)
    {
        var s = new Mock<ICheckoutService>();
        s.Setup(x => x.GetCustomerAddresses(customerId)).Returns([
            new AddressDto(1, "123 Main", "City", "ST", "12345", "US", false)
        ]);
        s.Setup(x => x.PlaceOrder(customerId, 1, It.IsAny<string>(), It.IsAny<string>())).Returns(
            new OrderConfirmationDto(1, "RF-20260101-1234", DateTime.UtcNow, "Paid", 10, 99, 19.62m, 128.62m, null, [])
        );

        var ctrl = new CheckoutController(s.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim("customer_id", customerId.ToString())], "Test"));
        ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return ctrl;
    }

    [Fact]
    public void GetAddresses_ReturnsOk()
    {
        var ctrl = Controller(1);
        var result = ctrl.GetAddresses() as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void PlaceOrder_MissingHeader_Returns400()
    {
        var ctrl = Controller(1);
        ctrl.ControllerContext.HttpContext.Request.Headers.Remove("Idempotency-Key");
        var result = ctrl.PlaceOrder(new PlaceOrderRequest(1, ""));
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void PlaceOrder_Valid_Returns201()
    {
        var ctrl = Controller(1);
        ctrl.ControllerContext.HttpContext.Request.Headers["Idempotency-Key"] = "key-1";
        var result = ctrl.PlaceOrder(new PlaceOrderRequest(1, "key-1"));
        Assert.IsType<ObjectResult>(result);
        var obj = (ObjectResult)result;
        Assert.Equal(201, obj.StatusCode);
    }
}
