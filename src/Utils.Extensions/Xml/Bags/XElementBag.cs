using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Utils.Extensions.Xml.Bags
{
    /// <summary>
    /// Xml Bag che implementa la navigazione XElement
    /// </summary>
    internal class XElementBag : IXmlBag
    {

        /// <summary>
        /// Current XElement
        /// </summary>
        public XElement RootElement
        {
            get;
            set;
        }

        /// <summary>
        /// Initialize a XmlElementBag froma an xml String for XElement Navigation Strategy
        /// </summary>
        /// <param name="xmlString">xml string</param>
        /// <exception cref="ArgumentNullException">xmlString</exception>
        public XElementBag(string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                throw new ArgumentNullException(nameof(xmlString));
            }

            RootElement = XElement.Parse(xmlString);
        }

        public IEnumerable<TType> GetValuesFromXPath<TType>(string xPath)
        {
            return RootElement.GetValuesFromXElement<TType>(xPath);
        }

        public TType GetValueFromXPath<TType>(string xPath)
        {
            return RootElement.GetValueFromXElement<TType>(xPath);
        }

        public void Update(string xPath, string value)
        {
            RootElement.Update(xPath, value);
        }

        public string GetRawXml()
        {
            return RootElement.GetRawXml();
        }

        public void Validate(string xmlSchema)
        {
            var xml = GetRawXml();
            xml.Validate(xmlSchema);
        }
    }
}