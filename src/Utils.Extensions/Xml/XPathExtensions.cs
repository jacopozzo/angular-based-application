using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Utils.Extensions.Collections;

namespace Utils.Extensions.Xml
{
    /// <summary>
    /// Estesioni sulle stringhe per la manipolazione degli xPath
    /// </summary>
    public static class XPathExtensions
    {

        /// <summary>
        /// Add xpath function to the endo of xPath
        /// </summary>
        /// <param name="xPath">xPath a cui aggiungere la funzione</param>
        /// <param name="xPathFunction">funziona da aggiungere in coda all'xpath</param>
        /// <returns>l'xPath con la funzione aggiunta</returns>
        public static string AddFunction(this string xPath, string xPathFunction)
        {
            if (string.IsNullOrEmpty(xPath))
            {
                throw new ArgumentNullException(nameof(xPath));
            }
            if (string.IsNullOrEmpty(xPathFunction))
            {
                throw new ArgumentNullException(nameof(xPathFunction));
            }

            if (xPath.EndsWith(xPathFunction))
            {
                return xPath;
            }

            return $"{xPath}/{xPathFunction}";
        }

        /// <summary>
        /// Add text() function to the endo of xPath
        /// </summary>
        /// <param name="xPath">xPath a cui aggiungere la funzione</param>
        /// <returns>l'xPath con la funzione aggiunta</returns>
        public static string AddText(this string xPath)
        {
            return xPath.AddFunction("text()");
        }

        /// <summary>
        /// Effettua una normalizzazione di un xpath definito con parentesi [] per gli indici e text() in fondo all'xpath
        /// Trasforma un xPath specifico //root/books/book[1]/title/text() in //root/books/book/title
        /// </summary>
        /// <param name="xPath">xPath</param>
        /// <returns>xPath normalizzato</returns>
        public static string NormalizeXPath(this string xPath)
        {
            if (string.IsNullOrEmpty(xPath))
            {
                return string.Empty;
            }

            var temp = xPath.Replace("/text()", string.Empty);
            if (temp.EndsWith("]"))
            {
                return temp;
            }
            var lastIndexOfPath = temp.LastIndexOf('/');
            return lastIndexOfPath > 0 ? temp.Substring(0, lastIndexOfPath) : temp;
        }

        /// <summary>
        /// Elimina una funzione dall'xPath per poter effettuare operazioni di pulizia o per gestire le funzioni XPath 2.0
        /// </summary>                                                                                                  
        /// <param name="xpath">XPath in formato stringa</param>
        /// <param name="openToken">Token di inizio funzione</param>
        /// <param name="closeToken">Token di fine da eliminare</param>
        /// <returns>XPath epurato della funzione</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string NormalizeXPathFunction(this string xpath, string openToken, string closeToken)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            if (string.IsNullOrEmpty(openToken))
            {
                throw new ArgumentNullException(openToken);
            }

            if (string.IsNullOrEmpty(closeToken))
            {
                throw new ArgumentNullException(openToken);
            }

            var startIndex = xpath.IndexOf(openToken) + openToken.Length;
            var length = xpath.Length - openToken.Length - closeToken.Length;
            return xpath.Substring(startIndex, length);
        }

        /// <summary>
        /// Aggiunge il prefisso ad una stringa xPath
        /// </summary>
        /// <param name="xpath">xPath da modificare</param>
        /// <param name="prefix">prefisso</param>
        /// <returns>Un xpath con i prefissi su ogni nodo</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string AddPrefix(this string xpath, string prefix)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            var prefixed = xpath;
            var elements = new List<string>();
            var startSlash = string.Empty;
            if (prefixed.Contains("//"))
            {
                prefixed = prefixed.Replace("//", "/");
                startSlash = "//";
            }
            if (prefixed.Contains("/"))
            {
                var splitted = prefixed.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                elements.AddRange(splitted.Select(el => $"{prefix}:{el}"));
            }
            else
            {
                elements.Add($"{prefix}:{prefixed}");
            }

            return $"{startSlash}{elements.Stringify("/")}";
        }

        /// <summary>
        /// Definisce se un'espressione xpath ritorna un booleano
        /// </summary>
        /// <param name="xpath">espressione xpath</param>
        /// <returns>true se è un'espressione booleana, false altrimenti</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsBooleanXPath(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            var expr = XPathExpression.Compile(xpath);
            return expr.ReturnType == XPathResultType.Boolean;
        }


        /// <summary>
        /// Definisce se un xpath contiene funzioni stringa
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsStringFunction(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            return xpath.Contains("substring(") || xpath.Contains("string-length(") || xpath.Contains("concat(");
        }

        /// <summary>
        /// Definisce se l'xpath contiene funzioni sulle collection
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsCollectionFunction(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }
            return xpath.Contains("distinct-values(") || xpath.Contains("string-join");
        }

        /// <summary>
        /// Definisce se un xpath deve essere valutato tramite Evaluate
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns>true se contiene funzioni booleane, Stringa o Collection</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsEvaluable(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            return
              xpath.IsBooleanXPath()
              || xpath.IsStringFunction()
              || xpath.IsCollectionFunction()
              || xpath.IsSubstringMatchFunction()
              || xpath.IsExpression();
        }

        /// <summary>
        /// Define if a xpath is an Expression
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsExpression(this string xpath)
        {
            if (xpath == null)
            {
                throw new ArgumentNullException(nameof(xpath));
            }

            return xpath.Contains("local-name")
                || xpath.Contains("namespace-uri");
        }

        /// <summary>
        /// Definisce se xPath ha come funzione terminale un string.join funzione XPath 2.0
        /// </summary>
        /// <param name="xpath">XPath in formato stringa</param>
        /// <returns>true se xpath inzia con 'string-join(', false altrimenti</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsStringJoinFunction(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }
            return xpath.StartsWith("string-join(");
        }

        /// <summary>
        /// Normalizza uno string join di XPath 2.0 da gestire nell'evaluate la lista in ritorno e il join successivo
        /// Funziona solo se lo string-join è agli estremi
        /// </summary>
        /// <param name="xpath">XPath in formato stringa</param>
        /// <param name="delimiter"></param>
        /// <returns>XPath epurato della funzione string-join, gestita in GetValue di XElement</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string NormalizeStringJoin(this string xpath, out string delimiter)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            delimiter = ";";

            if (!xpath.IsStringJoinFunction())
            {
                return xpath;
            }
            var openToken = "string-join(";
            var closeToken = ",',')";


            var closeTagStartIndex = xpath.IndexOf(closeToken);
            var closeTag = xpath.Substring(closeTagStartIndex);
            delimiter = closeTag.Substring(closeTag.IndexOf(",'") + 2, closeTag.IndexOf("')") - 2);
            return xpath.NormalizeXPathFunction(openToken, closeToken);
        }

        /// <summary>
        /// Definisce se xPath ha come funzione terminale un substring-match custom function
        /// </summary>
        /// <param name="xpath">XPath in formato stringa</param>
        /// <returns>true se xpath inzia con 'substring-match(', false altrimenti</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsSubstringMatchFunction(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            return xpath.StartsWith("substring-match(");
        }

        /// <summary>
        /// Define if an xpath is a count function
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static bool IsCountFunction(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                return false;
            }

            return xpath.StartsWith("count(");
        }

        /// <summary>
        /// Normalize a count function for evaluation
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="startToken"></param>
        /// <param name="endToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string NormalizeCountFunction(this string xpath, out string startToken, out string endToken)
        {
            startToken = string.Empty;
            endToken = string.Empty;
            if (!xpath.IsCountFunction())
            {
                throw new ArgumentException($"L'XPath {xpath} non è un'espressione di tipo CountFunction");
            }

            startToken = "count(";
            endToken = xpath.Substring(xpath.IndexOf(")"));

            string innerToken = xpath.NormalizeXPathFunction(startToken, endToken);
            return innerToken;

        }

        /// <summary>
        /// Normalizza uno substring-match funzione custom da gestire nell'evaluate la lista in ritorno e il join successivo
        /// Funziona solo se lo substring-match è agli estremi
        /// </summary>
        /// <param name="xpath">XPath in formato stringa</param>
        /// <param name="startToken"></param>
        /// <param name="endToken"></param>
        /// <returns>XPath epurato della funzione substring-match, gestita in GetValue di XElement</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string NormalizeSubstringMatchFunction(this string xpath, out string startToken, out string endToken)
        {
            startToken = string.Empty;
            endToken = string.Empty;

            if (!xpath.IsSubstringMatchFunction())
            {
                return xpath;
            }

            var openToken = "substring-match(";
            var closeToken = ")";

            var normalized = xpath.NormalizeXPathFunction(openToken, closeToken);

            var arguments = normalized.Split(',');
            if (arguments.Length < 2)
            {
                throw new IndexOutOfRangeException("substring-match function must have 2 arguments");
            }

            var pattern = arguments[1].Substring(1, arguments[1].Length - 2);

            if (pattern.Contains("|"))
            {
                var args = pattern.Split('|');
                startToken = args[0];
                endToken = args[1];
            }
            else
            {
                startToken = pattern;
            }

            return arguments[0];
        }

        /// <summary>
        /// Definisce se un xpath contiene una funzione distinct values funzione XPath 2.0
        /// Funziona solo se lo string-join è agli estremi
        /// </summary>
        /// <param name="xpath">XPath in formato stringa</param>
        /// <returns>true se xpath inzia con 'distinct-values(', false altrimenti</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsDistinctValuesFunction(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            return xpath.StartsWith("distinct-values(");
        }

        /// <summary>
        /// Elimina la funzione distinct-values dall'xpath per poterla gestire lato codice in quanto XPath 2.0 non è supportato
        /// Vale solo nel caso in cui la funzione xpath sia agli estremi
        /// </summary>
        /// <param name="xpath">XPath in formato stringa</param>
        /// <returns>XPath epurato della funzione</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string NormalizeDistinctValues(this string xpath)
        {
            if (string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(xpath);
            }

            if (!xpath.IsDistinctValuesFunction())
            {
                return xpath;
            }
            var openToken = "distinct-values(";
            var closeToken = ")";

            return xpath.NormalizeXPathFunction(openToken, closeToken);
        }

        /// <summary>
        /// Normalizza un'espressione xpath per poter essere valutata su un XElement
        /// Elimina l'elemento radice e la funzione text()
        /// </summary>
        /// <param name="xPath">xpath da normalizzare</param>
        /// <param name="root">elemento radice da eliminare</param>
        /// <returns></returns>
        public static string Normalize(this string xPath, string root)
        {
            if (xPath.IsStringFunction())
            {
                return xPath;
            }

            if (xPath.StartsWith("//"))
            {
                xPath = xPath.Replace("//", string.Empty);
            }

            if (xPath.StartsWith($"{root}/"))
            {
                xPath = xPath.Replace($"{root}/", string.Empty);
            }

            if (xPath.Contains("/text()"))
            {
                xPath = xPath.Replace("/text()", string.Empty);
            }

            return xPath;
        }
    }
}