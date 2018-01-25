using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utils.Extensions.Reflection;

namespace Utils.Extensions.Data
{
  /// <summary>
  /// Raccolta di estesioni per la classe DataTable
  /// </summary>
  public static class DataTableExtensions
  {
    /// <summary>
    /// Indica se una DataTable è null
    /// </summary>
    /// <param name="table">DataTable da valutare</param>
    /// <returns>true se è null, false altrimenti</returns>
    public static bool IsNull(this DataTable table)
    {
      return table == null;
    }

    /// <summary>
    /// Indica se una DataTable contiene records
    /// </summary>
    /// <param name="table">DataTable da valutare</param>
    /// <returns>true se contiene righe false altrimenti</returns>
    public static bool HasRows(this DataTable table)
    {
      return !table.IsNull() && table.Rows.Count > 0;
    }

    /// <summary>
    ///  Indica se una DataTable è vuota
    /// </summary>
    /// <param name="table">DataTable da valutare</param>
    /// <returns>true se vuota, false altrimenti</returns>
    public static bool IsEmpty(this DataTable table)
    {
      return table.IsNull() || !table.IsInitialized || !table.HasRows();
    }

    /// <summary>
    /// Aggiunge la colonna Totale a seconda dei parametri effettuando la somma delle colonne incluse
    /// </summary>
    /// <param name="table">DataTable da valutare</param>
    /// <param name="label">Nome colonna del Totale</param>
    /// <param name="isRow">se true aggiunge la riga totale con la somma dei valori delle colonne incluse, se false aggiunge la colonna con il totale delle celle relative alle colonne incluse</param>
    /// <param name="includeKeys">se null effettua il totale di tutte colonne, altrimenti solo delle colonne indicate</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddSum(this DataTable table, string label, bool isRow = true, IList<string> includeKeys = null)
    {
      if (table == null)
        throw new ArgumentNullException("table");
      if (string.IsNullOrEmpty(label))
        throw new ArgumentNullException("label");

      if (includeKeys == null)
        includeKeys = new List<string>();

      var cols = new DataColumn[table.Columns.Count];
      table.Columns.CopyTo(cols, 0);
      IList<DataColumn> intCols = cols.Where(col => col.DataType.IsNumeric()).ToList();
      if (isRow)
      {
        var totalRow = table.NewRow();
        totalRow[0] = label;
        foreach (var dataColumn in intCols)
        {
          var colSum = ( from DataRow row in table.Rows
                         where includeKeys.Contains(row[0] as string) || !includeKeys.Any()
                         select Convert.ToInt32(row[dataColumn]) ).Sum();
          totalRow[dataColumn] = colSum;
        }
        table.Rows.Add(totalRow);
      }
      else
      {
        var totalColumn = new DataColumn(label, typeof(int));
        table.Columns.Add(totalColumn);
        foreach (DataRow row in table.Rows)
        {
          var rowSum = intCols.Where(dataColumn => includeKeys.Contains(dataColumn.ColumnName) || !includeKeys.Any()).Sum(dataColumn => Convert.ToInt32(row[dataColumn]));
          row[totalColumn] = rowSum;
        }
      }
    }

    /// <summary>
    /// Costruisce il filtro/query di eguaglianza per DataTable
    /// </summary>
    /// <param name="table">DataTable sorgente/</param>
    /// <param name="paramName">Nome del campo da filtrare</param>
    /// <param name="value">Valore da filtrare</param>
    /// <param name="valueType">Tipo di valore del campo da filtrare</param>
    /// <returns>La query in formato stringa per filtrare le DataRow della DataTable</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static string CreateFilterEQ(this DataTable table, string paramName, object value, Type valueType = null)
    {
      if (table == null)
      {
        throw new ArgumentNullException("table");
      }
      if (paramName == null)
      {
        throw new ArgumentNullException("paramName");
      }

      if (!table.Columns.Contains(paramName))
      {
        throw new Exception("La colonna selezionata non esiste");
      }

      if (valueType == null)
      {
        valueType = value == null ? typeof(object) : value.GetType();
      }

      string commandFormat = "{0} {1} {2}";

      string operatore = value == null ? "is" : "=";
      string stringValue =
        value == null
          ? "null"
          : valueType.IsNumeric()
            ? value.ToString()
            : $"'{value}'";


      return string.Format(
        commandFormat,
        paramName,
        operatore,
        stringValue);
    }
  }
}