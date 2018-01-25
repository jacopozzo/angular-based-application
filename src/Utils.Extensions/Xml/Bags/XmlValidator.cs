namespace Utils.Extensions.Xml.Bags
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using System.Xml.Schema;

    internal class XmlValidator
    {
        /// <summary>
        /// Validate an Xml According to the passed schema
        /// </summary>
        /// <param name="xml">Xml da validare</param>
        /// <param name="xmlSchema">XsdSchema</param>
        protected virtual void Validate(string xml, string xmlSchema)
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

            Validate(doc, schemas);
        }

        /// <summary>
        /// Validate an Xml According to the schema
        /// </summary>
        /// <param name="xml">Xml da validare</param>
        /// <param name="xmlSchemas">schem set per la validazione</param>
        protected virtual void Validate(XDocument xml, XmlSchemaSet xmlSchemas)
        {
            if (xml == null)
            {
                throw new InvalidOperationException("Non è possibile validare un xml vuoto");
            }
            if (xmlSchemas == null)
            {
                throw new ArgumentNullException(nameof(xmlSchemas));
            }
            
            var validationErrors = new List<XsdValidationInfo>();

            var errors = false;
            xml.Validate(
                xmlSchemas,
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