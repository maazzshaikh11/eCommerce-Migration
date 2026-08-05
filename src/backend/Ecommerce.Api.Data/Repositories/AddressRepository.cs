using System.Data;
using Microsoft.Data.SqlClient;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Data.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly DatabaseHelper _db;

    public AddressRepository(DatabaseHelper db)
    {
        _db = db;
    }

    public Address? GetById(int id)
    {
        var sql = "SELECT * FROM Address WHERE Id = @Id";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@Id", id));
        if (dt.Rows.Count == 0) return null;
        return MapAddress(dt.Rows[0]);
    }

    public IEnumerable<Address> GetByCustomerId(int customerId)
    {
        var sql = "SELECT * FROM Address WHERE CustomerId = @CustomerId";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@CustomerId", customerId));
        foreach (DataRow row in dt.Rows)
            yield return MapAddress(row);
    }

    public void Add(Address entity)
    {
        var sql = @"INSERT INTO Address (CustomerId, AddressType, StreetAddress, City, StateProvince, PostalCode, Country, IsDefault)
                     VALUES (@CustomerId, @AddressType, @StreetAddress, @City, @StateProvince, @PostalCode, @Country, @IsDefault);
                     SELECT SCOPE_IDENTITY();";
        entity.Id = Convert.ToInt32(_db.ExecuteScalar(sql,
            new SqlParameter("@CustomerId", entity.CustomerId),
            new SqlParameter("@AddressType", entity.AddressType),
            new SqlParameter("@StreetAddress", entity.StreetAddress),
            new SqlParameter("@City", entity.City),
            new SqlParameter("@StateProvince", entity.StateProvince ?? (object)DBNull.Value),
            new SqlParameter("@PostalCode", entity.PostalCode ?? (object)DBNull.Value),
            new SqlParameter("@Country", entity.Country),
            new SqlParameter("@IsDefault", entity.IsDefault)));
    }

    public void Update(Address entity)
    {
        var sql = @"UPDATE Address SET CustomerId = @CustomerId, AddressType = @AddressType,
                     StreetAddress = @StreetAddress, City = @City, StateProvince = @StateProvince,
                     PostalCode = @PostalCode, Country = @Country, IsDefault = @IsDefault WHERE Id = @Id";
        _db.ExecuteNonQuery(sql,
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@CustomerId", entity.CustomerId),
            new SqlParameter("@AddressType", entity.AddressType),
            new SqlParameter("@StreetAddress", entity.StreetAddress),
            new SqlParameter("@City", entity.City),
            new SqlParameter("@StateProvince", entity.StateProvince ?? (object)DBNull.Value),
            new SqlParameter("@PostalCode", entity.PostalCode ?? (object)DBNull.Value),
            new SqlParameter("@Country", entity.Country),
            new SqlParameter("@IsDefault", entity.IsDefault));
    }

    public void Delete(Address entity)
    {
        var sql = "DELETE FROM Address WHERE Id = @Id";
        _db.ExecuteNonQuery(sql, new SqlParameter("@Id", entity.Id));
    }

    private static Address MapAddress(DataRow row)
    {
        return new Address
        {
            Id = row.GetInt("Id"),
            CustomerId = row.GetInt("CustomerId"),
            AddressType = row.GetString("AddressType"),
            StreetAddress = row.GetString("StreetAddress"),
            City = row.GetString("City"),
            StateProvince = row.GetString("StateProvince"),
            PostalCode = row.GetString("PostalCode"),
            Country = row.GetString("Country"),
            IsDefault = row.GetBool("IsDefault")
        };
    }
}
