using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Utils.Extensions.Data
{

  /// <summary>
  /// Raccolta di estesioni per la classe SqlParameter
  /// </summary>
  public static class SqlParameterExtensions
  {
    /// <summary>
    /// Data una lista di SqlParameter normalizza i valori null in DBNull.Value
    /// </summary>
    /// <param name="parameters">Lista di SqlParameter</param>
    public static void Normalize(this IEnumerable<SqlParameter> parameters)
    {
      parameters.ToList().ForEach(parameter =>
      {
        if (parameter.Value == null)
        {
          parameter.Value = DBNull.Value;
        }
      });
    }
  }
}