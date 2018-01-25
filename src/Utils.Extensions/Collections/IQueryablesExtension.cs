using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Utils.Extensions.Collections
{
    /// <summary>
    /// Extension methods su per IQueryable 
    /// </summary>
    public static class IQueryablesExtension
    {
        /// <summary>
        /// Fa il performe di un queryable tramite reflection
        /// Serve per la deserializzazione delle query sql in queryable
        /// </summary>
        /// <param name="source">query</param>
        /// <param name="property">proprietà sulla quale fare il confronto</param>
        /// <param name="predicateType">Tipo di confronto da fare</param>
        /// <param name="value">valore dell'espressione di confronto</param>
        /// <returns>La query aggiornata</returns>
        public static IQueryable Where(this IQueryable source, string property, PredicateType predicateType, object value)
        {
            var type = source.GetType();
            ParameterExpression param = Expression.Parameter(source.GetType(), "specificQueryable");
            Expression sourceExpression = Expression.Constant(source);
            Expression propertyExpression = Expression.Constant(property);
            Expression predicateTypeExpression = Expression.Constant(predicateType);
            Expression valueExpression = Expression.Convert(Expression.Constant(value), typeof(object));
            var methodInfo = typeof(IQueryablesExtension).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .FirstOrDefault(mi => mi.Name == "Where" && mi.IsGenericMethod)?.MakeGenericMethod(source.ElementType);
            
            var methodExpression = Expression.Call(null, methodInfo, sourceExpression, propertyExpression, predicateTypeExpression, valueExpression);
            //var methodException = Expression.Call(null, "Wherea", new Type[] { source.ElementType }, sourceExpression, propertyExpression, predicateTypeExpression, valueExpression);
            return Expression.Lambda<Func<IQueryable>>(methodExpression).Compile().Invoke();
        }

        /// <summary>
        /// Fa il performe di un queryable tramite reflection
        /// Serve per la deserializzazione delle query sql in queryable
        /// </summary>
        /// <param name="source">query</param>
        /// <param name="property">proprietà sulla quale fare il confronto</param>
        /// <param name="predicateType">Tipo di confronto da fare</param>
        /// <param name="value">valore dell'espressione di confronto</param>
        /// <returns>La query aggiornata</returns>
        public static IQueryable<TType> Where<TType>(this IQueryable<TType> source, string property, PredicateType predicateType, object value)
        {
            source.Cast<object>();
            ParameterExpression param = Expression.Parameter(typeof(TType), "parameter");

            Expression propertyExpression = Expression.Property(param, property);
            Expression valueExpression = Expression.Constant(value);
            if (propertyExpression.Type.IsGenericType && propertyExpression.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                valueExpression = Expression.Convert(valueExpression, propertyExpression.Type);
            }
            BinaryExpression conditionExpresion = null;
            switch (predicateType)
            {
                case PredicateType.Equal:
                    conditionExpresion = Expression.Equal(propertyExpression, valueExpression);
                    break;
                case PredicateType.GreaterThan:
                    conditionExpresion = Expression.GreaterThan(propertyExpression, valueExpression);
                    break;
                case PredicateType.GreaterThanOrEqual:
                    conditionExpresion = Expression.GreaterThanOrEqual(propertyExpression, valueExpression);
                    break;
                case PredicateType.LessThan:
                    conditionExpresion = Expression.LessThan(propertyExpression, valueExpression);
                    break;
                case PredicateType.LessThanOrEqual:
                    conditionExpresion = Expression.LessThanOrEqual(propertyExpression, valueExpression);
                    break;
                case PredicateType.NotEqual:
                    conditionExpresion = Expression.NotEqual(propertyExpression, valueExpression);
                    break;
                default:
                    break;
            }

            var predicate = Expression.Lambda<Func<TType, bool>>(conditionExpresion, param);

            return source.Where(predicate);
        }
    }
}
