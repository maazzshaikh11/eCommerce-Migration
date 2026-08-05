using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.Api.Data;

public class DatabaseHelper : IDisposable
{
    private readonly string _connectionString;
    private SqlConnection? _connection;

    public SqlTransaction? CurrentTransaction { get; set; }

    public DatabaseHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("LegacyEcommerceDb")
            ?? throw new InvalidOperationException("Connection string 'LegacyEcommerceDb' not configured.");
    }

    public virtual SqlConnection GetConnection()
    {
        if (_connection == null)
        {
            _connection = new SqlConnection(_connectionString);
        }
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }
        return _connection;
    }

    public DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
    {
        var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn, CurrentTransaction);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        var dt = new DataTable();
        using var reader = cmd.ExecuteReader();
        dt.Load(reader);
        return dt;
    }

    public DataTable ExecuteQuery(string sql, SqlTransaction transaction, params SqlParameter[] parameters)
    {
        using var cmd = new SqlCommand(sql, transaction.Connection!, transaction);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        var dt = new DataTable();
        using var reader = cmd.ExecuteReader();
        dt.Load(reader);
        return dt;
    }

    public object ExecuteScalar(string sql, params SqlParameter[] parameters)
    {
        var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn, CurrentTransaction);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }

    public object ExecuteScalar(string sql, SqlTransaction transaction, params SqlParameter[] parameters)
    {
        using var cmd = new SqlCommand(sql, transaction.Connection!, transaction);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteScalar();
    }

    public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
    {
        var conn = GetConnection();
        using var cmd = new SqlCommand(sql, conn, CurrentTransaction);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }

    public int ExecuteNonQuery(string sql, SqlTransaction transaction, params SqlParameter[] parameters)
    {
        using var cmd = new SqlCommand(sql, transaction.Connection!, transaction);
        if (parameters != null && parameters.Length > 0)
            cmd.Parameters.AddRange(parameters);
        return cmd.ExecuteNonQuery();
    }

    public virtual SqlTransaction BeginTransaction()
    {
        return GetConnection().BeginTransaction();
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }
}