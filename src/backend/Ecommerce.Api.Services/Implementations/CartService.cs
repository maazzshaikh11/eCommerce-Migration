using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartItemRepository cartItemRepository, IProductRepository productRepository)
    {
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
    }

    public List<CartItemDto> GetCartItems(int customerId)
    {
        var items = _cartItemRepository.GetCartItemsByCustomer(customerId).ToList();
        return items.Select(ci => new CartItemDto(
            ci.Id,
            ci.ProductId,
            ci.Product?.Name ?? "Unknown Product",
            ci.Variant?.VariantName,
            ci.Product?.ThumbnailUrl ?? ci.Product?.ImageUrl,
            ci.UnitPrice,
            ci.Quantity,
            ci.UnitPrice * ci.Quantity
        )).ToList();
    }

    public CartSummaryDto GetCartSummary(int customerId)
    {
        var items = _cartItemRepository.GetCartItemsByCustomer(customerId).ToList();
        var itemCount = items.Sum(i => i.Quantity);
        var subTotal = items.Sum(i => i.UnitPrice * i.Quantity);
        return new CartSummaryDto(itemCount, subTotal, subTotal);
    }

    public void AddToCart(int customerId, int productId, int quantity, int? variantId = null)
    {
        if (quantity < 1) quantity = 1;

        var existingItems = _cartItemRepository.GetCartItemsByCustomer(customerId).ToList();
        var existingItem = existingItems.FirstOrDefault(ci => ci.ProductId == productId && ci.VariantId == variantId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            _cartItemRepository.Update(existingItem);
            return;
        }

        var product = _productRepository.GetById(productId);
        if (product == null)
            throw new ArgumentException("Product not found.", nameof(productId));

        var cartItem = new CartItem
        {
            CustomerId = customerId,
            ProductId = productId,
            VariantId = variantId,
            Quantity = quantity,
            UnitPrice = product.Price,
            CreatedDate = DateTime.UtcNow
        };
        _cartItemRepository.Add(cartItem);
    }

    public void UpdateQuantity(int customerId, int cartItemId, int quantity)
    {
        var item = _cartItemRepository.GetById(cartItemId);
        if (item == null) return;
        if (item.CustomerId != customerId) return;

        if (quantity < 1)
        {
            _cartItemRepository.Delete(item);
        }
        else
        {
            item.Quantity = quantity;
            _cartItemRepository.Update(item);
        }
    }

    public void RemoveFromCart(int customerId, int cartItemId)
    {
        var item = _cartItemRepository.GetById(cartItemId);
        if (item == null) return;
        if (item.CustomerId != customerId) return;

        _cartItemRepository.Delete(item);
    }

    public void ClearCart(int customerId)
    {
        _cartItemRepository.ClearCartByCustomerId(customerId);
    }
}
