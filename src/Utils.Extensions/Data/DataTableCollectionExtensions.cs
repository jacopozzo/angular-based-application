using System;
using System.Data;

namespace Utils.Extensions.Data
{
  /// <summary>
  /// Raccolta di estesioni per la classe DataTableCollection
  /// </summary>
  public static class DataTableCollectionExtensions
  {
    /// <summary>
    /// Recupera la prima tabella di una DataTableCollection
    /// </summary>
    /// <param name="tables">collection di DataTable</param>
    /// <returns>La prima DataTable disponibile della collection</returns>
    /// <exception cref="ArgumentNullException">tables non può essere null</exception>
    /// <exception cref="ArgumentOutOfRangeException">tables deve contenere elementi</exception>
    public static DataTable First(this DataTableCollection tables)
    {
      if (tables == null)
      {
        throw new ArgumentNullException("tables");
      }

      if (tables.Count == 0)
      {
        throw new ArgumentOutOfRangeException("tables", "la collection non contiene tabelle");
      }

      return tables[0];
    }
   
    /// <summary>
    /// Recupera la prima tabella di una DataTableCollection se esiste, null altrimenti
    /// </summary>
    /// <param name="tables">collection di DataTable</param>
    /// <returns>La prima DataTable disponibile della collection</returns>
    public static DataTable FirstOrDefault(this DataTableCollection tables)
    {
      return tables == null || tables.Count == 0 ? null : tables[0];
    }
  }
}