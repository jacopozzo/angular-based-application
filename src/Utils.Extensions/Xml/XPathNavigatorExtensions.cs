using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Utils.Extensions.Reflection;

namespace Utils.Extensions.Xml
{
  /// <summary>
  /// Raccolta di estesioni per XPathNavigator
  /// </summary>
  public static class XPathNavigatorExtensions
  {

    /// <summary>
    /// Recupera l'xml dato un XPathNavigator
    /// </summary>
    /// <param name="navigator">XPathNavigator da esportare</param>
    /// <returns>stringa Xml</returns>
    public static string GetRawXml(this XPathNavigator navigator)
    {
        return navigator == null ? string.Empty : navigator.OuterXml;
    }

      /// <summary>
    /// Aggiorna il un campo xml con il valore specificato per l'elemento radice
    /// </summary>
    /// <param name="navigator">Elemento da modificare</param>
    /// <param name="xPath">XPath da valutare per il SetValue</param>
    /// <param name="value">Valore da aggiornare</param>
    public static void Update(this XPathNavigator navigator, string xPath, string value)
    {
      if (navigator == null)
      {
        throw new ArgumentNullException(nameof(navigator));
      }
      if (string.IsNullOrEmpty(xPath))
      {
        throw new ArgumentNullException(nameof(xPath));
      }

      var elementToUpdate = navigator
        .SelectFromXPath(navigator.GenerateExpression(xPath), stopAtFirstResult:true, forUpdate:true)
        .FirstOrDefault();
      if (elementToUpdate == null)
      {
        throw new ArgumentException("Non esiste nessun nodo per l'xpath specificato", "xPath");
      }

      elementToUpdate.SetValue(value);
    }

    /// <summary>
    /// Per ogni valore riscontrato tramite l'xPath costruisce un dizionario con l'xpath specifico come chiave e il valore dell'xpath valutato come valore
    /// Es:   {"//books/book[1]/authors/author[1]", "Antonio"},{"//books/book[1]/authors/author[2]", "Erasmo"},{"//books/book[1]/authors/author[3]", "Renata"}, ecc...
    /// </summary>
    /// <typeparam name="TType">Tipo di valore da salvare nel dizionario come valore</typeparam>
    /// <param name="nav">Navigator da valutare</param>
    /// <param name="xPath">Espressione XPath Generica  Es. //books/book/authors/author</param>
    /// <returns>Dizionario xPath esplicito, valore</returns>
    public static IDictionary<string, TType> GetValuesFromXPathDictionary<TType>(this XPathNavigator nav, string xPath)
    {
      IDictionary<string, TType> dictionary = new Dictionary<string, TType>();

      XPathExpression expr = nav.Compile(xPath);
      XPathNodeIterator iterator = nav.Select(expr);

      while (iterator.MoveNext())
      {
        XPathNavigator currentNavigator = iterator.Current.Clone();

        var currentXPath = currentNavigator.GetFullXPath();
        var currentValue = String.IsNullOrEmpty(currentNavigator.InnerXml) ? currentNavigator.OuterXml : nav.InnerXml;
        dictionary.Add(currentXPath, currentValue.GetValue<TType>());
      }

      return dictionary;
    }

    private static string GetFullXPath(this XPathNavigator navigator)
    {
      var path = new StringBuilder();
      for (var node = navigator.UnderlyingObject as XmlNode; node != null; node = node.ParentNode)
      {
        var appendBuilder = new StringBuilder();
        appendBuilder.AppendFormat("/{0}", path);
        

        if (node.ParentNode != null && node.ParentNode.ChildNodes.Count > 1)
        {
          appendBuilder.Append("[");

          int index = 1;
          while (node.PreviousSibling != null)
          {
            index++;
          }

          appendBuilder.Append("]");
        }

        path.Insert(0, appendBuilder.ToString());
      }

      return path.ToString();
    }


    /// <summary>
    /// Verifica se un xpath riscontra dei valori nel navigator
    /// </summary>
    /// <param name="navigator">Navigator da valutare</param>
    /// <param name="xPath">espressione xpath</param>
    /// <returns>true se riscontra valori, false altrimenti</returns>
    public static bool XPathExists(this XPathNavigator navigator, string xPath)
    {
      var node = navigator.SelectSingleNode(xPath);
      return node != null;
    }

    /// <summary>
    /// Recupera il primo valore trovato per l'xpath specificato utilizzando XPathNavigator
    /// </summary>
    /// <typeparam name="TType">Tipo di ritorno, effettua il cast utilizzando GetValue-TTYpe</typeparam>
    /// <param name="nav">XPathNavigator da valutare</param>
    /// <param name="xPath">query xpath</param>
    /// <returns>il valore recuperato</returns>
    public static TType GetValueFromXPath<TType>(this XPathNavigator nav, string xPath)
    {
      return nav.GetValuesFromXPath<TType>(xPath, true).FirstOrDefault();
    }

    /// <summary>
    /// Recupera gli XPath Navigator data una XPathExpression
    /// </summary>
    /// <param name="nav">Navigator di partenza</param>
    /// <param name="xPathExpression">XPath expression da valutare</param>
    /// <param name="stopAtFirstResult">Si ferma al primo risultato se true, false altrimenti</param>
    /// <param name="forUpdate">per l'update di un unico elemento restituisce l'xPathNavigator corrente da poter modificare </param>
    /// <returns>Lista degli XPathNavigator</returns>
    public static IEnumerable<XPathNavigator> SelectFromXPath(this XPathNavigator nav, XPathExpression xPathExpression, bool stopAtFirstResult = false, bool forUpdate = false)
    {
      var navigators = new List<XPathNavigator>();
      XPathNodeIterator iterator = nav.Select(xPathExpression);
      while (iterator.MoveNext())
      {
        navigators.Add(forUpdate ? iterator.Current : iterator.Current.Clone());
        if (stopAtFirstResult)
          break;
      }

      return navigators;
    }

    /// <summary>
    /// Genera una XPathExpression dato un xPath
    /// </summary>
    /// <param name="navigator">Navigator da valutare</param>
    /// <param name="xPath">xPath da convertire</param>
    /// <returns>XPathExpression corrispondente</returns>
    public static XPathExpression GenerateExpression(this XPathNavigator navigator, string xPath)
    {
      if (!String.IsNullOrEmpty(navigator.Prefix) && xPath.Contains($"{navigator.Prefix}:"))
      {
        xPath = xPath.AddPrefix(navigator.Prefix);
      }

      XPathExpression expr = navigator.Compile(xPath);
      navigator.ManagePrefixes(expr);

      return expr;
    }

    /// <summary>
    /// Recupera i valori trovati per l'xpath specificato utilizzando XPathNavigator
    /// </summary>
    /// <typeparam name="TType">Tipo di ritorno, effettua il cast utilizzando GetValue-TTYpe</typeparam>
    /// <param name="nav">XPathNavigator da valutare</param>
    /// <param name="xPath">query xpath</param>
    /// <param name="stopAtFirstResult">si ferma al primo riscontro (interrompe il ciclo)</param>
    /// <returns>il valore recuperato</returns>
    public static IEnumerable<TType> GetValuesFromXPath<TType>(this XPathNavigator nav, string xPath, bool stopAtFirstResult = false)
    {
      var expr = nav.GenerateExpression(xPath);

      if (expr.ReturnType == XPathResultType.Boolean)
      {
        var eval = nav.Evaluate(expr);
        return new[] { eval.GetValue<TType>() };
      }

      var navigators = nav.SelectFromXPath(expr, stopAtFirstResult);

      return navigators
        .Select(currentNavigator => 
          (String.IsNullOrEmpty(currentNavigator.InnerXml) ? currentNavigator.OuterXml : currentNavigator.InnerXml)
          .GetValue<TType>())
        .ToList();
    }

    private static void ManagePrefixes(this XPathNavigator nav, XPathExpression expr)
    {
      var nameSpaceManager = new XmlNamespaceManager(nav.NameTable ?? new NameTable());

      if (!String.IsNullOrEmpty(nav.NamespaceURI))
      {
        var namespaceUri = nav.NamespaceURI;
        var prefix = nav.Prefix;
        if (String.IsNullOrEmpty(prefix))
        {
          prefix = "a";
        }
        nameSpaceManager.AddNamespace(prefix, namespaceUri);
        expr.SetContext(nameSpaceManager);
      }
    }
  }
}
