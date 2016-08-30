using System;
using System.Data;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class DataRowExtensions
    {

        public static decimal GetValueAsDecimal(this DataRow row, string columnName)
        {
            decimal result = -1;

            if (row.Table.Columns.Contains(columnName))
                result = row.Field<decimal>(columnName);

            return result;
        }

        public static DateTime GetValueAsDateTime(this DataRow row, string columnName)
        {
            DateTime result = DateTime.MinValue;

            if (row.Table.Columns.Contains(columnName))
                result = row.Field<DateTime>(columnName);

            return result;
        }

        public static string GetValueAsString(this DataRow row, string columnName)
        {
            string result = string.Empty;

            if (row.Table.Columns.Contains(columnName))
                result = row[columnName].ToString();

            return result;
        }

        public static bool GetValueAsBoolean(this DataRow row, string columnName)
        {
            string stringValue = row.GetValueAsString(columnName);
            bool result = stringValue.ToBoolean();

            return result;
        }

        public static string GetAsStringOrNullLiteral(this DataRow row, string columnName)
        {
            string result = row.IsNull(columnName) ? "NULL" : row[columnName].ToString();
            return result;
        }

        public static string GetAsStringOrNull(this DataRow row, string columnName)
        {
            string result = row.IsNull(columnName) ? null : row[columnName].ToString();
            return result;
        }

    }
}
