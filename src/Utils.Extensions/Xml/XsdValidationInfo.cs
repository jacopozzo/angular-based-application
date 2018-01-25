using System.Text;
using System.Xml.Schema;

namespace Utils.Extensions.Xml
{
    /// <summary>
    ///     Info di validazione xsd
    /// </summary>
    public class XsdValidationInfo
    {
        /// <summary>
        /// Create an instance of an xsdValidation Info
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public XsdValidationInfo(string message, XmlSchemaException exception)
        {
            Message = message;
            Exception = exception;
        }

        /// <summary>
        ///     Messaggio dell'errore di validazione
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Eccezione di validazione xsd
        /// </summary>
        public System.Xml.Schema.XmlSchemaException Exception { get; set; }

        /// <summary>
        ///     Serializzazione in stringa dell'oggetto
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormat("Message {0}\n", Message);
            //sb.AppendFormat("Exception {0}\n", this.Exception.Stringify());
            return sb.ToString();
        }
    }
}
