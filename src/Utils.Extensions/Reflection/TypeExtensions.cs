using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils.Extensions.Reflection
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Estesioni sui Tipi di dato Type
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Nome del metodo get Value
        /// </summary>
        public const string GetValueMethodName = "GetValue";
        private static readonly Hashtable DbTypeTable = new Hashtable
                    {
                        {SqlDbType.Bit, typeof (bool)},
                        {SqlDbType.SmallInt, typeof (short)},
                        {SqlDbType.Int, typeof (int)},
                        {SqlDbType.BigInt, typeof (long)},
                        {SqlDbType.Float, typeof (double)},
                        {SqlDbType.Decimal, typeof (decimal)},
                        {SqlDbType.VarChar, typeof (string)},
                        {SqlDbType.DateTime, typeof (DateTime)},
                        {SqlDbType.VarBinary, typeof (byte[])},
                        {SqlDbType.UniqueIdentifier, typeof (Guid)}
                    };

        /// <summary>
        /// Dato un Type ritorna il SqlDbType corrispondente
        /// </summary>
        /// <param name="theType">Type da valutare</param>
        /// <returns>SqlDbType corrispondente</returns>
        public static SqlDbType GetDbType(this Type theType)
        {
            var param = new SqlParameter();
            var tc = TypeDescriptor.GetConverter(param.DbType);
            if (tc.CanConvertFrom(theType))
            {
                var convertFrom = tc.ConvertFrom(theType.Name);
                if (convertFrom != null)
                    param.DbType = (DbType)convertFrom;
            }
            else
            {
                // try to forcefully convert
                try
                {
                    if (theType.IsNullable())
                    {
                        var underlyingType = Nullable.GetUnderlyingType(theType);
                        theType = underlyingType;
                    }
                    var convertFrom = tc.ConvertFrom(theType.Name);
                    if (convertFrom != null)
                        param.DbType = (DbType)convertFrom;
                }
                catch
                {
                    // ignore the exception
                }
            }
            return param.SqlDbType;
        }

        /// <summary>
        /// Degine if a type is Nullable
        /// </summary>
        /// <param name="type">The type parameter</param>
        /// <returns>true if is nullable, false otherwise</returns>
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }
    

        /// <summary>
        /// Converte un SqlDbType in Type
        /// </summary>
        /// <param name="theType">SqlDbType da convertire</param>
        /// <returns>Type corrispondente</returns>
        public static Type ConvertToDbType(this SqlDbType theType)
        {
            Type type;
            try
            {
                type = (Type)DbTypeTable[theType];
            }
            catch
            {
                type = typeof(object);
            }
            return type;
        }

        /// <summary>
        /// Dato un object recupera effettua il cast a TType o converte il valore passato
        /// Gestisce DBNull.Value => null
        /// null => null o Default(TType) in caso di primitive
        /// Tipi Primitivi 
        /// Enum
        /// se value is string and TType:DateTime => Converte in DateTime
        /// Nullable
        /// </summary>
        /// <typeparam name="TType">Tipo di ritorno</typeparam>
        /// <param name="value">Valore da valutare</param>
        /// <returns>valore di tipo TType</returns>
        public static TType GetValue<TType>(this object value)
        {
            var type = typeof(TType);
            if (value == System.DBNull.Value)
            {
                value = null;
            }

            if (value == null)
            {
                if (type.IsPrimitive)
                {
                    return default(TType);
                }

                return (TType)value;
            }

            if (type.IsPrimitive)
            {
                return (TType)Convert.ChangeType(value, type);
            }

            if (type.IsEnum)
            {
                var objectType = value.GetType();

                if (objectType.IsNumeric() || objectType == type)
                {
                    return (TType)value;
                }

                return (TType)Enum.Parse(type, value.ToString());

            }
            if (type.IsDateTime() && value is string)
            {
                return (TType)Convert.ChangeType(DateTime.Parse(value.ToString()), type);
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                MethodInfo methodInfo = typeof(TypeExtensions).GetMethod("GetValue", new[] { typeof(object) });
                MethodInfo generic = methodInfo.MakeGenericMethod(underlyingType);
                return (TType)generic.Invoke(null, new[] { value });
                //return (TType)Convert.ChangeType(value, underlyingType);
            }

            if (type.IsNumeric())
            {
                return (TType)Convert.ChangeType(value, type);
            }

            return (TType)value;
        }

        /// <summary>
        /// indica se un Type è di tipo DateTime
        /// </summary>
        /// <param name="type">Type da valutare</param>
        /// <returns>true se DateTime, false altrimenti</returns>
        public static bool IsDateTime(this Type type)
        {
            if (type == null) return false;
            if (type == typeof(DateTime))
                return true;
            return false;
        }

        /// <summary>
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric.
        /// </summary>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// </remarks>
        public static bool IsNumeric(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return Nullable.GetUnderlyingType(type).IsNumeric();
                    }
                    return false;
            }
            return false;
        }

        /// <summary>
        /// GetValue passando il tipo come argomento del metodo
        /// </summary>
        /// <param name="value">oggetto su cui applicare</param>
        /// <param name="genericType">tipo del generics</param>
        /// <returns>Il valore recuperato</returns>
        public static object GetValue(this object value, Type genericType)
        {
            //if (value == null) throw new ArgumentNullException("value");
            if (genericType == null) throw new ArgumentNullException("genericType");

            return value.InvokeExtensionGenericMethod(
                typeof (TypeExtensions),
                GetValueMethodName,
                new[] {typeof (object)},
                new[] { genericType });
        }

        /// <summary>
        /// Invoca un metodo di un extension method
        /// </summary>
        /// <param name="value">istanza del tipo this dell'extension method</param>
        /// <param name="extensionClass">classe dell'estensione</param>
        /// <param name="methodName">nome del metodo da invocare</param>
        /// <param name="paramsTypes">tipi degli argomenti del metodo per recuperare il metodo corretto</param>
        /// <param name="parameters">parametri da passare per l'invocazione del metodo</param>
        /// <returns>Il valore ritornato dal metodo invocato</returns>
        public static object InvokeExtensionMethod(this object value, Type extensionClass, string methodName,
            Type[] paramsTypes, params object[] parameters)
        {
            var methodInfo = extensionClass.GetMethod(methodName, paramsTypes);
            var methodParameters = new List<object> { value };
            methodParameters.AddRange(parameters);

            return methodInfo.Invoke(null, methodParameters.ToArray());
        }

        /// <summary>
        /// Invoca un metodo di un extension method
        /// </summary>
        /// <param name="value">istanza del tipo this dell'extension method</param>
        /// <param name="extensionClass">classe dell'estensione</param>
        /// <param name="methodName">nome del metodo generics da invocare</param>
        /// <param name="paramsTypes">tipi degli argomenti del metodo estensione per recuperare il metodo corretto</param>
        /// <param name="genericTypes">tipi del generics tra parentesi convesse</param>
        /// <param name="parameters">parametri del genericMethod</param>
        /// <returns></returns>
        public static object InvokeExtensionGenericMethod(this object value, Type extensionClass, string methodName,
            Type[] paramsTypes, Type[] genericTypes, params object[] parameters)
        {
            //if (value == null)
            //{
            //    throw new ArgumentNullException("value");
            //}
            if (extensionClass == null) throw new ArgumentNullException("extensionClass");
            if (methodName == null) throw new ArgumentNullException("methodName");
            if (genericTypes == null) throw new ArgumentNullException("genericTypes");


            var methodInfo = extensionClass.GetMethod(methodName, paramsTypes);
            MethodInfo generic = methodInfo.MakeGenericMethod(genericTypes);
            var genericParameters = new List<object> {value};
            genericParameters.AddRange(parameters);

            return generic.Invoke(null, genericParameters.ToArray());
        }
    }
}