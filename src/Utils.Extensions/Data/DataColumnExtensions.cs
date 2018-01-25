using System.Data;
using Utils.Extensions.Reflection;

namespace Utils.Extensions.Data
{
  /// <summary>
  /// Raccolta di estezioni a DataColumn
  /// </summary>
  public static class DataColumnExtensions
  {
    /// <summary>
    /// recupera un SqlDbType in formato stringa da una colonna di una data table
    /// </summary>
    /// <param name="column">colonna della DataTable</param>
    /// <returns></returns>
    public static string GetTypeString(this DataColumn column)
    {
      var dbtype = column.DataType.GetDbType();
      var typeString = dbtype.ToString();
      switch (dbtype)
      {
        case SqlDbType.Binary:
        case SqlDbType.Decimal:
        case SqlDbType.NVarChar:
        case SqlDbType.VarBinary:
        case SqlDbType.VarChar:
        case SqlDbType.NChar:
        case SqlDbType.NText:
        case SqlDbType.Char:
          return $"{typeString} ({column.MaxLength})";
      }
      return typeString;
    }
  }
}