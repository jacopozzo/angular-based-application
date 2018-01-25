using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Utils.Extensions.Basic
{
  /// <summary>
  /// Raccolta estesioni sulle stringhe
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Split a string into an IEnumerable
    /// </summary>
    /// <param name="stringCollection"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitMe(this string stringCollection, string separator)
    {
      if (stringCollection == null)
        throw new ArgumentNullException(nameof(stringCollection));
      if (separator == null)
        throw new ArgumentNullException(nameof(separator));

      return stringCollection.Split(separator.ToCharArray());
    }
    /// <summary>
    /// Compute an hash froma a string value
    /// </summary>
    /// <param name="value">string value</param>
    /// <param name="encoding">Encoding, ASCII by default</param>
    /// <param name="algorithm">HashAlgorithm, SHA1 by default</param>
    /// <returns>An hashed string</returns>
    public static object ComputeHash(this string value, Encoding encoding = null, HashAlgorithm algorithm = null)
    {
      if (encoding == null)
        encoding = new ASCIIEncoding();

      return encoding.GetString(value.HashBytes(encoding, algorithm));
    }

    /// <summary>
    /// Compute an hash froma a string value
    /// </summary>
    /// <param name="value">string value</param>
    /// <param name="encoding">Encoding, ASCII by default</param>
    /// <param name="algorithm">HashAlgorithm, SHA1 by default</param>
    /// <returns>An hashed string</returns>
    public static byte[] HashBytes(this string value, Encoding encoding = null, HashAlgorithm algorithm = null)
    {
      if (string.IsNullOrWhiteSpace(value))
        return null;
      if (encoding == null)
        encoding = new ASCIIEncoding();
      var stringToBytes = encoding.GetBytes(value);

      if (algorithm == null)
        algorithm = new SHA1CryptoServiceProvider();
      var encoded = algorithm.ComputeHash(new MemoryStream(stringToBytes));
      return encoded;
    }
  }
}
