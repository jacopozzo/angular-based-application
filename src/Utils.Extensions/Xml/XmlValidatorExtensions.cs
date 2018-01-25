namespace Utils.Extensions.Xml
{
    using System;
    using System.Xml.Linq;
    using System.Xml.Schema;

    /// <summary>
    /// Extension methods per la validazione dell'xml
    /// </summary>
    public static class XmlValidatorExtensions
    {
        /// <summary>
        /// Validate an Xml According to the schema
        /// </summary>
        /// <param name="xml">Xml da validare</param>
        /// <param name="xmlSchema">XsdSchema</param>
        public static void Validate(this string xml, string xmlSchema)
        {

            if (string.IsNullOrEmpty(xml))
            {
                throw new InvalidOperationException("Non è possibile validare un xml vuoto");
            }

            if (string.IsNullOrEmpty(xmlSchema))
            {
                throw new ArgumentNullException(nameof(xmlSchema));
            }

            var schemas = new XmlSchemaSet();
            schemas.Add(string.Empty, System.Xml.XmlReader.Create(new System.IO.StringReader(xmlSchema)));

            var doc = XDocument.Parse(xml);
            doc.Validate(schemas);
        }

        /// <summary>
        /// Validate an Xml According to the schema
        /// </summary>
        /// <param name="xml">Xml da validare</param>
        /// <param name="xmlSchemas">schem set per la validazione</param>
        public static void Validate(this XDocument xml, XmlSchemaSet xmlSchemas)
        {
            if (xml == null)
            {
                throw new InvalidOperationException("Non è possibile validare un xml vuoto");
            }
            if (xmlSchemas == null)
            {
                throw new ArgumentNullException(nameof(xmlSchemas));
            }

            var result = xml.ValidateXml(xmlSchemas);

            if (result.IsNotValid)
            {
                throw new XsdValidationException(result.ValidationErrors);
            }
        }

        /// <summary>
        /// Effettua la validazio dell'xDocument dato un set di schemas
        /// </summary>
        /// <param name="xml">Xml da validare</param>
        /// <param name="xmlSchemas">Xsd Schema</param>
        /// <returns>Validation Result</returns>
        /// <exception cref="InvalidOperationException">Xml null</exception>
        /// <exception cref="ArgumentNullException">schema null</exception>
        public static XsdValidationResult ValidateXml(this XDocument xml, XmlSchemaSet xmlSchemas)
        {
            if (xml == null)
            {
                throw new InvalidOperationException("Non è possibile validare un xml vuoto");
            }
            if (xmlSchemas == null)
            {
                throw new ArgumentNullException(nameof(xmlSchemas));
            }

            var result = new XsdValidationResult();

            xml.Validate(
                xmlSchemas,
                (o, e) =>
                {
                    result.Add(new XsdValidationInfo(e.Message, e.Exception));
                });

            return result;
        }
    }
}