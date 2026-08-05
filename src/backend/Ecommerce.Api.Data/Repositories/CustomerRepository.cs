using System.Data;
using Microsoft.Data.SqlClient;
using Ecommerce.Api.Core.Interfaces;
using Ecommerce.Api.Core.Models;

namespace Ecommerce.Api.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DatabaseHelper _db;

    public CustomerRepository(DatabaseHelper db)
    {
        _db = db;
    }

    public Customer? GetById(int id)
    {
        var sql = "SELECT * FROM Customer WHERE Id = @Id";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@Id", id));
        if (dt.Rows.Count == 0) return null;
        return MapCustomer(dt.Rows[0]);
    }

    public Customer? GetByEmail(string email)
    {
        var sql = "SELECT * FROM Customer WHERE Email = @Email";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@Email", email));
        if (dt.Rows.Count == 0) return null;
        return MapCustomer(dt.Rows[0]);
    }

    public Customer? GetByIdentityUserId(string identityUserId)
    {
        var sql = "SELECT * FROM Customer WHERE IdentityUserId = @IdentityUserId";
        var dt = _db.ExecuteQuery(sql, new SqlParameter("@IdentityUserId", identityUserId));
        if (dt.Rows.Count == 0) return null;
        return MapCustomer(dt.Rows[0]);
    }

    public void Add(Customer entity)
    {
        var sql = @"INSERT INTO Customer (IdentityUserId, Email, FirstName, LastName, PhoneNumber, CreatedDate)
                     VALUES (@IdentityUserId, @Email, @FirstName, @LastName, @PhoneNumber, @CreatedDate);
                     SELECT SCOPE_IDENTITY();";
        entity.Id = Convert.ToInt32(_db.ExecuteScalar(sql,
            new SqlParameter("@IdentityUserId", entity.IdentityUserId ?? (object)DBNull.Value),
            new SqlParameter("@Email", entity.Email),
            new SqlParameter("@FirstName", entity.FirstName),
            new SqlParameter("@LastName", entity.LastName),
            new SqlParameter("@PhoneNumber", entity.PhoneNumber ?? (object)DBNull.Value),
            new SqlParameter("@CreatedDate", entity.CreatedDate)));
    }

    public void Update(Customer entity)
    {
        var sql = @"UPDATE Customer SET IdentityUserId = @IdentityUserId, Email = @Email,
                     FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber
                     WHERE Id = @Id";
        _db.ExecuteNonQuery(sql,
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@IdentityUserId", entity.IdentityUserId ?? (object)DBNull.Value),
            new SqlParameter("@Email", entity.Email),
            new SqlParameter("@FirstName", entity.FirstName),
            new SqlParameter("@LastName", entity.LastName),
            new SqlParameter("@PhoneNumber", entity.PhoneNumber ?? (object)DBNull.Value));
    }

    public void Delete(Customer entity)
    {
        var sql = "DELETE FROM Customer WHERE Id = @Id";
        _db.ExecuteNonQuery(sql, new SqlParameter("@Id", entity.Id));
    }

    private static Customer MapCustomer(DataRow row)
    {
        return new Customer
        {
            Id = row.GetInt("Id"),
            IdentityUserId = row.GetString("IdentityUserId"),
            Email = row.GetString("Email"),
            FirstName = row.GetString("FirstName"),
            LastName = row.GetString("LastName"),
            PhoneNumber = row.GetString("PhoneNumber"),
            CreatedDate = row.GetDateTime("CreatedDate")
        };
    }
}
