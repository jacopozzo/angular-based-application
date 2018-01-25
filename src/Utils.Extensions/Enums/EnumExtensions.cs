using System;
using System.Collections.Generic;

namespace Utils.Extensions.Enums
{
    /// <summary>
    /// Estesioni sul tipo di dati Enum
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Per l'enum specificato costruisce un dizionario Valore, Nome di tutti i valori dell'enum
        /// </summary>
        /// <typeparam name="TEnum">Enum da valutare</typeparam>
        /// <returns>Tutti i valori dell'enum. Es  enum ErrorLevel{Debug = 1, Info = 2, Error = 3} => {{1, Debug}, {2,Info}, {3, Error}}</returns>
        /// <exception cref="ArgumentException">Enumeration type is expected.</exception>
        public static IDictionary<object, string> GetAll<TEnum>() where TEnum : struct
        {
            var enumerationType = typeof(TEnum);

            if (!enumerationType.IsEnum)
            {
                throw new ArgumentException("Enumeration type is expected.");
            }

            var dictionary = new Dictionary<object, string>();

            foreach (int value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                dictionary.Add(value, name);
            }

            return dictionary;
        }
    }
}
