using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using ConceptONE.Infrastructure.Extensions;

namespace ConceptONE.Infrastructure.Data
{

    public class BaseContext : DbContext
    {

        #region constants

        private const string CREATE_TABLE_SQL = "CREATE TABLE {0} ({1})";
        private const string DROP_TABLE = "DROP TABLE {0}";
        private const string SELECT_ALL_ROWS = "SELECT * FROM {0}";
        private const string SELECT_NO_ROWS = "SELECT TOP 0 * FROM {0}";
        private const string SELECT_ROWS_COUNT = "SELECT COUNT(*) AS OutputCount FROM {0}";
        private const string TABLE_EXISTS = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = @p0";

        private int SQL_TIMEOUT = 480;

        #endregion

        #region Public/Internal Methods

        public BaseContext(string name) : base(name)
        {
        }

        public bool TableExists(string tableName)
        {
            int count = Database.SqlQuery<int>(TABLE_EXISTS, tableName.RemoveBrackets()).FirstOrDefault();
            bool result = count > 0;

            return result;
        }

        public bool DropTable(string tableName)
        {
            try
            {
                string sql = string.Format(DROP_TABLE, tableName);
                ExecuteNonQuery(sql);
                return true;
            }
            catch
            {
                //Table did not exist
                return false;
            }
        }

        public DataTable GetAsDataTable(string sql)
        {
            DataTable result = new DataTable();

            if (sql.IndexOf(' ') == -1)
            {
                result.TableName = sql;
                sql = string.Format(SELECT_ALL_ROWS, sql);
            }

            OpenConnection();

            using (DbCommand command = CreateTextCommand(sql))
            using (DbDataReader reader = command.ExecuteReader())
                result.Load(reader);

            CloseConnection();

            return result;
        }

        public object GetScalarValue(string sql)
        {
            OpenConnection();

            object result = null;

            using (DbCommand command = CreateTextCommand(sql))
            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                    result = (reader.IsDBNull(0)) ? null : reader.GetValue(0);
            }

            CloseConnection();

            return result;
        }

        public T GetScalarValue<T>(string sql)
        {
            object result = GetScalarValue(sql);
            return (T)result;
        }

        protected DbDataReader ExecuteDataReader(string sql)
        {
            DbCommand command = CreateTextCommand(sql);
            DbDataReader result = command.ExecuteReader();

            return result;
        }

        public DataRow GetAsDataRow(string sql)
        {
            DataTable table = GetAsDataTable(sql);
            DataRow result = table.Rows.Count > 0 ? table.Rows[0] : null;

            return result;
        }

        public List<string> GetColumnNames(string table)
        {
            string sql = string.Format(SELECT_NO_ROWS, table);
            DataTable dataTable = GetAsDataTable(sql);
            List<string> result = new List<string>();

            foreach (DataColumn column in dataTable.Columns)
                result.Add(column.ColumnName);

            return result;
        }

        public int GetTableRowsCount(string table)
        {
            int result = 0;

            if (TableExists(table))
            {
                string sql = string.Format(SELECT_ROWS_COUNT, table);
                result = ExecuteIntegerScalar(sql);
            }

            return result;
        }

        public Dictionary<TK, TV> GetAsDictionary<TK, TV>(string sql)
        {
            Dictionary<TK, TV> result = new Dictionary<TK, TV>();

            OpenConnection();

            using (DbCommand command = CreateTextCommand(sql))
            using (DbDataReader reader = command.ExecuteReader())
            {
                TK key;
                TV value;

                while (reader.Read())
                {
                    key = (TK)Convert.ChangeType(reader[0], typeof(TK));
                    value = (TV)Convert.ChangeType(reader[1], typeof(TV));

                    if (!result.ContainsKey(key))
                        result.Add(key, value);
                }
            }

            CloseConnection();

            return result;
        }

        /// <summary>
        /// Assumes that:
        /// 1) SQL returns KEY in first field (Index: 0) and VALUE in second field (Index: 1)
        /// 2) Result is sorted by first field, second field
        /// </summary>
        public Dictionary<string, List<string>> GetAsStringListDictionary(string sql)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            List<string> values = new List<string>();

            OpenConnection();

            using (DbCommand command = CreateTextCommand(sql))
            using (DbDataReader reader = command.ExecuteReader())
            {
                string key = "";
                string previousKey = "";
                string value = "";

                while (reader.Read())
                {
                    key = reader[0].ToString();
                    value = reader[1].ToString();

                    if (!key.Equals(previousKey) && values.Count > 0)
                    {
                        if (!result.ContainsKey(previousKey))
                            result.Add(previousKey, values);
                        values = new List<string>();
                    }

                    values.Add(value);
                    previousKey = key;
                }

                if (!result.ContainsKey(key) && !String.IsNullOrEmpty(key))
                    result.Add(key, values);
            }

            CloseConnection();

            return result;
        }

        public Dictionary<string, T> GetRowAsDictionary<T>(string sql, bool includeNullValues, int startIndex = 0)
        {
            Dictionary<string, T> result = new Dictionary<string, T>();

            OpenConnection();

            using (DbCommand command = CreateTextCommand(sql))
            using (DbDataReader reader = command.ExecuteReader())
            {
                string column;
                T value;

                if (reader.Read())
                {
                    for (int index = startIndex; index < reader.FieldCount; index++)
                    {
                        if (includeNullValues || !reader.IsDBNull(index))
                        {
                            column = reader.GetName(index);
                            value = (T)Convert.ChangeType(reader[index], typeof(T));
                            result.Add(column, value);
                        }
                    }
                }
            }

            CloseConnection();

            return result;
        }

        public void ExecuteStoredProcedure(string storedProcedure)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ExecuteStoredProcedure(storedProcedure, parameters);
        }

        public void ExecuteStoredProcedure(string storedProcedure, Dictionary<string, object> parameters)
        {
            OpenConnection();

            using (DbCommand comm = CreateTextCommand(storedProcedure))
            {
                foreach (string parameterName in parameters.Keys)
                {
                    DbParameter sqlParameter = GetSqlParameter(parameterName, parameters[parameterName]);
                    comm.Parameters.Add(sqlParameter);
                }

                comm.CommandType = CommandType.StoredProcedure;
                comm.ExecuteNonQuery();
            }

            CloseConnection();
        }

        public int ExecuteIntegerScalar(string sql)
        {
            int result = -1;

            OpenConnection();

            using (DbCommand command = CreateTextCommand(sql))
            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string value = (reader.IsDBNull(0)) ? "-1" : reader.GetValue(0).ToString();
                    int.TryParse(value, out result);
                }
            }

            CloseConnection();

            return result;
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

            DropTable(tableName);
            ExecuteNonQuery(createTableSQL);

            Logger.LogActivity("Table created: " + tableName);
        }

        #endregion

        #region Protected/Private Methods

        protected int ExecuteNonQuery(string sql)
        {
            int result = Database.ExecuteSqlCommand(sql);

            return result;
        }

        protected int ExecuteNonQueryNoException(string sql)
        {
            int result = 0;

            try
            {
                result = Database.ExecuteSqlCommand(sql);
            }
            catch
            {
                result = -1;
            }

            return result;
        }

        protected int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            string completeSql = string.Format(sql, parameters);
            int result = Database.ExecuteSqlCommand(completeSql);

            return result;
        }

        protected bool DataFound(string sql)
        {
            DataRow row = GetAsDataRow(sql);
            bool result = row != null;

            return result;
        }

        private void OpenConnection()
        {
            if (Database.Connection.State == ConnectionState.Closed)
                Database.Connection.Open();
        }

        private void CloseConnection()
        {
            if (Database.Connection.State == ConnectionState.Open)
                Database.Connection.Close();
        }

        private DbCommand CreateTextCommand(string sql)
        {
            DbCommand result = Database.Connection.CreateCommand();
            result.CommandText = sql;
            result.CommandType = CommandType.Text;
            result.CommandTimeout = SQL_TIMEOUT;

            return result;
        }

        private DbParameter GetSqlParameter(string name, object value)
        {
            SqlDbType dbType;

            if (value is int)
                dbType = SqlDbType.Int;
            else if (value is DateTime)
                dbType = SqlDbType.DateTime;
            else
                dbType = SqlDbType.VarChar; //extend as needed

            DbParameter result = new SqlParameter(name, dbType);

            result.Value = value;
            result.Direction = ParameterDirection.Input;

            return result;
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

        #endregion

    }
}
