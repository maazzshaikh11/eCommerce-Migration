using System.Data;

namespace Ecommerce.Api.Data;

public static class DataRowExtensions
{
    public static string GetString(this DataRow row, string column) => row.IsNull(column) ? string.Empty : row[column].ToString()!;
    public static int GetInt(this DataRow row, string column) => row.IsNull(column) ? 0 : Convert.ToInt32(row[column]);
    public static int? GetIntNullable(this DataRow row, string column) => row.IsNull(column) ? null : Convert.ToInt32(row[column]);
    public static decimal GetDecimal(this DataRow row, string column) => row.IsNull(column) ? 0m : Convert.ToDecimal(row[column]);
    public static bool GetBool(this DataRow row, string column) => row.IsNull(column) ? false : Convert.ToBoolean(row[column]);
    public static DateTime GetDateTime(this DataRow row, string column) => row.IsNull(column) ? DateTime.MinValue : Convert.ToDateTime(row[column]);
}
