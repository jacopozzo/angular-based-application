using System.Collections.Generic;

namespace Utils.Extensions.Data
{
  /// <summary>
  /// Classe di appoggio per l'astrazione di una Tabella
  /// </summary>
  public class TableInfo
  {
    /// <summary>
    /// Nome della tabella
    /// </summary>
    public string Name
    {
      get;
      set;
    }

    /// <summary>
    /// Nomi dei campi
    /// </summary>
    public IList<string> Fields
    {
      get;
      set;
    }

    /// <summary>
    /// Comando Create
    /// </summary>
    public string Create
    {
      get;
      set;
    }
  }
}