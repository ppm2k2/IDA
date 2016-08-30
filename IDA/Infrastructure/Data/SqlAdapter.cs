using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ConceptONE.Infrastructure.Extensions;

namespace ConceptONE.Infrastructure.Data
{
    /// <summary>
    /// Generic SQL adapter that can be used in any project.
    /// It should not contain any business specific namespaces.
    /// </summary>
    public class SqlAdapter
    {

        #region Constants

        private const string CREATE_TABLE_COPY = "SELECT * INTO {0} FROM {1}";
        private const string DELETE_ALL_ROWS = "DELETE FROM {0}";
        private const string DROP_TABLE = "DROP TABLE {0}";
        private const string FIELD_EXISTS = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '{0}' and COLUMN_NAME = '{1}'";
        private const string INSERT_TABLE_INTO_TABLE = "INSERT INTO {0} SELECT * FROM {1}";
        private const string SELECT_ROWS_COUNT = "SELECT COUNT(*) AS OutputCount FROM {0}";
        private const string TABLE_EXISTS = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = '{0}'";

        private int SQL_TIMEOUT = 480;
        #endregion

        public string ThreadName;
        public int Id;

        private SqlConnection _Connection = null;
        public string ConnString;

        public void Open(string connectionString)
        {
            if (_Connection == null)
            {
                _Connection = new SqlConnection();
                _Connection.ConnectionString = connectionString;
                _Connection.Open();
            }
        }

        public void Close()
        {
            if (_Connection != null)
            {
                if (_Connection.State != ConnectionState.Closed)
                {
                    _Connection.Close();
                    _Connection.Dispose();
                    _Connection = null;
                }
            }
        }

        public int DeleteTableContents(string tableName)
        {
            int result = -1;

            if (TableExists(tableName))
            {
                string sql = string.Format(DELETE_ALL_ROWS, tableName);
                result = ExecuteNonQuery(sql);
            }

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

        public int ExecuteNonQuery(string sql, params object[] parameters)
        {
            string completeSql = string.Format(sql, parameters);
            return ExecuteNonQuery(completeSql);
        }

        public int ExecuteNonQuery(string sql)
        {
            SqlCommand command = CreateTextCommand(sql);
            int result = command.ExecuteNonQuery();
            CleanupCommand(command);

            return result;
        }

        public SqlDataReader ExecuteReader(string sql)
        {
            SqlCommand command = CreateTextCommand(sql);

            SqlDataReader result = command.ExecuteReader();
            CleanupCommand(command);

            return result;
        }

        public int GetRecordCount(string sql)
        {
            SqlCommand command = CreateTextCommand(sql);
            SqlDataReader reader = command.ExecuteReader();
            CleanupCommand(command);
            int result = 0;

            while (reader.Read())
                result++;

            CleanUpReader(reader);

            return result;
        }

        public int ExecuteIntScalar(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            int result = -1;

            while (reader.Read())
            {
                string value = (reader.IsDBNull(0)) ? "-1" : reader.GetValue(0).ToString();
                Int32.TryParse(value, out result);
            }
            CleanUpReader(reader);

            return result;
        }

        public int ExecuteIntScalararFunction(string function, Dictionary<string, object> parameters)
        {
            int result = -1;

            using (SqlCommand comm = new SqlCommand(function, _Connection))
            {
                comm.CommandType = CommandType.StoredProcedure;

                foreach (string parameterName in parameters.Keys)
                {
                    SqlParameter sqlParameter = GetSqlParameter(parameterName, parameters[parameterName]);
                    comm.Parameters.Add(sqlParameter);
                }

                SqlParameter outputParameter = new SqlParameter("@Result", SqlDbType.VarChar);

                outputParameter.Direction = ParameterDirection.ReturnValue;
                comm.Parameters.Add(outputParameter);

                comm.ExecuteNonQuery();

                if (outputParameter.Value != DBNull.Value)
                    result = (int)outputParameter.Value;
            }

            return result;
        }

        public int ExecuteStoredProcedure(string spName, Dictionary<string, object> inputParameters, ref Dictionary<string, object> outputParameters)
        {
            int result = -1;

            using (SqlCommand comm = new SqlCommand(spName, _Connection))
            {
                comm.CommandType = CommandType.StoredProcedure;

                foreach (string name in inputParameters.Keys)
                {
                    SqlParameter sqlParameter = GetSqlParameter(name, inputParameters[name]);
                    comm.Parameters.Add(sqlParameter);
                }

                foreach (string name in outputParameters.Keys)
                {
                    SqlParameter sqlParameter = GetSqlParameter(name, outputParameters[name]);
                    sqlParameter.Direction = ParameterDirection.Output;
                    comm.Parameters.Add(sqlParameter);
                }

                SqlParameter resultParameter = new SqlParameter("@result", SqlDbType.Int);
                resultParameter.Direction = ParameterDirection.ReturnValue;
                comm.Parameters.Add(resultParameter);

                comm.ExecuteNonQuery();

                if (resultParameter.Value != DBNull.Value)
                    result = (int)resultParameter.Value;

                outputParameters.Clear();

                foreach (SqlParameter parameter in comm.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.Output)
                        outputParameters.Add(parameter.ParameterName, parameter.Value);
                }
            }

            return result;
        }

        public double ExecuteDoubleScalar(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            double result = -1;

            while (reader.Read())
            {
                string value = (reader.IsDBNull(0)) ? "-1" : reader.GetValue(0).ToString();
                double.TryParse(value, out result);
            }
            CleanUpReader(reader);

            return result;
        }

        public int ExecuteIntegerScalar(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            int result = -1;

            while (reader.Read())
            {
                string value = (reader.IsDBNull(0)) ? "-1" : reader.GetValue(0).ToString();
                int.TryParse(value, out result);
            }
            CleanUpReader(reader);

            return result;
        }

        public string ExecuteStringScalar(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            string result = string.Empty;

            while (reader.Read())
                result = (reader.IsDBNull(0)) ? "" : reader.GetValue(0).ToString();
            CleanUpReader(reader);

            return result;
        }

        public List<string> ExecuteStringListScalar(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            List<string> result = new List<string>();
            string value = string.Empty;

            while (reader.Read())
            {
                value = (reader.IsDBNull(0)) ? "" : reader.GetValue(0).ToString();
                result.Add(value);
            }
            CleanUpReader(reader);

            return result;
        }

        public List<int> ExecuteIntegerListScalar(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            List<int> result = new List<int>();
            int value = -1;

            while (reader.Read())
            {
                value = (reader.IsDBNull(0)) ? -1 : reader.GetInt32(0);
                result.Add(value);
            }
            CleanUpReader(reader);

            return result;
        }

        /// <summary>
        /// SQL gets 1 row
        /// </summary>
        /// <returns>
        /// List&lt;string&gt;
        /// </returns>
        public List<string> GetOneRowAsStringList(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            string[] fieldNames;
            List<string> result = new List<string>();

            if (reader.Read())
            {
                fieldNames = new string[reader.FieldCount];
                reader.GetValues(fieldNames);
                result = fieldNames.ToList();
            }
            CleanUpReader(reader);

            return result;
        }

        /// <summary>
        /// SQL gets 1 row
        /// </summary>
        /// <returns>
        /// "value1, value2, value3"
        /// </returns>
        public string GetOneRowAsCommaSeparatedList(string sql)
        {
            List<string> resultList = GetOneRowAsStringList(sql);
            string result = resultList.ToCommaSeparatedList();

            return result;
        }

        /// <summary>
        /// SQL gets 1 col
        /// </summary>
        /// <returns>
        /// List&lt;string&gt;
        /// </returns>
        public List<string> GetOneColumnAsStringList(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            List<string> result = new List<string>();
            object fieldValue = null;

            while (reader.Read())
            {
                fieldValue = reader.GetValue(0);
                result.Add(fieldValue.ToString());
            }
            CleanUpReader(reader);

            return result;
        }

        /// <summary>
        /// SQL gets 1 col
        /// </summary>
        /// <returns>
        /// String
        /// </returns>
        public string GetOneColumnAsString(string sql)
        {
            SqlDataReader reader = ExecuteReader(sql);
            string result = string.Empty;
            object fieldValue = null;

            while (reader.Read())
            {
                fieldValue = reader.GetValue(0);
                result = fieldValue.ToString();
                break;
            }
            CleanUpReader(reader);

            return result;
        }

        /// <summary>
        /// SQL gets 1 col
        /// </summary>
        /// <returns>
        /// "value1, value2, value3"
        /// </returns>
        public string GetOneColumnAsCommaSeparatedList(string sql)
        {
            List<string> resultList = GetOneColumnAsStringList(sql);
            string result = resultList.ToCommaSeparatedList();

            return result;
        }

        public DataTable ExecuteDataTable(string sql)
        {
            SqlCommand command = CreateTextCommand(sql);
            SqlDataReader reader = command.ExecuteReader();
            DataTable result = new DataTable();

            result.Load(reader);
            CleanupCommand(command);
            CleanUpReader(reader);

            return result;
        }

        public DataRow ExecuteDataRow(string sql)
        {
            DataTable table = ExecuteDataTable(sql);
            DataRow result = null;

            if (table.Rows.Count > 0)
                result = table.Rows[0];

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

            using (SqlDataReader reader = ExecuteReader(sql))
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

                if (!result.ContainsKey(key))
                    result.Add(key, values);

            }

            return result;
        }

        public Dictionary<TK, TV> GetAsDictionary<TK, TV>(string sql)
        {
            Dictionary<TK, TV> result = new Dictionary<TK, TV>();

            using (SqlDataReader reader = ExecuteReader(sql))
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

            return result;
        }

        public void InsertDataTable(DataTable table)
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_Connection))
            {
                bulkCopy.DestinationTableName = table.TableName;
                bulkCopy.WriteToServer(table);
            }
        }

        public bool TableExists(string tableName)
        {
            string sql = string.Format(TABLE_EXISTS, tableName.RemoveBrackets());
            SqlDataReader reader = ExecuteReader(sql);
            bool result = reader.Read();
            CleanUpReader(reader);

            return result;
        }

        public bool FieldExists(string tableName, string fieldName)
        {
            string sql = string.Format(FIELD_EXISTS, tableName.RemoveBrackets(), fieldName);
            int count = GetRecordCount(sql);
            bool result = (count > 0);

            return result;
        }

        public void CopyTables(string source, string target)
        {
            if (TableExists(source) && TableExists(target))
            {
                string sql = string.Format(INSERT_TABLE_INTO_TABLE, target, source);
                ExecuteNonQuery(sql);
            }
        }

        public void CreateTableCopy(string source, string target)
        {
            if (TableExists(source))
            {
                string sql = string.Format(CREATE_TABLE_COPY, target, source);
                ExecuteNonQuery(sql);
            }
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

        /// <summary>
        /// This is needed in case we pass the fully qualified table name
        /// </summary>
        public int GetTableRowsCountNoCheck(string table)
        {
            string sql = string.Format(SELECT_ROWS_COUNT, table);
            int result = ExecuteIntegerScalar(sql);

            return result;
        }

        public int GetQueryRowsCount(string sql)
        {
            int result = ExecuteIntegerScalar(sql);
            return result;
        }

        private void CleanupCommand(SqlCommand command)
        {
            if ((command != null))
                command.Dispose();
        }

        private SqlCommand CreateTextCommand(string sql)
        {
            SqlCommand result = _Connection.CreateCommand();
            result.CommandText = sql;
            result.CommandType = CommandType.Text;
            result.CommandTimeout = SQL_TIMEOUT;

            return result;
        }

        private void CleanUpReader(SqlDataReader reader)
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
        }

        private SqlParameter GetSqlParameter(string name, object value)
        {
            SqlDbType dbType;

            if (value is int)
                dbType = SqlDbType.Int;
            else if (value is decimal)
                dbType = SqlDbType.Decimal;
            else if (value is double)
                dbType = SqlDbType.Decimal;
            else if (value is DateTime)
                dbType = SqlDbType.DateTime;
            else if (value is string)
                dbType = SqlDbType.VarChar;
            else
                dbType = SqlDbType.VarChar; //extend as needed

            SqlParameter result = new SqlParameter(name, dbType);
            result.Size = 255;
            result.Value = value;
            result.Direction = ParameterDirection.Input;

            return result;
        }

    }
}
