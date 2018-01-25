using System;
using System.Data;
using System.Linq;
using Utils.Extensions.Reflection;

namespace Utils.Extensions.Data
{
    /// <summary>
    /// Raccolta di estezioni a DataRow
    /// </summary>
    public static class DataRowExtensions
    {
        /// <summary>
        /// Recupera un valore da una colonna di una DataRow tramite il nome colonna
        /// Effettua il controllo se il valore è DBNull.Value in tal caso torna null
        /// </summary>
        /// <typeparam name="TType">Tipo di ritorno Generic</typeparam>
        /// <param name="row">DataRow da valutare</param>
        /// <param name="columnName">Nome colonna</param>
        /// <param name="caseSensitive">Indicare se la ricerca della colonna deve essere case sensistive oppure no</param>
        /// <returns>Il valore recuperato con casting a TTYpe o conversione GetValue[TType]</returns>
        public static TType GetValueByColumnName<TType>(this DataRow row, string columnName, bool caseSensitive = true)
        {

            var objectValue = row.GetValueByColumnName(columnName, caseSensitive);
            if (objectValue == DBNull.Value)
            {
                objectValue = null;
            }

            return objectValue.GetValue<TType>();
        }

        /// <summary>
        /// Recupera l'object value da una colonna di una DataRow tramite il nome colonna
        /// </summary>
        /// <param name="row">DataRow da valutare</param>
        /// <param name="columnName">Nome colonna da cercare</param>
        /// <param name="caseSensitive">Indica se la ricerca della colonna deve essere case sensistive</param>
        /// <exception cref="ArgumentNullException">row e columnName non possono essere null</exception>
        /// <exception cref="DataException">in caso di colonna non trovata</exception>
        /// <exception cref="Exception">in caso di nessuna colonna trovata nella tabella</exception> 
        /// <returns></returns>
        public static object GetValueByColumnName(this DataRow row, string columnName, bool caseSensitive = true)
        {
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }
            if (String.IsNullOrEmpty(columnName))
            {
                throw new ArgumentNullException(columnName);
            }

            var columnNotFoundMessage = String.Intern(
                $"la colonna {columnName} non è presente nella tabella di riferimento");
            if (caseSensitive)
            {
                if (!row.Table.Columns.Contains(columnName))
                {
                    throw new DataException(columnNotFoundMessage);
                }
                return row[columnName];
            }


            var cols = row.Table.Columns.Cast<DataColumn>().ToList();
            if (!cols.Any())
            {
                throw new Exception("Nessuna colonna trovata nella tabella");
            }

            var dataColumn =
              cols.FirstOrDefault(
                column => String.Equals(columnName, column.ColumnName, StringComparison.CurrentCultureIgnoreCase));

            if (dataColumn == null)
            {
                throw new DataException(columnNotFoundMessage);
            }

            return row[dataColumn];
        }




        /// <summary>
        /// Aggiunge una relazione tra tabelle per creare sul DataSet il vincolo di Chiave esterna
        /// </summary>
        /// <param name="rowToUpdate">DataRow da aggiornare</param>
        /// <param name="parentIdName">nome della chiave esterna, per convenzione deve essere IdNomeTabella. serve per recuperare la colonna relativa della tabella parent</param>
        /// <param name="rowFkColumnName">nome del campo della data row corrente che corrisponde alla chiave per relazionare la tabella corrente con la tabella parent </param>
        /// <exception cref="ArgumentNullException">rowToUpdate, parentIdName e rowFkColumnName non possono essere nulli o vuoti </exception>
        public static void AddRelationToSet(this DataRow rowToUpdate, string parentIdName, string rowFkColumnName)
        {
            if (rowToUpdate == null)
            {
                throw new ArgumentNullException("rowToUpdate");
            }

            if (string.IsNullOrEmpty(parentIdName))
            {
                throw new ArgumentNullException("parentIdName");
            }

            if (string.IsNullOrEmpty(rowFkColumnName))
            {
                throw new ArgumentNullException("rowFkColumnName");
            }

            var dataSet = rowToUpdate.Table.DataSet;
            var tableParentName = parentIdName.Substring(3);

            var parentColumnId = dataSet.Tables[tableParentName].Columns["Id"];
            var childColumn = rowToUpdate.Table.Columns[rowFkColumnName];
            var relationName =
                $"FK_{childColumn.Table.TableName}_{parentColumnId.Table.TableName}_{childColumn.ColumnName}";

            if (dataSet.Relations.Contains(relationName))
            {
                return;
            }

            var relation = new DataRelation(relationName, parentColumnId, childColumn, true);

            dataSet.Relations.Add(relation);
        }
    }
}
