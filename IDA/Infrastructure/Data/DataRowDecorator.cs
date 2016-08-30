using System;
using System.Collections.Generic;
using System.Data;

namespace ConceptONE.Infrastructure.Data
{
    public class DataRowDecorator
    {

            #region Fields

            private DataRow _DataRow;

            #endregion

            public DataRowDecorator(DataRow dataRow)
            {
                _DataRow = dataRow;
            }

            public string GetAsString(string field, bool trim = false)
            {
                string result = string.Empty;

                if (_DataRow.Table.Columns.Contains(field))
                    result = _DataRow[field].ToString();

                return trim ? result.Trim() : result;
            }

            public DateTime GetAsDateTime(string field)
            {
                DateTime result = DateTime.MinValue;
                string value = GetAsString(field);
                DateTime.TryParse(value, out result);

                return result;
            }

            public double GetAsDouble(string field)
            {
                double result = 0;
                string value = GetAsString(field);
                double.TryParse(value, out result);

                return result;
            }

            public double? GetAsNullableDouble(string field)
            {
                double? result = null;
                double value;

                if (_DataRow.Table.Columns.Contains(field))
                    if (_DataRow[field] != null)
                        if (double.TryParse(_DataRow[field].ToString(), out value))
                            result = (double?)value;

                return result;
            }

            public decimal GetAsDecimal(string field)
            {
                decimal result = 0;
                string value = GetAsString(field);
                decimal.TryParse(value, out result);

                return result;
            }

            public int GetAsInteger(string field)
            {
                int result = 0;
                string value = GetAsString(field);
                Int32.TryParse(value, out result);

                return result;
            }

            public Dictionary<string, double> GetValues(List<string> fields)
            {
                Dictionary<string, double> result = new Dictionary<string, double>();

                foreach (string field in fields)
                {
                    double value = GetAsDouble(field);
                    result.Add(field, value);
                }

                return result;
            }

            public Dictionary<string, double?> GetNullableValues(List<string> fields)
            {
                Dictionary<string, double?> result = new Dictionary<string, double?>();

                foreach (string field in fields)
                {
                    double? value = null;

                    if (_DataRow[field].GetType() != typeof(DBNull))
                        value = GetAsDouble(field);

                    result.Add(field, value);
                }

                return result;
            }

    }
}
