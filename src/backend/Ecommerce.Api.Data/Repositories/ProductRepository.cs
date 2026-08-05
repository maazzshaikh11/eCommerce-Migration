using System.Data;
using Microsoft.Data.SqlClient;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DatabaseHelper _db;

    public ProductRepository(DatabaseHelper db)
    {
        _db = db;
    }

    public Product? GetById(int id)
    {
        var dt = _db.ExecuteQuery("SELECT * FROM Product WHERE Id = @Id", new SqlParameter("@Id", id));
        if (dt.Rows.Count == 0) return null;
        return MapProduct(dt.Rows[0]);
    }

    public IEnumerable<Product> GetAll()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM Product ORDER BY Name");
        foreach (DataRow row in dt.Rows)
            yield return MapProduct(row);
    }

    public IEnumerable<Product> GetProductsByCategory(int categoryId)
    {
        var dt = _db.ExecuteQuery(
            "SELECT * FROM Product WHERE CategoryId = @CategoryId AND IsActive = 1 ORDER BY Name",
            new SqlParameter("@CategoryId", categoryId));
        foreach (DataRow row in dt.Rows)
            yield return MapProduct(row);
    }

    public IEnumerable<Product> Search(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return [];
        var dt = _db.ExecuteQuery(
            "SELECT * FROM Product WHERE IsActive = 1 AND (Name LIKE @Term OR Description LIKE @Term) ORDER BY Name",
            new SqlParameter("@Term", "%" + searchTerm.Trim() + "%"));
        var list = new List<Product>();
        foreach (DataRow row in dt.Rows)
            list.Add(MapProduct(row));
        return list;
    }

    public IEnumerable<Product> GetActiveProducts()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM Product WHERE IsActive = 1 ORDER BY Name");
        foreach (DataRow row in dt.Rows)
            yield return MapProduct(row);
    }

    public Product? GetProductWithVariants(int id)
    {
        var product = GetById(id);
        if (product == null) return null;

        var imgDt = _db.ExecuteQuery("SELECT * FROM ProductImage WHERE ProductId = @ProductId ORDER BY DisplayOrder", new SqlParameter("@ProductId", id));
        foreach (DataRow row in imgDt.Rows)
            product.Images.Add(MapProductImage(row));

        var varDt = _db.ExecuteQuery("SELECT * FROM ProductVariant WHERE ProductId = @ProductId", new SqlParameter("@ProductId", id));
        foreach (DataRow row in varDt.Rows)
            product.Variants.Add(MapProductVariant(row));

        return product;
    }

    public ProductVariant? GetVariantById(int id)
    {
        var dt = _db.ExecuteQuery("SELECT * FROM ProductVariant WHERE Id = @Id", new SqlParameter("@Id", id));
        if (dt.Rows.Count == 0) return null;
        return MapProductVariant(dt.Rows[0]);
    }

    public void UpdateVariant(ProductVariant variant)
    {
        _db.ExecuteNonQuery(
            "UPDATE ProductVariant SET VariantName = @VariantName, VariantValue = @VariantValue, PriceAdjustment = @PriceAdjustment, StockQuantity = @StockQuantity WHERE Id = @Id",
            new SqlParameter("@Id", variant.Id),
            new SqlParameter("@VariantName", variant.VariantName),
            new SqlParameter("@VariantValue", variant.VariantValue),
            new SqlParameter("@PriceAdjustment", variant.PriceAdjustment),
            new SqlParameter("@StockQuantity", variant.StockQuantity));
    }

    public void Add(Product entity)
    {
        var sql = @"INSERT INTO Product (Name, Description, Price, CategoryId, ImageUrl, ThumbnailUrl, IsActive, CreatedDate)
                     VALUES (@Name, @Description, @Price, @CategoryId, @ImageUrl, @ThumbnailUrl, @IsActive, @CreatedDate);
                     SELECT SCOPE_IDENTITY();";
        entity.Id = Convert.ToInt32(_db.ExecuteScalar(sql,
            new SqlParameter("@Name", entity.Name),
            new SqlParameter("@Description", string.IsNullOrEmpty(entity.Description) ? DBNull.Value : entity.Description),
            new SqlParameter("@Price", entity.Price),
            new SqlParameter("@CategoryId", entity.CategoryId),
            new SqlParameter("@ImageUrl", string.IsNullOrEmpty(entity.ImageUrl) ? DBNull.Value : entity.ImageUrl),
            new SqlParameter("@ThumbnailUrl", string.IsNullOrEmpty(entity.ThumbnailUrl) ? DBNull.Value : entity.ThumbnailUrl),
            new SqlParameter("@IsActive", entity.IsActive),
            new SqlParameter("@CreatedDate", entity.CreatedDate)));
    }

    public void Update(Product entity)
    {
        _db.ExecuteNonQuery(
            "UPDATE Product SET Name = @Name, Description = @Description, Price = @Price, CategoryId = @CategoryId, ImageUrl = @ImageUrl, ThumbnailUrl = @ThumbnailUrl, IsActive = @IsActive WHERE Id = @Id",
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@Name", entity.Name),
            new SqlParameter("@Description", string.IsNullOrEmpty(entity.Description) ? DBNull.Value : entity.Description),
            new SqlParameter("@Price", entity.Price),
            new SqlParameter("@CategoryId", entity.CategoryId),
            new SqlParameter("@ImageUrl", string.IsNullOrEmpty(entity.ImageUrl) ? DBNull.Value : entity.ImageUrl),
            new SqlParameter("@ThumbnailUrl", string.IsNullOrEmpty(entity.ThumbnailUrl) ? DBNull.Value : entity.ThumbnailUrl),
            new SqlParameter("@IsActive", entity.IsActive));
    }

    public void Delete(Product entity)
    {
        _db.ExecuteNonQuery("DELETE FROM Product WHERE Id = @Id", new SqlParameter("@Id", entity.Id));
    }

    private static Product MapProduct(DataRow row) => new()
    {
        Id = row.GetInt("Id"),
        Name = row.GetString("Name"),
        Description = row.GetString("Description"),
        Price = row.GetDecimal("Price"),
        CategoryId = row.GetInt("CategoryId"),
        ImageUrl = row.GetString("ImageUrl"),
        ThumbnailUrl = row.GetString("ThumbnailUrl"),
        IsActive = row.GetBool("IsActive"),
        CreatedDate = row.GetDateTime("CreatedDate")
    };

    private static ProductImage MapProductImage(DataRow row) => new()
    {
        Id = row.GetInt("Id"),
        ProductId = row.GetInt("ProductId"),
        ImageUrl = row.GetString("ImageUrl"),
        IsPrimary = row.GetBool("IsPrimary"),
        DisplayOrder = row.GetInt("DisplayOrder")
    };

    private static ProductVariant MapProductVariant(DataRow row) => new()
    {
        Id = row.GetInt("Id"),
        ProductId = row.GetInt("ProductId"),
        VariantName = row.GetString("VariantName"),
        VariantValue = row.GetString("VariantValue"),
        PriceAdjustment = row.GetDecimal("PriceAdjustment"),
        StockQuantity = row.GetInt("StockQuantity")
    };
}
