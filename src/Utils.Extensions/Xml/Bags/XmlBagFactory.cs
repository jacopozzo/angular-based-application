using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Utils.Extensions.Xml.Bags
{
  /// <summary>
  /// Factory per la creazione di una strategia di navigazione dell'xml (incapsulamento)
  /// </summary>
  public class XmlBagFactory
  {
    /// <summary>
    /// Crea una Xml Bag in base alla strategia richiesta
    /// </summary>
    /// <param name="xmlString">xml in formato stringa da valutare</param>
    /// <param name="navigationStrategy">Strategia di navigazione</param>
    /// <param name="editable">Se le operazioni sull'xml sono in sola lettura conviene utilizzare in modalita non modificabile, altrimenti true</param>
    /// <returns>Implementazione della strategia di navigazione</returns>
    public static IXmlBag Create(string xmlString, NavigationStrategy navigationStrategy = NavigationStrategy.XElementNavigator, bool editable = false)
    {
      if (xmlString == null)
      {
        throw new ArgumentNullException(nameof(xmlString));
      }

      switch (navigationStrategy)
      {
        case NavigationStrategy.XPathNavigator:
          return new XPathNavigatorBag(xmlString, editable);
        case NavigationStrategy.XElementNavigator:
          return new XElementBag(xmlString);
        default:
          throw new ArgumentOutOfRangeException(nameof(navigationStrategy));
      }
    }

    /// <summary>
    /// Recupera l'XPathNavigator radice della xmlBag passata, se non è di tipo XPathNavigator ne crea una nuova a partire dalla stringa Xml della xmlBag
    /// </summary>
    /// <param name="xmlBag">xmlBag sulla quale recuperare il Navigator radice</param>
    /// <returns>XPathNavigator corrispondente</returns>
    public static XPathNavigator GetNavigator(IXmlBag xmlBag)
    {
      if (xmlBag == null)
      {
        throw new ArgumentNullException(nameof(xmlBag));
      }

      if(xmlBag.GetType() == typeof(XPathNavigatorBag))
      {
        return ((XPathNavigatorBag) xmlBag).Navigator;
      }

      var newBag = new XPathNavigatorBag(xmlBag.GetRawXml(), false);
      return newBag.Navigator;
    }

    /// <summary>
    ///  Recupera l'XElement radice della xmlBag passata, se non è di tipo XElement ne crea una nuova a partire dalla stringa Xml della xmlBag
    /// </summary>
    /// <param name="xmlBag">xmlBag sulla quale recuperare l'XElement radice</param>
    /// <returns>XElement corrispondente</returns>
    public static XElement GetXElement(IXmlBag xmlBag)
    {
      if (xmlBag == null)
      {
        throw new ArgumentNullException(nameof(xmlBag));
      }

      if (xmlBag.GetType() == typeof(XElementBag))
      {
        return ( (XElementBag)xmlBag ).RootElement;
      }

      var newBag = new XElementBag(xmlBag.GetRawXml());
      return newBag.RootElement;
    }
  }
}