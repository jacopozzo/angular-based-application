using System;
using System.Collections.Generic;

namespace Utils.Extensions.Memoization
{
    /// <summary>
    /// Extension methods per la memoization dei metodi
    /// </summary>
    public static class MemoizationExtensions
    {
        /// <summary>
        /// Effettua la memoization di una func
        /// </summary>
        /// <typeparam name="A">Tipo di ritorno</typeparam>
        /// <typeparam name="R">Parametro del metodo</typeparam>
        /// <param name="f">Func</param>
        /// <returns>Il metodo memoized</returns>
        public static Func<A, R> Memoize<A, R>(this Func<A, R> f)
        {
            var d = new Dictionary<A, R>();
            return a =>
            {
                R r;
                lock (d)
                {
                    if (!d.TryGetValue(a, out r))
                    {
                        r = f(a);
                        d.Add(a, r);
                    }
                }
                return r;
            };
        }


        
    }
}
