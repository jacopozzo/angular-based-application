namespace Utils.Extensions.Xml
{
    using System.Linq;

    /// <summary>
    /// Risultato della validazione per non sollevare l'eccezione
    /// </summary>
    public class XsdValidationResult
    {
        private System.Collections.Generic.List<XsdValidationInfo> _validationErrors;

        /// <summary>
        /// Ottiene il risultato della validazione
        /// </summary>
        public bool IsNotValid => _validationErrors.Any();

        /// <summary>
        /// Ottiene o imposta il set di errori di validazione
        /// </summary>
        public System.Collections.Generic.IEnumerable<XsdValidationInfo> ValidationErrors
        {
            get { return _validationErrors; }
            set { _validationErrors = (System.Collections.Generic.List<XsdValidationInfo>) value; }
        }


        /// <summary>
        /// Inizializza un'istanza della classe
        /// </summary>
        public XsdValidationResult()
        {
            _validationErrors = new System.Collections.Generic.List<XsdValidationInfo>();
        }

        /// <summary>
        /// Aggiunge un errore agli errori di validazione
        /// </summary>
        /// <param name="error">errore da aggiungere</param>
        public void Add(XsdValidationInfo error)
        {
            _validationErrors.Add(error);
        }
    }
}