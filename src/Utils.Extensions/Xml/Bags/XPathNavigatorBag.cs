using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Utils.Extensions.Xml.Bags
{
    /// <summary>
    /// Xml Bag che implementa la navigazione  XPathNavigator
    /// </summary>
    internal class XPathNavigatorBag : XmlValidator, IXmlBag
    {
        /// <summary>
        ///  Current XPathNavigator
        /// </summary>
        public XPathNavigator Navigator { get; set; }

        /// <summary>
        /// Initialize a XmlElementBag froma an xml String for XPathNavigator Navigation Strategy
        /// </summary>
        /// <param name="xmlString">xml string</param>
        /// <param name="editable"></param>
        /// <exception cref="ArgumentNullException">xmlString</exception>
        public XPathNavigatorBag(string xmlString, bool editable)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                throw new ArgumentNullException(nameof(xmlString));
            }
            if (editable)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);
                Navigator = xmlDoc.CreateNavigator();
            }
            else
            {
                var doc = XDocument.Parse(xmlString);
                Navigator = doc.CreateNavigator();
            }
        }

        /// <summary>
        /// Recupera tutti i valori per un determinato xpath
        /// </summary>
        /// <typeparam name="TType">Tipo di ritorno richiesto</typeparam>
        /// <param name="xPath">XPath da valutare</param>
        /// <returns>La lista dei valori convertiti secondo il parametro TType</returns>
        public IEnumerable<TType> GetValuesFromXPath<TType>(string xPath)
        {
            return Navigator.GetValuesFromXPath<TType>(xPath);
        }

        /// <summary>
        /// Recupera il valore di un determinato xpath
        /// </summary>
        /// <typeparam name="TType">Tipo di ritorno richiesto</typeparam>
        /// <param name="xPath">XPath da valutare</param>
        /// <returns>Il valore convertito</returns>
        public TType GetValueFromXPath<TType>(string xPath)
        {
            return Navigator.GetValueFromXPath<TType>(xPath);
        }

        /// <summary>
        /// Effettua l'update di un xpath
        /// </summary>
        /// <param name="xPath">xpath da aggiornare</param>
        /// <param name="value">Valore da aggioranre</param>
        public void Update(string xPath, string value)
        {
            Navigator.Update(xPath, value);
        }

        /// <summary>
        /// Restituisce l'xml in formato stringa
        /// </summary>
        /// <returns></returns>
        public string GetRawXml()
        {
            return Navigator.GetRawXml();
        }

        /// <summary>
        /// Effettua la validazione di un xml secondo lo schema xsd passato
        /// </summary>
        /// <param name="xmlSchema">Schema Xsd In formato stringa</param>
        public void Validate(string xmlSchema)
        {
            Validate(GetRawXml(), xmlSchema);
        }
    }
}