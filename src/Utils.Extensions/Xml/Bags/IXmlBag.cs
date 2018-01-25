using System.Collections.Generic;

namespace Utils.Extensions.Xml.Bags
{
    /// <summary>
    /// Utility per recupero valori dato un xPath
    /// </summary>
    public interface IXmlBag
    {
        /// <summary>
        /// Recupera tutti i valori dato l'xpath 
        /// </summary>
        /// <typeparam name="TType">Tipo di ritorno dei valori</typeparam>
        /// <param name="xPath">xPath da valutare</param>
        /// <returns>Lista dei valori tipizzata secondo il generics</returns>
        IEnumerable<TType> GetValuesFromXPath<TType>(string xPath);

        /// <summary>
        /// Recupera il valore dato un xPath
        /// </summary>
        /// <typeparam name="TType">Tipo di ritorno</typeparam>
        /// <param name="xPath">xpath da valutare</param>
        /// <returns>Valore di tipo TType recuperato dall'xml</returns>
        TType GetValueFromXPath<TType>(string xPath);

        /// <summary>
        /// Aggiorna il valore di un xpath all'interno di un xml e restituisce l'xml modificato
        /// </summary>
        /// <param name="xPath">xpath dell'elemento da modificare</param>
        /// <param name="value">valore da sostituire</param>
        void Update(string xPath, string value);

        /// <summary>
        /// Recupera l'xml in formato stringa
        /// </summary>
        /// <returns>xml in formato stringa</returns>
        string GetRawXml();

        /// <summary>
        /// Validate an Xml According to the passed schema
        /// </summary>
        /// <param name="xmlSchema">XsdSchema</param>
        void Validate(string xmlSchema);
    }
}