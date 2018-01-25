using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Extensions.Collections
{
  /// <summary>
  /// Estesioni sulle collections e IEnumerable
  /// </summary>
  public static class CollectionsExtension
  {
    /// <summary>
    /// Converte una lista di stringhe in una unica delimitata da un separatore
    /// </summary>
    /// <param name="stringList">lista di elementi</param>
    /// <param name="delimiter">separatore tra elementi default: ';'</param>
    /// <returns>una stringa di elementi delimitati da un carattere separatore</returns>
    public static string Stringify(this IEnumerable<string> stringList, string delimiter = ";")
    {
      if (stringList == null || !stringList.Any())
      {
        return string.Empty;
      }

      var sb = new StringBuilder();
      var list = stringList.ToList();

      var lastIndex = list.Count - 1;
      var index = 0;

      foreach (var item in list)
      {
        sb.Append(item);
        if (index < lastIndex)
          sb.Append(delimiter);
        index++;
      }

      return sb.ToString();
    }

    /// <summary>
    /// Divide una lista di elementi in n parti suddivise equamente
    /// </summary>
    /// <typeparam name="T">Tipo dell'enumeratore</typeparam>
    /// <param name="list">lista di elementi</param>
    /// <param name="parts">numero di parti da suddividere</param>
    /// <returns>lista di liste suddivide equamente</returns>
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
    {
      if (parts == 1)
        return new List<IEnumerable<T>> { list };
      var index = 0;
      var splits = from item in list
                   group item by index++ % parts into part
                   select part.AsEnumerable();
      return splits;
    }
  }
}
