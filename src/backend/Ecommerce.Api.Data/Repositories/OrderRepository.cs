using System.Data;
using Microsoft.Data.SqlClient;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DatabaseHelper _db;

    public OrderRepository(DatabaseHelper db)
    {
        _db = db;
    }

    public Order? GetById(int id)
    {
        var sql = "SELECT * FROM [Order] WHERE Id = @Id";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@Id", id));
        if (dt.Rows.Count == 0) return null;
        return MapOrder(dt.Rows[0]);
    }

    public IEnumerable<Order> GetOrdersByCustomer(int customerId)
    {
        var sql = "SELECT * FROM [Order] WHERE CustomerId = @CustomerId ORDER BY OrderDate DESC";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@CustomerId", customerId));
        var orders = new List<Order>();
        var orderDict = new Dictionary<int, Order>();
        foreach (DataRow row in dt.Rows)
        {
            var order = MapOrder(row);
            order.OrderItems = [];
            orders.Add(order);
            orderDict[order.Id] = order;
        }

        if (orders.Count > 0)
        {
            var itemSql = @"SELECT oi.* FROM OrderItem oi
                            INNER JOIN [Order] o ON oi.OrderId = o.Id
                            WHERE o.CustomerId = @CustomerId";
            var itemDt = _db.ExecuteQuery(itemSql, new SqlParameter("@CustomerId", customerId));
            foreach (DataRow row in itemDt.Rows)
            {
                var orderId = row.GetInt("OrderId");
                if (orderDict.ContainsKey(orderId))
                    orderDict[orderId].OrderItems.Add(MapOrderItem(row));
            }
        }

        return orders;
    }

    public Order? GetOrderWithItems(int id)
    {
        var order = GetById(id);
        if (order == null) return null;
        order.OrderItems = GetOrderItems(id);
        return order;
    }

    public Order? FindByIdempotencyKey(string idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey)) return null;
        var sql = "SELECT * FROM [Order] WHERE IdempotencyKey = @IdempotencyKey";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@IdempotencyKey", idempotencyKey));
        if (dt.Rows.Count == 0) return null;
        var order = MapOrder(dt.Rows[0]);
        order.OrderItems = GetOrderItems(order.Id);
        return order;
    }

    public void Add(Order entity)
    {
        var sql = @"INSERT INTO [Order] (CustomerId, OrderDate, OrderStatus, OrderNumber, SubTotal, ShippingTotal, TaxTotal, GrandTotal, ShippingAddressId, BillingAddressId, IdempotencyKey)
                     VALUES (@CustomerId, @OrderDate, @OrderStatus, @OrderNumber, @SubTotal, @ShippingTotal, @TaxTotal, @GrandTotal, @ShippingAddressId, @BillingAddressId, @IdempotencyKey);
                     SELECT SCOPE_IDENTITY();";
        entity.Id = Convert.ToInt32(_db.ExecuteScalar(sql,
            new SqlParameter("@CustomerId", entity.CustomerId),
            new SqlParameter("@OrderDate", entity.OrderDate),
            new SqlParameter("@OrderStatus", entity.OrderStatus),
            new SqlParameter("@OrderNumber", entity.OrderNumber),
            new SqlParameter("@SubTotal", entity.SubTotal),
            new SqlParameter("@ShippingTotal", entity.ShippingTotal),
            new SqlParameter("@TaxTotal", entity.TaxTotal),
            new SqlParameter("@GrandTotal", entity.GrandTotal),
            new SqlParameter("@ShippingAddressId", entity.ShippingAddressId ?? (object)DBNull.Value),
            new SqlParameter("@BillingAddressId", entity.BillingAddressId ?? (object)DBNull.Value),
            new SqlParameter("@IdempotencyKey", entity.IdempotencyKey ?? (object)DBNull.Value)));

        foreach (var item in entity.OrderItems)
        {
            var itemSql = @"INSERT INTO OrderItem (OrderId, ProductId, ProductName, Quantity, UnitPrice, LineTotal)
                            VALUES (@OrderId, @ProductId, @ProductName, @Quantity, @UnitPrice, @LineTotal)";
            _db.ExecuteNonQuery(itemSql,
                new SqlParameter("@OrderId", entity.Id),
                new SqlParameter("@ProductId", item.ProductId),
                new SqlParameter("@ProductName", item.ProductName),
                new SqlParameter("@Quantity", item.Quantity),
                new SqlParameter("@UnitPrice", item.UnitPrice),
                new SqlParameter("@LineTotal", item.LineTotal));
        }
    }

    public void Update(Order entity)
    {
        var sql = @"UPDATE [Order] SET CustomerId = @CustomerId, OrderDate = @OrderDate, OrderStatus = @OrderStatus,
                     OrderNumber = @OrderNumber, SubTotal = @SubTotal, ShippingTotal = @ShippingTotal,
                     TaxTotal = @TaxTotal, GrandTotal = @GrandTotal, ShippingAddressId = @ShippingAddressId,
                     BillingAddressId = @BillingAddressId, IdempotencyKey = @IdempotencyKey WHERE Id = @Id";
        _db.ExecuteNonQuery(sql,
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@CustomerId", entity.CustomerId),
            new SqlParameter("@OrderDate", entity.OrderDate),
            new SqlParameter("@OrderStatus", entity.OrderStatus),
            new SqlParameter("@OrderNumber", entity.OrderNumber),
            new SqlParameter("@SubTotal", entity.SubTotal),
            new SqlParameter("@ShippingTotal", entity.ShippingTotal),
            new SqlParameter("@TaxTotal", entity.TaxTotal),
            new SqlParameter("@GrandTotal", entity.GrandTotal),
            new SqlParameter("@ShippingAddressId", entity.ShippingAddressId ?? (object)DBNull.Value),
            new SqlParameter("@BillingAddressId", entity.BillingAddressId ?? (object)DBNull.Value),
            new SqlParameter("@IdempotencyKey", entity.IdempotencyKey ?? (object)DBNull.Value));
    }

    public void Delete(Order entity)
    {
        var sql = "DELETE FROM [Order] WHERE Id = @Id";
        _db.ExecuteNonQuery(sql, new SqlParameter("@Id", entity.Id));
    }

    private List<OrderItem> GetOrderItems(int orderId)
    {
        var items = new List<OrderItem>();
        var sql = "SELECT * FROM OrderItem WHERE OrderId = @OrderId";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@OrderId", orderId));
        foreach (DataRow row in dt.Rows)
            items.Add(MapOrderItem(row));
        return items;
    }

    private static Order MapOrder(DataRow row)
    {
        return new Order
        {
            Id = row.GetInt("Id"),
            CustomerId = row.GetInt("CustomerId"),
            OrderDate = row.GetDateTime("OrderDate"),
            OrderStatus = row.GetString("OrderStatus"),
            OrderNumber = row.GetString("OrderNumber"),
            SubTotal = row.GetDecimal("SubTotal"),
            ShippingTotal = row.GetDecimal("ShippingTotal"),
            TaxTotal = row.GetDecimal("TaxTotal"),
            GrandTotal = row.GetDecimal("GrandTotal"),
            ShippingAddressId = row.GetIntNullable("ShippingAddressId"),
            BillingAddressId = row.GetIntNullable("BillingAddressId"),
            IdempotencyKey = row.GetString("IdempotencyKey")
        };
    }

    private static OrderItem MapOrderItem(DataRow row)
    {
        return new OrderItem
        {
            Id = row.GetInt("Id"),
            OrderId = row.GetInt("OrderId"),
            ProductId = row.GetInt("ProductId"),
            ProductName = row.GetString("ProductName"),
            Quantity = row.GetInt("Quantity"),
            UnitPrice = row.GetDecimal("UnitPrice"),
            LineTotal = row.GetDecimal("LineTotal")
        };
    }
}
