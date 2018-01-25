using System.IO;
using System.ServiceModel.Channels;
using System.Xml;

namespace Utils.Extensions.ServiceModel
{
    /// <summary>
    /// Estesioni su Message di ServiceModel [WCF]
    /// </summary>
    public static class ExtensionsToMessage
    {
        #region structure extensions

        /// <summary>
        /// Converts the raw transport message to a string representation using 
        /// a temporary buffer.
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <returns></returns>
        public static string ToStringBuffered(this Message message)
        {
            var buffer = new MemoryStream();
            var messageBuffer = message.CreateBufferedCopy(int.MaxValue);
            var temporary = messageBuffer.CreateMessage();
            var writer = XmlWriter.Create(buffer);
            temporary.WriteMessage(writer);
            writer.Flush();
            buffer.Seek(0, SeekOrigin.Begin);

            return new StreamReader(buffer).ReadToEnd();
        }

        /// <summary>
        /// Gets the content of all the message in each part.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Stream GetMessageContent(this Message message)
        {
            var buffer = new MemoryStream();
            var messageBuffer = message.CreateBufferedCopy(int.MaxValue);
            var temporary = messageBuffer.CreateMessage();
            var writer = XmlWriter.Create(buffer);
            temporary.WriteMessage(writer);
            writer.Flush();
            //var reader = Encoding.UTF8.GetBytes(message.ToString());
            //var buffer = new MemoryStream(reader, false);
            buffer.Seek(0, SeekOrigin.Begin);

            return buffer;
        }

        /// <summary>
        /// Gets the content of the body.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static Stream GetBodyContent(this Message message)
        {
            var reader = message.GetReaderAtBodyContents();
            var buffer = new MemoryStream();

            var writer = new StreamWriter(buffer);
            writer.Write(reader.ReadOuterXml());
            writer.Flush();
            buffer.Seek(0, SeekOrigin.Begin);

            return buffer;
        }

        #endregion
    }
}