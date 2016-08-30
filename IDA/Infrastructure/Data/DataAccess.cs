using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using ConceptONE.Infrastructure.Extensions;

namespace ConceptONE.Infrastructure.Data
{
    public class DataAccess : IDisposable
    {

        #region Constants

        public const string CREATE_TABLE_SQL = "CREATE TABLE {0} ({1})";
        public const string SELECT_ALL_ROWS = "SELECT * FROM {0}";
        public const string SELECT_FILTERED_ROWS = "SELECT * FROM {0} WHERE {1}";
        public const string SELECT_NO_ROWS = "SELECT TOP 0 * FROM {0}";
        public const string SELECT_TOP_ROW = "SELECT TOP 1 * FROM {0} ORDER BY {1} {2}";

        #endregion

        #region Protected Fields

        protected SqlAdapter _SqlAdapter;
        protected bool _Initialized = false;
        protected string _ConnectionString;

        protected bool DebugMode
        {
            get
            {
                bool result = (ConfigurationManager.AppSettings["DebugMode"] == "true");
                return result;
            }
        }

        #endregion

        #region Constructor

        public DataAccess()
        {
        }

        public DataAccess(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            Close();
        }

        public void Open()
        {
            if ((!_Initialized))
            {
                _Initialized = true;
                _SqlAdapter = GetInstance(_ConnectionString);
            }
        }

        public void Close()
        {
            if ((_Initialized))
            {
                if (_SqlAdapter != null)
                    _SqlAdapter.Close();
                _Initialized = false;
            }
        }

        public DataRow GetDataRow(string tableName, string orderBy, bool descending)
        {
            DataRow result = null;
            string sql;

            if (_SqlAdapter.TableExists(tableName))
            {
                string sortOrder = (descending) ? "DESC" : "";
                sql = string.Format(SELECT_TOP_ROW, tableName, orderBy, sortOrder);
                DataTable table = _SqlAdapter.ExecuteDataTable(sql);
                table.TableName = tableName;
                if (table.Rows.Count > 0)
                    result = table.Rows[0];
                else
                    result = table.NewRow();
            }

            return result;
        }

        public DataTable ExecuteDataTable(string sql)
        {
            DataTable result = _SqlAdapter.ExecuteDataTable(sql);
            return result;
        }

        public int ExecuteNonQuery(string sql)
        {
            int result = _SqlAdapter.ExecuteNonQuery(sql);
            return result;
        }

        public void EnableIdentityInsert(string table)
        {
            _SqlAdapter.ExecuteNonQuery("SET IDENTITY_INSERT {0} ON", table);
        }

        public void DisableIdentityInsert(string table)
        {
            _SqlAdapter.ExecuteNonQuery("SET IDENTITY_INSERT {0} OFF", table);
        }

        public DataTable GetDataTable(string tableName)
        {
            DataTable result;
            string sql;

            if (_SqlAdapter.TableExists(tableName))
            {
                sql = string.Format(SELECT_ALL_ROWS, tableName);
                result = _SqlAdapter.ExecuteDataTable(sql);
                result.TableName = tableName;
            }
            else
            {
                result = new DataTable();
                result.TableName = tableName + " [Table not found]";
            }

            return result;
        }

        public DataTable GetDataTable(string tableName, string filter)
        {
            DataTable result;
            string sql;

            if (_SqlAdapter.TableExists(tableName))
            {
                if (string.IsNullOrEmpty(filter))
                    sql = string.Format(SELECT_ALL_ROWS, tableName);
                else
                    sql = string.Format(SELECT_FILTERED_ROWS, tableName, filter);
                result = _SqlAdapter.ExecuteDataTable(sql);
                result.TableName = tableName;
            }
            else
            {
                result = new DataTable();
                result.TableName = tableName;
            }

            return result;
        }

        public void InsertDataTable(DataTable table)
        {
            _SqlAdapter.InsertDataTable(table);
        }

        public bool DropTable(string tableName)
        {
            return _SqlAdapter.DropTable(tableName);
        }

        public int DeleteTableContents(string tableName)
        {
            int result = _SqlAdapter.DeleteTableContents(tableName);
            return result;
        }

        public bool FieldExists(string table, string field)
        {
            bool result = _SqlAdapter.FieldExists(table, field);
            return result;
        }

        public bool FieldsExists(string table, List<string> fields)
        {
            foreach (string field in fields)
            {
                if (!FieldExists(table, field))
                    return false;
            }

            return true;
        }

        public bool TableExists(string table)
        {
            bool result = _SqlAdapter.TableExists(table);
            return result;
        }

        public int GetRowsCount(string table)
        {
            return _SqlAdapter.GetTableRowsCount(table);
        }

        public int GetRowsCountNoCheck(string table)
        {
            return _SqlAdapter.GetTableRowsCountNoCheck(table);
        }

        public List<string> GetColumnNames(string table)
        {
            string sql = string.Format(SELECT_NO_ROWS, table);
            DataTable dataTable = _SqlAdapter.ExecuteDataTable(sql);
            List<string> result = new List<string>();

            foreach (DataColumn column in dataTable.Columns)
                result.Add(column.ColumnName);

            return result;
        }

        public void ExportTable(string tableName, string outputFilePath)
        {
            DataTable table = GetDataTable(tableName);
            table.Export(outputFilePath);
        }

        public void CreateTableWithTypeFields(DataTable table, bool recreateIfAlreadyExists)
        {
            if (recreateIfAlreadyExists || !TableExists(table.TableName))
                CreateTableWithTypeFields(table.TableName, table.Columns);
        }

        public void CreateTableWithTypeFields(string tableName, DataColumnCollection columns)
        {
            string commaSeparatedFields = GetTypeFieldDefinitionsSql(columns);
            string createTableSQL = string.Format(CREATE_TABLE_SQL, tableName, commaSeparatedFields);

            _SqlAdapter.DropTable(tableName);
            _SqlAdapter.ExecuteNonQuery(createTableSQL);

            Logger.LogActivity("Table created: " + tableName);
        }

        #endregion

        #region Private/Protected Methods

        protected void CleanUpReader(SqlDataReader reader)
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
        }

        protected void ThrowException(string format, params object[] parmeters)
        {
            string message = string.Format(format, parmeters);
            throw new Exception(message);
        }

        protected decimal? GetDecimalValue(SqlDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return (decimal?)reader.GetDecimal(index);
        }

        protected string GetNumericFieldDefinitionsSql(List<string> columnNames, int precision)
        {
            string result = string.Empty;

            foreach (string columnName in columnNames)
                result += string.Format("[{0}] NUMERIC(24,{1}),", columnName, precision);

            return result.Trim(',');
        }

        protected string GetStringFieldDefinitionsSql(List<string> columnNames)
        {
            string result = string.Empty;

            foreach (string columnName in columnNames)
                result += string.Format("[{0}] VARCHAR(MAX),", columnName);

            return result.Trim(',');
        }

        private string GetTypeFieldDefinitionsSql(DataColumnCollection columns)
        {
            string result = string.Empty;
            string sqlType = "NVARCHAR(MAX)";

            foreach (DataColumn column in columns)
            {
                if (column.DataType == typeof(decimal))
                    sqlType = "NUMERIC(24,6)";
                else if (column.DataType == typeof(double))
                    sqlType = "NUMERIC(24,6)";
                else if (column.DataType == typeof(int))
                    sqlType = "INT";
                else if (column.DataType == typeof(bool))
                    sqlType = "BIT";
                else if (column.DataType == typeof(DateTime))
                    sqlType = "DATETIME";
                else
                    sqlType = "NVARCHAR(MAX)";

                result += string.Format("[{0}] {1},", column.ColumnName, sqlType);
            }

            return result.Trim(',');
        }

        private SqlAdapter GetInstance(string connectionString)
        {
            SqlAdapter result = new SqlAdapter();
            result.ThreadName = System.Threading.Thread.CurrentThread.Name;
            result.Id = result.GetHashCode();
            result.ConnString = connectionString;

            result.Open(connectionString);
            return result;
        }

        #endregion

    }
}