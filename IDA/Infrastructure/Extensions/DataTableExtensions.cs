using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class DataTableExtensions
    {
        public static List<string> GetDuplicateValues(this DataTable table, string fieldName)
        {
            List<DataRow> rowList = table.AsEnumerable().ToList();
            List<string> result = rowList.GroupBy(p => p[fieldName].ToString()).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

            return result;
        }

        public static void Export(this DataTable table, string path)
        {
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            StreamWriter writer = new StreamWriter(path);
            string tableContent = table.GetTableContent();

            writer.Write(tableContent);
            writer.Close();
        }

        public static string GetTableContent(this DataTable table)
        {
            StringBuilder result = new StringBuilder();
            string cellValue;

            result.Append("\"");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].ColumnName);
                result.Append(i == table.Columns.Count - 1 ? "\"\n\"" : "\",\"");
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    cellValue = GetCellValue(row, i);
                    result.Append(cellValue);
                    result.Append(i == table.Columns.Count - 1 ? "\"\n\"" : "\",\"");
                }
            }

            return result.ToString().TrimEnd('\"');
        }

        public static void Import(this DataTable table, StreamReader reader)
        {
            string row = reader.ReadLine();
            string[] columns = row.Replace("\"", "").Split(',');

            while (!reader.EndOfStream)
            {
                row = reader.ReadLine();
                string[] fields = row.Replace("\"", "").Split(',');

                DataRow newRow = table.NewRow();

                for (int index=0; index < fields.Length; index++)
                {
                    if (string.IsNullOrEmpty(fields[index]))
                        newRow[index] = DBNull.Value;
                    else
                        newRow[index] = fields[index];
                }

                table.Rows.Add(newRow);
            }
        }

        private static string GetCellValue(DataRow row, int i)
        {
            Type columnDataType = row.Table.Columns[i].DataType;
            string result = string.Empty;
            object value = row[i];

            if (columnDataType == typeof(DateTime))
            {
                DateTime dtValue = DateTime.MaxValue;
                if (DateTime.TryParse(value.ToString(), out dtValue))
                    result = dtValue.ToString("MM/dd/yyyy");
                else
                    result = null;
            }
            else
            {
                result = value.ToString();
            }

            return result;
        }

        public static List<string> GetColumnNames(this DataTable table)
        {
            List<string> result = new List<string>();

            foreach (DataColumn column in table.Columns)
                result.Add(column.ColumnName);

            return result;
        }

    }
}
