using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConceptONE.Infrastructure.Extensions;

namespace ConceptONE.Infrastructure.Data
{
    public class CsvBulkLoader
    {
        private List<string> _FileLines = new List<string>();
        private List<string> _Columns;

        DataAccess _DataAccess;

        const string INSERT_SQL = "INSERT INTO {0} ({1}) VALUES ({2})";

        private CsvBulkLoader()
        {
        }

        public CsvBulkLoader(DataAccess dataAccess)
        {
            _DataAccess = dataAccess;
        }

        public int SaveFileToTable(string file, string table, bool deleteExistingData)
        {
            _FileLines = File.ReadAllLines(file).ToList();
            int result = SaveFileToTable(table, deleteExistingData);

            return result;
        }

        public int SaveFileToTable(Stream stream, string table, bool deleteExistingData)
        {
            string line = "";

            using (StreamReader reader = new StreamReader(stream))
            {
                while (reader.Peek() > -1)
                {
                    line = reader.ReadLine();
                    _FileLines.Add(line);
                }
            }

            int result = SaveFileToTable(table, deleteExistingData);

            return result;
        }

        private int SaveFileToTable(string table, bool deleteExistingData)
        {
            int result = 0;

            if (!_DataAccess.TableExists(table))
                throw new CustomException("Table {0} does not exist", table);

            if (FileContantsAreValid(table))
            {
                result = _FileLines.Count;

                _DataAccess.DeleteTableContents(table);
                _DataAccess.EnableIdentityInsert(table);

                foreach (String line in _FileLines)
                    ProcessRecord(table, line);

                _DataAccess.DisableIdentityInsert(table);
            }

            return result;
        }

        private void ProcessRecord(string table, string line)
        {
            List<string> values = line.Split(',').ToList();
            string insertSql = GetInsertStatement(table, values);

            _DataAccess.ExecuteNonQuery(insertSql);
        }

        private string GetInsertStatement(string table, List<string> allValues)
        {
            List<string> columns = new List<string>();
            List<string> values = new List<string>();

            for (int i = 0; i < _Columns.Count; i++)
            {
                if (!String.IsNullOrEmpty(allValues[i]) && allValues[i] != "NULL")
                {
                    columns.Add(_Columns[i]);
                    values.Add(allValues[i]);
                }
            }

            string result = String.Format(INSERT_SQL, table, columns.ToCommaSeparatedList(), values.ToCommaSeparatedListWithSingleQuotes());  

            return result;
        }

        private bool FileContantsAreValid(string table)
        {
            if (_FileLines.Count <= 1)
            {
                Logger.LogActivity("File is empty");
                return false;
            }

            _Columns = GetFileColumns();
            List<string> tableColumns = GetTableColumns(table);
            bool result = _Columns.SequenceEqual(tableColumns);

            if (!result)
                throw new CustomException("File schema ({0} columns), does not match table schema ({1} columns)", _Columns.Count, tableColumns.Count);

            _FileLines.RemoveAt(0);

            return result;
        }

        private List<string> GetFileColumns()
        {
            List<string> result = _FileLines[0].Split(',').ToList();
            return result;
        }

        private List<string> GetTableColumns(string table)
        {
            List<string> result = _DataAccess.GetColumnNames(table);
            return result;
        }
    }
}
