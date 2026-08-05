using System.Data;
using Microsoft.Data.SqlClient;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Data.Repositories;

public class CartItemRepository : ICartItemRepository
{
    private readonly DatabaseHelper _db;

    public CartItemRepository(DatabaseHelper db)
    {
        _db = db;
    }

    public CartItem? GetById(int id)
    {
        var sql = "SELECT * FROM CartItem WHERE Id = @Id";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@Id", id));
        if (dt.Rows.Count == 0) return null;
        return MapCartItem(dt.Rows[0]);
    }

    public IEnumerable<CartItem> GetCartItemsByCustomer(int customerId)
    {
        var sql = @"SELECT ci.*, p.Name as ProductName, p.ThumbnailUrl, p.ImageUrl, pv.VariantName, pv.VariantValue
                     FROM CartItem ci
                     LEFT JOIN Product p ON ci.ProductId = p.Id
                     LEFT JOIN ProductVariant pv ON ci.VariantId = pv.Id
                     WHERE ci.CustomerId = @CustomerId";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@CustomerId", customerId));
        foreach (DataRow row in dt.Rows)
            yield return MapCartItemWithProduct(row);
    }

    public void Add(CartItem entity)
    {
        var sql = @"INSERT INTO CartItem (SessionId, ProductId, VariantId, CustomerId, Quantity, UnitPrice, CreatedDate)
                     VALUES (@SessionId, @ProductId, @VariantId, @CustomerId, @Quantity, @UnitPrice, @CreatedDate);
                     SELECT SCOPE_IDENTITY();";
        entity.Id = Convert.ToInt32(_db.ExecuteScalar(sql,
            new SqlParameter("@SessionId", entity.SessionId ?? (object)DBNull.Value),
            new SqlParameter("@ProductId", entity.ProductId),
            new SqlParameter("@VariantId", entity.VariantId ?? (object)DBNull.Value),
            new SqlParameter("@CustomerId", entity.CustomerId ?? (object)DBNull.Value),
            new SqlParameter("@Quantity", entity.Quantity),
            new SqlParameter("@UnitPrice", entity.UnitPrice),
            new SqlParameter("@CreatedDate", entity.CreatedDate)));
    }

    public void Update(CartItem entity)
    {
        var sql = @"UPDATE CartItem SET SessionId = @SessionId, ProductId = @ProductId, VariantId = @VariantId,
                     CustomerId = @CustomerId, Quantity = @Quantity, UnitPrice = @UnitPrice WHERE Id = @Id";
        _db.ExecuteNonQuery(sql,
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@SessionId", entity.SessionId ?? (object)DBNull.Value),
            new SqlParameter("@ProductId", entity.ProductId),
            new SqlParameter("@VariantId", entity.VariantId ?? (object)DBNull.Value),
            new SqlParameter("@CustomerId", entity.CustomerId ?? (object)DBNull.Value),
            new SqlParameter("@Quantity", entity.Quantity),
            new SqlParameter("@UnitPrice", entity.UnitPrice));
    }

    public void Delete(CartItem entity)
    {
        var sql = "DELETE FROM CartItem WHERE Id = @Id";
        _db.ExecuteNonQuery(sql, new SqlParameter("@Id", entity.Id));
    }

    public void ClearCartByCustomerId(int customerId)
    {
        var sql = "DELETE FROM CartItem WHERE CustomerId = @CustomerId";
        _db.ExecuteNonQuery(sql, new SqlParameter("@CustomerId", customerId));
    }

    private static CartItem MapCartItem(DataRow row)
    {
        return new CartItem
        {
            Id = row.GetInt("Id"),
            SessionId = row.GetString("SessionId"),
            ProductId = row.GetInt("ProductId"),
            VariantId = row.GetIntNullable("VariantId"),
            CustomerId = row.GetIntNullable("CustomerId"),
            Quantity = row.GetInt("Quantity"),
            UnitPrice = row.GetDecimal("UnitPrice"),
            CreatedDate = row.GetDateTime("CreatedDate")
        };
    }

    private static CartItem MapCartItemWithProduct(DataRow row)
    {
        var item = MapCartItem(row);
        item.Product = new Product
        {
            Id = item.ProductId,
            Name = row.GetString("ProductName"),
            ThumbnailUrl = row.GetString("ThumbnailUrl"),
            ImageUrl = row.GetString("ImageUrl")
        };
        if (!row.IsNull("VariantName"))
        {
            item.Variant = new ProductVariant
            {
                Id = item.VariantId ?? 0,
                VariantName = row.GetString("VariantName"),
                VariantValue = row.GetString("VariantValue")
            };
        }
        return item;
    }
}
