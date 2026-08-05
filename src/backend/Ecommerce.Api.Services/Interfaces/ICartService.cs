using Ecommerce.Api.Core.DTOs;

namespace Ecommerce.Api.Core.Interfaces;

public interface ICartService
{
    List<CartItemDto> GetCartItems(int customerId);
    CartSummaryDto GetCartSummary(int customerId);
    void AddToCart(int customerId, int productId, int quantity, int? variantId = null);
    void UpdateQuantity(int customerId, int cartItemId, int quantity);
    void RemoveFromCart(int customerId, int cartItemId);
    void ClearCart(int customerId);
}
