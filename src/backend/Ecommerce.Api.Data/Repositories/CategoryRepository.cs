using System.Data;
using Microsoft.Data.SqlClient;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DatabaseHelper _db;

    public CategoryRepository(DatabaseHelper db)
    {
        _db = db;
    }

    public Category? GetById(int id)
    {
        var dt = _db.ExecuteQuery("SELECT * FROM Category WHERE Id = @Id", new SqlParameter("@Id", id));
        if (dt.Rows.Count == 0) return null;
        return MapCategory(dt.Rows[0]);
    }

    public IEnumerable<Category> GetAll()
    {
        var dt = _db.ExecuteQuery("SELECT * FROM Category ORDER BY Name");
        foreach (DataRow row in dt.Rows)
            yield return MapCategory(row);
    }

    public IEnumerable<Category> GetAllWithProducts()
    {
        var categories = new List<Category>();
        var catDt = _db.ExecuteQuery("SELECT * FROM Category");
        var dict = new Dictionary<int, Category>();
        foreach (DataRow row in catDt.Rows)
        {
            var cat = MapCategory(row);
            categories.Add(cat);
            dict[cat.Id] = cat;
        }

        var prodDt = _db.ExecuteQuery("SELECT * FROM Product WHERE IsActive = 1");
        foreach (DataRow row in prodDt.Rows)
        {
            var prod = MapProduct(row);
            if (dict.TryGetValue(prod.CategoryId, out var cat))
                cat.Products.Add(prod);
        }

        return categories;
    }

    public void Add(Category entity)
    {
        var sql = @"INSERT INTO Category (Name, Description, ThumbnailUrl, ParentCategoryId)
                     VALUES (@Name, @Description, @ThumbnailUrl, @ParentCategoryId);
                     SELECT SCOPE_IDENTITY();";
        entity.Id = Convert.ToInt32(_db.ExecuteScalar(sql,
            new SqlParameter("@Name", entity.Name),
            new SqlParameter("@Description", string.IsNullOrEmpty(entity.Description) ? DBNull.Value : entity.Description),
            new SqlParameter("@ThumbnailUrl", string.IsNullOrEmpty(entity.ThumbnailUrl) ? DBNull.Value : entity.ThumbnailUrl),
            new SqlParameter("@ParentCategoryId", entity.ParentCategoryId.HasValue ? entity.ParentCategoryId.Value : DBNull.Value)));
    }

    public void Update(Category entity)
    {
        _db.ExecuteNonQuery(
            "UPDATE Category SET Name = @Name, Description = @Description, ThumbnailUrl = @ThumbnailUrl, ParentCategoryId = @ParentCategoryId WHERE Id = @Id",
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@Name", entity.Name),
            new SqlParameter("@Description", string.IsNullOrEmpty(entity.Description) ? DBNull.Value : entity.Description),
            new SqlParameter("@ThumbnailUrl", string.IsNullOrEmpty(entity.ThumbnailUrl) ? DBNull.Value : entity.ThumbnailUrl),
            new SqlParameter("@ParentCategoryId", entity.ParentCategoryId.HasValue ? entity.ParentCategoryId.Value : DBNull.Value));
    }

    public void Delete(Category entity)
    {
        _db.ExecuteNonQuery("DELETE FROM Category WHERE Id = @Id", new SqlParameter("@Id", entity.Id));
    }

    private static Category MapCategory(DataRow row) => new()
    {
        Id = row.GetInt("Id"),
        Name = row.GetString("Name"),
        Description = row.GetString("Description"),
        ThumbnailUrl = row.GetString("ThumbnailUrl"),
        ParentCategoryId = row.GetIntNullable("ParentCategoryId")
    };

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
}
