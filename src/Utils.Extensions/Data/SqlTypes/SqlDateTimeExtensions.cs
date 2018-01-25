using System;
using System.Data.SqlTypes;

namespace Utils.Extensions.Data.SqlTypes
{

  /// <summary>
  /// Estesioni di SqlDateTime
  /// </summary>
  public static class SqlDateTimeExtensions
  {
    /// <summary>
    /// Effettua un try parse come DateTime.TryParse per SqlDateTime
    /// </summary>
    /// <param name="sqlDataString">SqlDateTime in formato stringa</param>
    /// <param name="sqlDateTime">parametro in out da valorizzare, in caso di errore sqlDateTime viene valorizzato con SqlDateTime.MinValue</param>
    /// <returns>true se il Parse è andato a buon fine, false altrimenti</returns>
    public static bool TryParse(this string sqlDataString, out SqlDateTime sqlDateTime)
    {
      try
      {
        sqlDateTime = SqlDateTime.Parse(sqlDataString);
      }
      catch (Exception)
      {

        sqlDateTime = SqlDateTime.MinValue;
      }

      return true;
    }
  }
}
