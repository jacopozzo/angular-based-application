namespace Utils.Extensions.Xml
{
    using CollectionsExtension = Collections.CollectionsExtension;
    using Enumerable = System.Linq.Enumerable;

    /// <summary>
    ///     Eccezione di validazione xsd
    /// </summary>
    [System.Serializable]
    public class XsdValidationException : System.Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        private const string Format = "Errore di validazione xsd : {0}";

        internal XsdValidationException(System.Collections.Generic.IEnumerable<XsdValidationInfo> validationInfos)
            : base(string.Format(Format, CollectionsExtension.Stringify(Enumerable.Select(validationInfos, info => info.ToString()))))
        {
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected XsdValidationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}