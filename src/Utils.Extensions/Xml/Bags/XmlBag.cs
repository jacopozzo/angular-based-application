using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Utils.Extensions.Xml.Bags
{
    using System.Xml.Schema;

    /// <summary>
    /// Classe d'appoggio per la navigazione xml
    /// </summary>
    [Obsolete("Use XmlBagFactory.Create(string xmlString, NavigationStrategy navigationStrategy ) instead of this class")]
    public class XmlBag : IXmlBag
    {
        private readonly IXmlBag _currentXmlBag;

        /// <summary>
        /// Costruisce un'xml bag che valuta gli xPath a seconda della strategia impostata
        /// </summary>
        /// <param name="xmlString">Xml da valutare</param>
        /// <param name="navigationStrategy">Strategia, default:XElementNavigator</param>
        public XmlBag(string xmlString, NavigationStrategy navigationStrategy = NavigationStrategy.XElementNavigator)
        {
            _currentXmlBag = XmlBagFactory.Create(xmlString, navigationStrategy);
        }


        /// <summary>
        /// Gets the current Navigator for the xmlBag
        /// </summary>
        public XPathNavigator Navigator => XmlBagFactory.GetNavigator(_currentXmlBag);

        /// <summary>
        /// Gets the current XElement for the xmlBag
        /// </summary>
        public XElement RootElement => XmlBagFactory.GetXElement(_currentXmlBag);

        /// <summary>
        /// Recupera tutti i valori di per un determinato xPath
        /// </summary>
        /// <param name="xPath">XPath da rilevare</param>
        /// <typeparam name="TType">Tipo di ritorno dell'enumerazione</typeparam>
        /// <returns>i valori risultanti dalla query xPath</returns>
        public IEnumerable<TType> GetValuesFromXPath<TType>(string xPath)
        {
            if (!xPath.Contains("/@") && !xPath.Contains("@"))
            {
                return _currentXmlBag.GetValuesFromXPath<TType>(xPath);
            }
            var navigator = XmlBagFactory.GetNavigator(_currentXmlBag);
            return navigator.GetValuesFromXPath<TType>(xPath);
        }

        /// <summary>
        /// Recupera un valore per un determinato xpath
        /// </summary>
        /// <param name="xPath">XPath da valutare</param>
        /// <typeparam name="TType">Tipo di ritorno</typeparam>
        /// <returns>il valore risultante dalla query xPath</returns>
        public TType GetValueFromXPath<TType>(string xPath)
        {
            if (!xPath.Contains("/@") && !xPath.Contains("@"))
            {
                return _currentXmlBag.GetValueFromXPath<TType>(xPath);
            }
            var navigator = XmlBagFactory.GetNavigator(_currentXmlBag);
            return navigator.GetValueFromXPath<TType>(xPath);
        }

        /// <summary>
        /// Aggiorna il valore di un xpath all'interno di un xml e restituisce l'xml modificato
        /// </summary>
        /// <param name="xPath">xpath dell'elemento da modificare</param>
        /// <param name="value">valore da sostituire</param>
        public virtual void Update(string xPath, string value)
        {
            _currentXmlBag.Update(xPath, value);
        }

        /// <summary>
        /// Recupera l'xml in formato stringa
        /// </summary>
        /// <returns>xml in formato stringa</returns>
        public virtual string GetRawXml()
        {
            return _currentXmlBag.GetRawXml();
        }

        /// <summary>
        /// Validate an Xml According to the passed schema
        /// </summary>
        /// <param name="xmlSchema">XsdSchema</param>
        public virtual void Validate(string xmlSchema)
        {
            var xml = _currentXmlBag.GetRawXml();
            if (string.IsNullOrEmpty(xml))
            {
                throw new InvalidOperationException("Non è possibile validare un xml vuoto");
            }

            if (string.IsNullOrEmpty(xmlSchema))
            {
                throw new ArgumentNullException(nameof(xmlSchema));
            }

            var schemas = new System.Xml.Schema.XmlSchemaSet();
            schemas.Add(string.Empty, System.Xml.XmlReader.Create(new System.IO.StringReader(xmlSchema)));

            var doc = XDocument.Parse(xml);

            var validationErrors = new List<XsdValidationInfo>();

            var errors = false;
            doc.Validate(
                schemas,
                (o, e) =>
                {
                    validationErrors.Add(new XsdValidationInfo(e.Message, e.Exception));
                    errors = true;
                });

            if (errors)
            {
                throw new XsdValidationException(validationErrors);
            }
        }
    }
}