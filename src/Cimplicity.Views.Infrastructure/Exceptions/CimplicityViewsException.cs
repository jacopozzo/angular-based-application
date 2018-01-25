using System;
using System.Runtime.Serialization;

namespace Cimplicity.Views.Infrastructure.Exceptions
{
    [Serializable]
    public class CimplicityViewsException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CimplicityViewsException()
        {
        }

        public CimplicityViewsException(string message) : base(message)
        {
        }

        public CimplicityViewsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CimplicityViewsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}