using System;
using System.Collections.Generic;

namespace Utils.Extensions.Compare
{
    /// <summary>
    /// Comparatore generico per qli equality comparaer
    /// </summary>
    /// <typeparam name="T">Tipo del comparatore</typeparam>
    public class GenericCompare<T> : IEqualityComparer<T> where T : class
    {
        private Func<T, object> Expr
        {
            get;
        }

        /// <summary>
        /// Expression for object
        /// </summary>
        /// <param name="expr">Funzione per definire la condizione di uguaglianza</param>
        public GenericCompare(Func<T, object> expr)
        {
            Expr = expr;
        }

        /// <summary>
        /// definisce se due oggetti di tipo T sono ugali
        /// </summary>
        /// <param name="x">Tipo da confrontare</param>
        /// <param name="y">Tipo da confrontare</param>
        /// <returns>True se sono uguali, false altrimenti</returns>

        public bool Equals(T x, T y)
        {
            var first = Expr.Invoke(x);
            var sec = Expr.Invoke(y);
            return first != null && first.Equals(sec);
        }

        /// <summary>
        /// Restituisce l'hashcode dell'oggetto
        /// </summary>
        /// <param name="obj">Oggetto da trasformare</param>
        /// <returns>hash code dell'oggetto</returns>
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
