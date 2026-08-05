using System.Data;
using Ecommerce.Api.Core.DTOs;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;
using Ecommerce.Api.Data;
using Microsoft.Data.SqlClient;

namespace Ecommerce.Api.Services.Implementations;

public class CheckoutService : ICheckoutService
{
    private readonly DatabaseHelper _db;
    private readonly IOrderRepository _orderRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IProductRepository _productRepository;
    private const decimal GST_RATE = 0.18m;

    public CheckoutService(
        DatabaseHelper db,
        IOrderRepository orderRepository,
        ICartItemRepository cartItemRepository,
        IAddressRepository addressRepository,
        IProductRepository productRepository)
    {
        _db = db;
        _orderRepository = orderRepository;
        _cartItemRepository = cartItemRepository;
        _addressRepository = addressRepository;
        _productRepository = productRepository;
    }

    public bool ValidateAddress(string streetAddress, string city, string stateProvince, string postalCode, string country)
    {
        return !string.IsNullOrWhiteSpace(streetAddress)
            && !string.IsNullOrWhiteSpace(city)
            && !string.IsNullOrWhiteSpace(stateProvince)
            && !string.IsNullOrWhiteSpace(postalCode)
            && !string.IsNullOrWhiteSpace(country);
    }

    public List<AddressDto> GetCustomerAddresses(int customerId)
    {
        return _addressRepository.GetByCustomerId(customerId)
            .Select(a => new AddressDto(a.Id, a.StreetAddress, a.City, a.StateProvince, a.PostalCode, a.Country, a.IsDefault))
            .ToList();
    }

    public OrderConfirmationDto? GetOrderConfirmation(int orderId)
    {
        var order = _orderRepository.GetOrderWithItems(orderId);
        if (order == null) return null;
        return BuildConfirmation(order);
    }

    public OrderConfirmationDto PlaceOrder(int customerId, int? addressId, string shippingMethod, string idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("Idempotency key is required.", nameof(idempotencyKey));

        var existing = _orderRepository.FindByIdempotencyKey(idempotencyKey);
        if (existing != null)
            return BuildConfirmation(existing);

        using var transaction = _db.BeginTransaction();
        _db.CurrentTransaction = transaction;
        try
        {
            var cartItems = _cartItemRepository.GetCartItemsByCustomer(customerId).ToList();
            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            // Deduct stock
            foreach (var item in cartItems)
            {
                if (item.VariantId.HasValue)
                {
                    var variant = _productRepository.GetVariantById(item.VariantId.Value);
                    if (variant != null)
                    {
                        variant.StockQuantity = Math.Max(0, variant.StockQuantity - item.Quantity);
                        _productRepository.UpdateVariant(variant);
                    }
                }
            }

            // Resolve address
            Address shippingAddr;
            if (addressId.HasValue)
            {
                shippingAddr = _addressRepository.GetById(addressId.Value)
                    ?? throw new InvalidOperationException("Address not found.");
            }
            else
            {
                throw new InvalidOperationException("Address is required.");
            }

            // Create billing address (same as shipping for simplicity)
            var billingAddr = new Address
            {
                CustomerId = customerId,
                AddressType = "Billing",
                StreetAddress = shippingAddr.StreetAddress,
                City = shippingAddr.City,
                StateProvince = shippingAddr.StateProvince,
                PostalCode = shippingAddr.PostalCode,
                Country = shippingAddr.Country,
                IsDefault = false
            };
            _addressRepository.Add(billingAddr);

            // Calculate totals
            var subTotal = cartItems.Sum(ci => ci.UnitPrice * ci.Quantity);
            var shippingTotal = shippingMethod?.ToLowerInvariant() switch
            {
                "express" => 199m,
                "standard" => 99m,
                _ => 99m // default
            };
            var taxTotal = (subTotal + shippingTotal) * GST_RATE;
            var grandTotal = subTotal + shippingTotal + taxTotal;

            // Create order
            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Paid",
                OrderNumber = GenerateOrderNumber(),
                SubTotal = subTotal,
                ShippingTotal = shippingTotal,
                TaxTotal = taxTotal,
                GrandTotal = grandTotal,
                ShippingAddressId = shippingAddr.Id,
                BillingAddressId = billingAddr.Id,
                IdempotencyKey = idempotencyKey,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? "Unknown Product",
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    LineTotal = ci.UnitPrice * ci.Quantity
                }).ToList()
            };

            _orderRepository.Add(order);

            // Clear cart
            _cartItemRepository.ClearCartByCustomerId(customerId);

            transaction.Commit();
            return BuildConfirmation(order);
        }
        catch (SqlException ex) when (IsDuplicateKeyException(ex))
        {
            transaction.Rollback();
            var duplicate = _orderRepository.FindByIdempotencyKey(idempotencyKey);
            if (duplicate != null) return BuildConfirmation(duplicate);
            throw;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            _db.CurrentTransaction = null;
        }
    }

    private static bool IsDuplicateKeyException(SqlException ex)
    {
        if (ex.Number == 2601 || ex.Number == 2627)
        {
            var msg = ex.Message;
            if (msg.Contains("IX_Order_IdempotencyKey") || msg.Contains("IdempotencyKey"))
                return true;
        }
        return false;
    }

    private static OrderConfirmationDto BuildConfirmation(Order order)
    {
        return new OrderConfirmationDto(
            order.Id,
            order.OrderNumber,
            order.OrderDate,
            order.OrderStatus,
            order.SubTotal,
            order.ShippingTotal,
            order.TaxTotal,
            order.GrandTotal,
            FormatAddress(order.ShippingAddressId, order.BillingAddressId),
            order.OrderItems.Select(oi => new OrderItemDto(oi.ProductName, oi.Quantity, oi.UnitPrice, oi.LineTotal)).ToList()
        );
    }

    private static string? FormatAddress(int? shippingId, int? billingId)
    {
        // ponytail: address formatting deferred; SPA fetches address separately if needed
        return null;
    }

    private static string GenerateOrderNumber()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Random.Shared.Next(1000, 9999).ToString();
        return $"RF-{datePart}-{randomPart}";
    }

    public List<ShippingOptionDto> GetShippingOptions()
    {
        return
        [
            new ShippingOptionDto("Standard", "Standard Shipping (5-7 business days)", 99m),
            new ShippingOptionDto("Express", "Express Shipping (2-3 business days)", 199m)
        ];
    }

    public bool ValidatePayment(PaymentRequest request)
    {
        if (request == null) return false;
        if (string.IsNullOrWhiteSpace(request.CardholderName)) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.CardNumber ?? "", @"^\d{16}$")) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.ExpirationDate ?? "", @"^(0[1-9]|1[0-2])\/\d{2}$")) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.Cvv ?? "", @"^\d{3,4}$")) return false;
        return true;
    }
}
