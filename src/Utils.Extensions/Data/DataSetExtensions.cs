using System.Data;

namespace Utils.Extensions.Data
{
  /// <summary>
  /// Raccolta di estesioni per la classe DataSet
  /// </summary>
  public static class DataSetExtensions
  {
    /// <summary>
    /// Controlla se un dataSet è null
    /// </summary>
    /// <param name="dataSet">Il dataset da verificare</param>
    /// <returns>true se null, false altrimenti</returns>
    public static bool IsNull(this DataSet dataSet)
    {
      return dataSet == null;
    }

    /// <summary>
    /// Controlla se un dataSet è vuoto
    /// Null, non ha tabelle o una tabella con 0 righe
    /// </summary>
    /// <param name="dataSet"></param>
    /// <returns></returns>
    public static bool IsEmpty(this DataSet dataSet)
    {
      return dataSet.IsNull() ||
             !dataSet.HasTables() ||
             ( dataSet.Tables.Count == 1 && dataSet.Tables[0].Rows.Count == 0 );
    }

    /// <summary>
    /// Il metodo indica se un dataset contiene tabelle
    /// </summary>
    /// <param name="dataSet">DataSet da valutare</param>
    /// <returns>true se esistono tabelle false altrimenti</returns>
    public static bool HasTables(this DataSet dataSet)
    {
      return !dataSet.IsNull() && dataSet.Tables.Count > 0;
    }
  }
}
