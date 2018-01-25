using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Utils.Data.DatabaseClient.Abstractions;
using Utils.Extensions.Collections;

namespace Utils.Data.DatabaseClient.FileSystem
{
    public class FileSystemStorageManager : IStorageManager
    {
        private const string QueryFileName = "query.log";

        public FileSystemStorageManager(string connectionString)
        {
            BasePath = connectionString;
            ConnectionString = connectionString;
        }

        public void Append(string text)
        {
            using (var sw = new StreamWriter(GetFullPath(),true))
            {
                Append(text,sw);
            }
        }

        public void Append(params string[] texts)
        {
            if (texts == null || !texts.Any())
            {
                return;
            }

            using (var sw = new StreamWriter(GetFullPath(),true))
            {
                foreach (var text in texts)
                {
                    Append(text, sw);
                }
            }
        }


        public void Append(string text, StreamWriter sw)
        {
            sw.WriteLine("--------------------     {0}      -------------------", text);
        }

        public string GetFullPath()
        {
            return Path.Combine(BasePath, QueryFileName);
        }

        public string BasePath { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool IsAllOkTransaction { get; set; }
        public void OpenTransaction()
        {
            
        }

        public void OpenTransaction(IsolationLevel? iso)
        {
            
        }

        public void CloseTransaction(bool isAllOk)
        {
            
        }

        public void CloseTransaction()
        {
            
        }

        public void Commit()
        {
            
        }

        public void RollBack()
        {
            
        }

        public void InTransaction(Action block, IsolationLevel? isolationLevel = null)
        {
            block();
        }

        public bool TestConnection()
        {
            var fullPath = GetFullPath();
            var dirInfo = new DirectoryInfo(fullPath);
            if (!dirInfo.Exists)
            {
                throw new DirectoryNotFoundException($"La directory {dirInfo.FullName} non è presente sul file system");
            }

            var fileInfo = new FileInfo(fullPath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"Il file {fileInfo.FullName} non è stato trovato");
            }

            return true;
        }

        public bool TestConnection(string connectionString)
        {
            BasePath = connectionString;
            return TestConnection();
        }

        public bool TestConnection(string connectionString, out string error)
        {
            bool result;
            error = string.Empty;
            try
            {
                result = TestConnection(connectionString);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
            return result;
        }

        private string Stringify(IEnumerable<IDbDataParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
            if (!parameters.Any())
            {
                return string.Empty;
            }

            return
                parameters.Select(prm => $"prmName {prm.ParameterName} = prmValue = {prm.Value}")
                    .Stringify(" - ");
        }

        public void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters)
        {
            Update(update,dataSet,parameters,false);
        }

        public void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters, bool isStored)
        {
            string error;
            Update(update, dataSet, parameters,isStored, out error);
        }

        public void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters, out string error)
        {
            Update(update,dataSet, parameters, false, out error);
        }

        public void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters, bool isStored,
            out string error)
        {
            Exception ex;
            Update(update, dataSet, parameters, isStored, out error, out ex);
        }

        public void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters, bool isStored,
            out string error, out Exception exception)
        {
            error = string.Empty;
            exception = null;

            var sb = new StringBuilder();
            sb.AppendLine("EseguiUpdate");
            sb.AppendLine($"isStored: {isStored}");
            sb.AppendLine(update);
            sb.AppendLine(Stringify(parameters));
            sb.AppendLine("EseguiUpdate End");
            sb.AppendLine();
            Append(sb.ToString());
        }

        public void Insert<TDataSource>(string insert, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters)
        {
            var sb = new StringBuilder();
            sb.AppendLine("EseguiInsert");
            sb.AppendLine(insert);
            sb.AppendLine(Stringify(parameters));
            sb.AppendLine("EseguiInsert End");
            sb.AppendLine();
            Append(sb.ToString());
        }

        public DataTable CloneDataTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public DataTable ExecuteCommandOnDataTable(DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        public int BulkInsert(DataSet dsTabelle)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters)
        {
            return ExecuteCommand(storeProcedure, parameters, false);
        }

        public DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore)
        {
            var error = string.Empty;
            return ExecuteCommand(storeProcedure, parameters, isStore, out error);
        }

        public DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore, out string error)
        {
            Exception exception = null;
            return ExecuteCommand(storeProcedure, parameters, isStore, out error, out exception);
        }

        public DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore, out string error,
            out Exception exception)
        {
            error = string.Empty;
            exception = null;

            var sb = new StringBuilder();
            sb.AppendLine("EseguiStored");
            sb.AppendLine($"isStored: {isStore}");
            sb.AppendLine(storeProcedure);
            sb.AppendLine(Stringify(parameters));
            sb.AppendLine("EseguiStored End");
            sb.AppendLine();
            Append(sb.ToString());

            return new DataSet();
        }

        public DataSet Query(string storeProcedure, IEnumerable<IDbDataParameter> parameters = null, bool isStore = false)
        {
            return ExecuteCommand(storeProcedure, parameters, isStore);
        }

        public string ConnectionString { get; set; }
    }
}