using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ConceptONE.Infrastructure;
using ConceptONE.Infrastructure.Data;
using ConceptONE.Infrastructure.Extensions;
using IDADataAccess.DEMI.Entities;
using Microsoft.Practices.Unity;

namespace IDADataAccess.DEMI
{

    public class DEMIContext : BaseContext
    {
        private const string SOURCE_DATA_TABLE = "Source_Data_{0}";
        private const string TRANSFORMED_DATA_TABLE = "Transformed_Data_{0}";

        private const string DROP_FUNCTION_SQL =
            "DROP FUNCTION {0};";

        private const string CREATE_FUNCTION_SQL =
            "CREATE FUNCTION {0}({1}) RETURNS VARCHAR(8000) AS " +
            "BEGIN DECLARE @Result VARCHAR(2000); " +
            "    {2} " +
            "    RETURN @Result; " +
            "END;";

        private DataLookup _DataLookup;
        private HashSet<string> _TemporaryFunctions = new HashSet<string>();

        public DataTable SourceDataTable { get; set; }
        public DbSet<Transformation> Transformations { get; set; }
        public DbSet<TransformationSet> TransformationSets { get; set; }

        private DataLookup DataLookup
        {
            get
            {
                if (_DataLookup == null)
                {
                    UnityContainer container = new UnityContainer();

                    container.RegisterInstance(this);

                    _DataLookup = container.Resolve<DataLookup>();
                }

                return _DataLookup;
            }
        }

        public DEMIContext()
            : base("DemiConnection")
        {
            Database.SetInitializer<DEMIContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new TransformationMap());
            modelBuilder.Configurations.Add(new TransformationSetMap());
        }

        #region Source Data

        public string GetSourceDataTableName(string userName)
        {
            string result = String.Format(SOURCE_DATA_TABLE, userName);
            return result;
        }

        public void CreateSourceDataTable(DataTable sourceTable, string userName)
        {
            string tableName = GetSourceDataTableName(userName);
            sourceTable.TableName = tableName;

            AppendIdentityField(sourceTable);
            CreateTableWithTypeFields(sourceTable, true);
        }

        /// <summary>
        /// Load data from source file into source table
        /// </summary>
        public void LoadSourceDataTable(DataTable sourceTable, string userName)
        {
            string tableName = GetSourceDataTableName(userName);
            Logger.LogActivity("Loading data in: {0}", tableName);

            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(Settings.DemiConnectionString))
            {
                bulkcopy.DestinationTableName = tableName;
                bulkcopy.WriteToServer(sourceTable);
            }

            Logger.LogActivity("Data loaded ({0} rows)", sourceTable.Rows);
        }

        #endregion

        #region Target Data

        public string GetTransformedDataTableName(string userName)
        {
            string result = String.Format(TRANSFORMED_DATA_TABLE, userName);
            return result;
        }

        public void CreateTransformedDataTable(DataTable table, string userName)
        {
            string tableName = GetTransformedDataTableName(userName);
            table.TableName = tableName;

            CreateTableWithTypeFields(table, true);
        }

        public DataTable LoadTransformedDataTable(int setId, string userName)
        {
            string sourceTableName = GetSourceDataTableName(userName);
            string targetTableName = GetTransformedDataTableName(userName);
            DataTable sourceTable = GetAsDataTable(sourceTableName);
            List<string> targetFields = GetColumnNames(targetTableName);
            Dictionary<string, string> transformations = GetTransformationList(setId, targetFields);

            //TODO: remove local copy
            SourceDataTable = sourceTable;

            if (transformations.Count == 0)
                Logger.LogActivity("No transformations (from SetId: {0}) can be applied to {1}", setId, targetTableName);
            else
                ExecuteTransformations(transformations, sourceTable, targetTableName, setId);

            DataTable result = GetAsDataTable(targetTableName);
            return result;
        }

        #endregion

        #region Private Methods

        //TODO: refactor
        private void ExecuteTransformations(Dictionary<string, string> transformations, DataTable sourceTable, string targetTableName, int setId)
        {
            const string INSERT_SQL = "INSERT INTO {0} ({1}) SELECT {2} FROM {3} WHERE {3}_ID={4}";

            List<string> sourceFields = GetColumnNames(sourceTable.TableName);
            string fieldNames = transformations.Keys.ToList().ToCommaSeparatedList();

            foreach (DataRow row in sourceTable.Rows)
            {
                int id = (int)row[sourceTable.Columns.Count - 1];

                StringBuilder fieldValues = new StringBuilder();

                foreach (KeyValuePair<string, string> item in transformations)
                {
                    string transformationResult = GetTransformationResult(item, sourceFields, setId, id);
                    fieldValues.AppendFormat("{0}, ", transformationResult);
                }

                fieldValues.TrimEndChars(2);

                string sql = String.Format(
                        INSERT_SQL,
                        targetTableName,
                        fieldNames,
                        fieldValues,
                        sourceTable.TableName,
                        id);

                Logger.LogActivity("Create SQL: {0}", sql);

                ExecuteNonQuery(sql);
            }

            DeleteTemporaryFunctions();
        }

        private void AppendIdentityField(DataTable table)
        {
            string idName = String.Format("{0}_ID", table.TableName);
            int id = 1;

            table.Columns.Add(idName, typeof(int));

            foreach (DataRow row in table.Rows)
                row[idName] = id++;
        }

        private Dictionary<string, string> GetTransformationList(int setId, List<string> targetColumns)
        {
            IQueryable<Transformation> transformations = Transformations.
                Where(t => t.TransformationSetId == setId && targetColumns.Contains(t.TargetColumn));

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (Transformation transformation in transformations)
            {
                if (String.IsNullOrEmpty(transformation.TransformationRule))
                    result.Add(transformation.TargetColumn, "NULL");
                else if (result.ContainsKey(transformation.TargetColumn))
                    Logger.LogActivity("Duplicate TargetColumn {0} in set ID {1}", transformation.TargetColumn, setId);
                else
                    result.Add(transformation.TargetColumn, transformation.TransformationRule);
            }

            return result;
        }

        private List<string> GetParameterList(string expression)
        {
            List<string> parts = expression.Split('@').ToList();
            HashSet<string> result = new HashSet<string>();

            if (parts.Count <= 1)
                return result.ToList();

            for (int i = 1; i < parts.Count; i++)
            {
                string part = parts[i];

                if (part.ToLower().StartsWith("result"))
                    continue;
                else
                {
                    //What if no space?
                    string param = part.Split(' ')[0];
                    result.Add(param);
                }
            }

            return result.ToList();
        }

        private string GetTransformationResult(KeyValuePair<string, string> transformation, List<string> sourceFields, int setId, int id)
        {
            string result = null;
            string expression = ProcessLookups(transformation.Value, id);

            if (expression.StartsWith("[["))
            {
                //Function expression
                expression = expression.RemoveBrackets();
                List<string> parameterNames = GetParameterList(expression);
                List<string> actualFieldNames = GetActualFieldList(parameterNames, sourceFields);
                string functionName = String.Format("dbo.fn_Temp_{0:0000}_{1}", setId, transformation.Key);

                CreateFunction(expression, parameterNames, functionName);

                result = String.Format("{0}({1})", functionName, actualFieldNames.ToCommaSeparatedList(), transformation.Key);
            }
            else
            {
                //Simple expression
                result = expression;
            }

            return result;
        }

        private string ProcessLookups(string value, int id)
        {
            string result = value;

            if (result.IndexOf("Lookup(") > -1)
            {
                List<string> lookupStrings = result.GetSubstringList("Lookup(", ")", true);

                foreach (string lookupString in lookupStrings)
                {
                    LookupItem lookupItem = new LookupItem(lookupString);
                    lookupItem.Id = id;

                    string lookupResult = DataLookup.GetLookupResult(lookupItem);

                    result = result.Replace(lookupString, lookupResult);
                }
            }

            return result;
        }

        private void CreateFunction(string expression, List<string> parameterNames, string functionName)
        {
            string parameterDefinitions = parameterNames.ToCommaSeparatedList("@{0} VARCHAR(MAX)");
            string createFunctionSql = String.Format(CREATE_FUNCTION_SQL, functionName, parameterDefinitions, expression);
            string dropFunctionSql = String.Format(DROP_FUNCTION_SQL, functionName);

            ExecuteNonQueryNoException(dropFunctionSql);
            ExecuteNonQuery(createFunctionSql);

            _TemporaryFunctions.Add(functionName);
        }

        /// <summary>
        /// Normalizes parameters in case they contain spaces
        /// 
        /// TODO: make case insensitive
        /// </summary>
        private List<string> GetActualFieldList(List<string> parameterNames, List<string> sourceFields)
        {
            List<string> result = new List<string>();

            foreach (string param in parameterNames)
            {
                if (sourceFields.Contains(param))
                    result.AddFormat("[{0}]", param);
                else if (sourceFields.Contains(param.Replace("_", " ")))
                    result.AddFormat("[{0}]", param.Replace("_", " "));
                else
                    result.AddFormat("[{0}_NOT_FOUND]", param);
            }

            return result;
        }

        private void DeleteTemporaryFunctions()
        {
            foreach (string function in _TemporaryFunctions)
            {
                string sql = String.Format(DROP_FUNCTION_SQL, function);
                ExecuteNonQueryNoException(sql);
            }

            _TemporaryFunctions.Clear();
        }

        #endregion

    }
}
