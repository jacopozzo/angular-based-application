using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Utils.Extensions.Collections;
using Utils.Extensions.Reflection;

namespace Utils.Extensions.Xml
{
    /// <summary>
    /// Raccolta di XElement Extensions
    /// </summary>
    public static class XElementExtensions
    {
        /// <summary>
        /// Recupera l'xml dato un XElement
        /// </summary>
        /// <param name="element">XElement da esportare</param>
        /// <returns>stringa Xml</returns>
        public static string GetRawXml(this XElement element)
        {
            return element?.ToString(SaveOptions.OmitDuplicateNamespaces) ?? string.Empty;
        }

        /// <summary>
        /// Aggiorna il un campo xml con il valore specificato per l'elemento radice
        /// </summary>
        /// <param name="element">Elemento da modificare</param>
        /// <param name="xPath">XPath da valutare per il SetValue</param>
        /// <param name="value">Valore da aggiornare</param>
        public static void Update(this XElement element, string xPath, string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (string.IsNullOrEmpty(xPath))
            {
                throw new ArgumentNullException(nameof(xPath));
            }

            var elementToUpdate = element.SelectFromXPath(xPath);
            if (elementToUpdate == null)
            {
                throw new ArgumentException("Non esiste nessun nodo per l'xpath specificato", nameof(xPath));
            }

            elementToUpdate.SetValue(value);
        }


        /// <summary>
        /// Seleziona l'XElement dato un xPath
        /// </summary>
        /// <param name="rootElement">elemento radice</param>
        /// <param name="xPath">xPath da valutare, viene normalizzato</param>
        /// <param name="nameSpaceManager">Namespace manager, se null viene calcolato</param>
        /// <returns>Elemento recuperato se viene trovato, null altrimenti</returns>
        public static XElement SelectFromXPath(this XElement rootElement, string xPath, XmlNamespaceManager nameSpaceManager = null)
        {
            if (rootElement == null)
            {
                throw new ArgumentNullException(nameof(rootElement));
            }

            var normalizedXPath = xPath.Normalize(rootElement.Name.LocalName);

            if (nameSpaceManager == null)
            {
                nameSpaceManager = rootElement.GetNameSpaceManager(ref normalizedXPath);
            }

            return rootElement.XPathSelectElement(normalizedXPath, nameSpaceManager);
        }

        /// <summary>
        /// Recupera il primo valore trovato per l'xpath specificato utilizzando XElement
        /// </summary>
        /// <typeparam name="TType">Tipo di ritorno, effettua il cast utilizzando GetValue-TTYpe</typeparam>
        /// <param name="element">XElement da valutare</param>
        /// <param name="xPath">query xpath intera, viene normalizzato internamente</param>
        /// <returns>il valore recuperato</returns>
        public static TType GetValueFromXElement<TType>(this XElement element, string xPath)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            var xpathNormalized = xPath.Normalize(element.Name.LocalName);
            var nameSpaceManager = element.GetNameSpaceManager(ref xpathNormalized);

            if (xPath.IsEvaluable())
            {
                return element.Evaluate<TType>(xPath, xpathNormalized, nameSpaceManager);
            }

            if (xpathNormalized.Contains("@"))
            {
                if (xpathNormalized.Contains("/"))
                {
                    var elementXPath = xpathNormalized.Substring(0, xpathNormalized.LastIndexOf('/'));
                    element = element.SelectFromXPath(elementXPath);

                    if (element == null)
                    {
                        return TypeExtensions.GetValue<TType>(null);
                    }

                    xpathNormalized = xpathNormalized.Replace(elementXPath, string.Empty);


                }
                
                if (xpathNormalized.Contains(":"))
                {
                    xpathNormalized = xpathNormalized.Substring(xpathNormalized.IndexOf(':')+1);
                }

                xpathNormalized = xpathNormalized.Replace("@", "");
                var attribute = element.Attribute(XName.Get(xpathNormalized, nameSpaceManager.DefaultNamespace));
                return attribute == null ? TypeExtensions.GetValue<TType>(null) : attribute.Value.GetValue<TType>();
            }


            var selectedElement = element.SelectFromXPath(xpathNormalized);
            if (selectedElement != null && typeof(TType) == typeof(string) && selectedElement.HasElements)
            {
                var xNode = selectedElement.ToString(SaveOptions.None);
                if (xNode.StartsWith("<") && xNode.EndsWith(">"))
                {
                    return xNode.GetValue<TType>();
                }
            }
            var nullObject = default(TType);
            return selectedElement == null ? nullObject.GetValue<TType>() : selectedElement.Value.GetValue<TType>();
        }

        /// <summary>
        /// Evaluates custom substring-match function
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xPath"></param>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TType EvaluateSubstringMatch<TType>(this XElement element, string xPath)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            if (xPath == null)
            {
                throw new ArgumentNullException(nameof(xPath));
            }

            string startToken;
            string endToken;
            string xpathNormalized = xPath.NormalizeSubstringMatchFunction(out startToken, out endToken);
            var result = element.GetValueFromXElement<string>(xpathNormalized);

            if (string.IsNullOrEmpty(result))
            {
                return default(TType);
            }

            if (!result.Contains(startToken))
            {
                startToken = WebUtility.HtmlDecode(startToken);
                if (!result.Contains(startToken))
                {
                    return result.GetValue<TType>();
                }
            }

            if (string.IsNullOrEmpty(endToken))
            {
                return result.Substring(result.IndexOf(startToken)).GetValue<TType>();
            }

            var length = result.IndexOf(endToken) - result.IndexOf(startToken) + endToken.Length;
            return result.Substring(result.IndexOf(startToken), length).GetValue<TType>();


        }


        /// <summary>
        ///   Evaluates an xpath function
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xPath"></param>
        /// <param name="xpathNormalized"></param>
        /// <param name="nameSpaceManager"></param>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        public static TType Evaluate<TType>(
          this XElement element,
          string xPath,
          string xpathNormalized,
          XmlNamespaceManager nameSpaceManager)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (xPath.IsSubstringMatchFunction())
            {
                return element.EvaluateSubstringMatch<TType>(xPath);
            }

            if (xPath.IsStringJoinFunction())
            {
                string delimiter;
                xpathNormalized = xPath.NormalizeStringJoin(out delimiter);

                var list = xpathNormalized.IsEvaluable()
                  ? element.GetValueFromXElement<IEnumerable<string>>(xpathNormalized)
                  : element.GetValuesFromXElement<string>(xpathNormalized);

                //element.XPathEvaluate(xpathNormalized, nameSpaceManager).GetValue<IEnumerable<string>>();
                return list.Stringify(delimiter).GetValue<TType>();
            }
            if (xPath.IsCountFunction())
            {
                string startToken;
                string endToken;
                xpathNormalized = xPath.NormalizeCountFunction(out startToken, out endToken);
                nameSpaceManager = element.GetNameSpaceManager(ref xpathNormalized);
                xpathNormalized = $"{startToken}{xpathNormalized}{endToken}";
            }

            if (xPath.IsDistinctValuesFunction())
            {
                xpathNormalized = xPath.NormalizeDistinctValues();

                var list = xpathNormalized.IsEvaluable()
                  ? element.GetValueFromXElement<IEnumerable<string>>(xpathNormalized)
                  : element.GetValuesFromXElement<string>(xpathNormalized);

                return list.Distinct().GetValue<TType>();
            }

            object eval = null;
            try
            {
                eval = element.XPathEvaluate(xpathNormalized, nameSpaceManager);
                
            }
            catch (XPathException)
            {
                eval = element.XPathEvaluate(xPath, nameSpaceManager);
            }

            return eval.GetValue<TType>();
        }

        /// <summary>
        /// Recupera i valori trovati per l'xpath specificato utilizzando XElement
        /// </summary>
        /// <typeparam name="TType">Tipo di ritorno, effettua il cast utilizzando GetValue-TTYpe</typeparam>
        /// <param name="element">XElement da valutare</param>
        /// <param name="xPath">query xpath intera, viene normalizzato internamente</param>
        /// <returns>lista di valori recuperati</returns>
        public static IEnumerable<TType> GetValuesFromXElement<TType>(this XElement element, string xPath)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var xpathNormalized = xPath.Normalize(element.Name.LocalName);
            var nameSpaceManager = element.GetNameSpaceManager(ref xpathNormalized);

            if (xpathNormalized.Contains("@"))
            {
                if (xpathNormalized.Contains("/"))
                {
                    var elementXPath = xpathNormalized.Substring(0, xpathNormalized.LastIndexOf('/'));
                    element = element.SelectFromXPath(elementXPath);
                    if (element == null)
                    {
                        return new[] {TypeExtensions.GetValue<TType>(null)};
                    }
                    xpathNormalized = xpathNormalized.Replace(elementXPath, string.Empty);
                }

                if (xpathNormalized.Contains(":"))
                {
                    xpathNormalized = xpathNormalized.Substring(xpathNormalized.IndexOf(':') + 1);
                }

                xpathNormalized = xpathNormalized.Replace("@", string.Empty);
                var attributes = element.Attributes(XName.Get(xpathNormalized, nameSpaceManager.DefaultNamespace));
                    //element.Attribute(XName.Get(xpathNormalized, nameSpaceManager.DefaultNamespace));
                IEnumerable<XAttribute> xAttributes = attributes as XAttribute[] ?? attributes.ToArray();
                if (!xAttributes.Any())
                {
                    //throw new Exception(string.Format("Nessun attributo trovato per l'xpath {0}", xPath));
                    return new[] {TypeExtensions.GetValue<TType>(null)};
                }
                
                var values = xAttributes.Select(attr => attr.Value);
                var result = new Collection<TType>();
                foreach (var value in values)
                {
                    result.Add(value.GetValue<TType>());
                }

                return result;

            }

            var elements = element.XPathSelectElements(xpathNormalized, nameSpaceManager);

            return elements.Select(xElement =>
            {
                if (xElement != null && typeof(TType) == typeof(string) && xElement.HasElements)
                {
                    var xNode = xElement.ToString(SaveOptions.None);
                    if (xNode.StartsWith("<") && xNode.EndsWith(">"))
                    {
                        return xNode.GetValue<TType>();
                    }
                }
                return xElement.Value.GetValue<TType>();
            }).ToList();
        }

        /// <summary>
        /// Recupera XmlNamespaceManager da un XElement
        /// </summary>
        /// <param name="element">XElement da valutare</param>
        /// <param name="xpathNormalized">xPath normalizzato  XPathExtensions.Normalize(xpath)</param>
        /// <returns>Namespace Manager</returns>
        public static XmlNamespaceManager GetNameSpaceManager(this XElement element, ref string xpathNormalized)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var nameSpaceManager = new XmlNamespaceManager(new NameTable());

            var ns = element.Attributes().FirstOrDefault(attr =>
              attr.IsNamespaceDeclaration && !attr.Value.ToLower().Contains("xmlschema"));
            if (ns == null)
            {
                return nameSpaceManager;
            }

            var nameSpaceUri = ns.Value.GetValue<string>();
            var nameSpace = XNamespace.Get(nameSpaceUri);
            var prefix = element.GetPrefixOfNamespace(nameSpace);

            if (new[] { "xsd", "xsi", "xs" }.Contains(prefix))
            {
                return nameSpaceManager;
            }

            if (string.IsNullOrEmpty(prefix))
            {
                prefix = "a";
            }
            
            nameSpaceManager.AddNamespace(prefix, nameSpaceUri);
            if (!xpathNormalized.Contains(":"))
            {
                xpathNormalized = xpathNormalized.AddPrefix(prefix);
            }
            return nameSpaceManager;
        }

        
    }
}