using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Utils.Extensions.Xml
{
  /// <summary>
  /// Estensioni sugli XObject
  /// </summary>
  public static class XObjectsExtensions
  { 
    /// <summary>
    /// Funzione di concatenazione di oggetti da concatenare in formato stringa delimitati da un separatore
    /// Utilizzando il metodo ToString dell'oggetto
    /// </summary>
    /// <typeparam name="T">Generics del tipo da concatenare</typeparam>
    /// <param name="source">Lista degli oggetti da concatenare</param>
    /// <param name="separator">Stringa di delimitazione</param>
    /// <returns>Una stringa contenente la concatenazione degli oggetti presenti nella lista</returns>
    public static string StrCat<T>(this IEnumerable<T> source, string separator)
    {
      return source.Aggregate(new StringBuilder(),
        (sb, i) => sb
          .Append(i.ToString())
          .Append(separator),
        s => s.ToString());
    }

    /// <summary>
    /// Recupera l'esspressione xpath di un determinato XObject
    /// </summary>
    /// <param name="xobj">Oggetto da valutare</param>
    /// <returns>xpath in formato stringa</returns>
    public static string GetXPath(this XObject xobj)
    {
      if (xobj.Parent == null)
      {
        var doc = xobj as XDocument;
        if (doc != null)
        {
          return ".";
        }

        var el = xobj as XElement;
        if (el != null)
        {
          return "/" + NameWithPredicate(el);
        }

        XText xt = xobj as XText;
        if (xt != null)
        {
          return null;
        }

        XComment com = xobj as XComment;
        if (com != null)
          return
            "/" +
            (
              com
                .Document
                .Nodes()
                .OfType<XComment>()
                .Count() != 1 ?
                "comment()[" +
                ( com
                  .NodesBeforeSelf()
                  .OfType<XComment>()
                  .Count() + 1 ) +
                "]" :
                "comment()"
              );
        XProcessingInstruction pi = xobj as XProcessingInstruction;
        if (pi != null)
          return
            "/" +
            (
              pi.Document.Nodes()
                .OfType<XProcessingInstruction>()
                .Count() != 1 ?
                "processing-instruction()[" +
                ( pi
                  .NodesBeforeSelf()
                  .OfType<XProcessingInstruction>()
                  .Count() + 1 ) +
                "]" :
                "processing-instruction()"
              );
        return null;
      }
      else
      {
        XElement el = xobj as XElement;
        if (el != null)
        {
          return
            "/" +
            el
              .Ancestors()
              .InDocumentOrder()
              .Select(NameWithPredicate)
              .StrCat("/") +
            NameWithPredicate(el);
        }
        XAttribute at = xobj as XAttribute;
        if (at != null)
          return
            "/" +
            at
              .Parent
              .AncestorsAndSelf()
              .InDocumentOrder()
              .Select(NameWithPredicate)
              .StrCat("/") +
            "@" + GetQName(at);
        XComment com = xobj as XComment;
        if (com != null)
          return
            "/" +
            com
              .Parent
              .AncestorsAndSelf()
              .InDocumentOrder()
              .Select(NameWithPredicate)
              .StrCat("/") +
            (
              com
                .Parent
                .Nodes()
                .OfType<XComment>()
                .Count() != 1 ?
                "comment()[" +
                ( com
                  .NodesBeforeSelf()
                  .OfType<XComment>()
                  .Count() + 1 ) + "]" :
                "comment()"
              );
        XCData cd = xobj as XCData;
        if (cd != null)
          return
            "/" +
            cd
              .Parent
              .AncestorsAndSelf()
              .InDocumentOrder()
              .Select(NameWithPredicate)
              .StrCat("/") +
            (
              cd
                .Parent
                .Nodes()
                .OfType<XText>()
                .Count() != 1 ?
                "text()[" +
                ( cd
                  .NodesBeforeSelf()
                  .OfType<XText>()
                  .Count() + 1 ) + "]" :
                "text()"
              );
        XText tx = xobj as XText;
        if (tx != null)
          return
            "/" +
            tx
              .Parent
              .AncestorsAndSelf()
              .InDocumentOrder()
              .Select(NameWithPredicate)
              .StrCat("/") +
            (
              tx
                .Parent
                .Nodes()
                .OfType<XText>()
                .Count() != 1 ?
                "text()[" +
                ( tx
                  .NodesBeforeSelf()
                  .OfType<XText>()
                  .Count() + 1 ) + "]" :
                "text()"
              );
        XProcessingInstruction pi = xobj as XProcessingInstruction;
        if (pi != null)
          return
            "/" +
            pi
              .Parent
              .AncestorsAndSelf()
              .InDocumentOrder()
              .Select(NameWithPredicate)
              .StrCat("/") +
            (
              pi
                .Parent
                .Nodes()
                .OfType<XProcessingInstruction>()
                .Count() != 1 ?
                "processing-instruction()[" +
                ( pi
                  .NodesBeforeSelf()
                  .OfType<XProcessingInstruction>()
                  .Count() + 1 ) + "]" :
                "processing-instruction()"
              );
        return null;
      }
    }

    /// <summary>
    /// Recupera ricorsivamente tutti gli XObject relativi ad un source
    /// </summary>
    /// <param name="source">XObject sorgente da cui recuperare tutti gli oggetti</param>
    /// <returns>lista degli oggetti appartenenti alla radice source</returns>
    public static IEnumerable<XObject> DescendantXObjects(this XObject source)
    {
      yield return source;
      XElement el = source as XElement;
      if (el != null)
        foreach (XAttribute att in el.Attributes())
          if (!att.IsNamespaceDeclaration)
            yield return att;
      XContainer con = source as XContainer;
      if (con == null)
        yield break;

      foreach (XObject s in con.Nodes().SelectMany(child => child.DescendantXObjects()))
        yield return s;
    }

    private static string GetQName(this XElement xe)
    {
      string prefix = xe.GetPrefixOfNamespace(xe.Name.Namespace);
      if (xe.Name.Namespace == XNamespace.None || prefix == null)
      {
        return xe.Name.LocalName;
      }
      return $"{prefix}:{xe.Name.LocalName}";
    }

    private static string GetQName(XAttribute xa)
    {
      string prefix = xa.Parent.GetPrefixOfNamespace(xa.Name.Namespace);
      if (xa.Name.Namespace == XNamespace.None || prefix == null)
      {
        return xa.Name.ToString();
      }

      return $"{prefix}:{xa.Name.LocalName}";
    }

    private static string NameWithPredicate(XElement el)
    {
      if (el.Parent != null && el.Parent.Elements(el.Name).Count() != 1)
        return $"{XObjectsExtensions.GetQName(el)}[{(el.ElementsBeforeSelf(el.Name).Count() + 1)}]";

      return GetQName(el);
    }
  }
}