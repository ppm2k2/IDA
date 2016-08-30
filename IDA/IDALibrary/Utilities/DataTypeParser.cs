using System;
using System.Collections.Generic;
using System.Globalization;
using ConceptONE.Infrastructure;

namespace IDALibrary.DEMI.Utilities
{

    public class DataTypeParser
    {
        private static List<string> _DateFormats = new List<string>() {
            "MM/dd/yy", "M/d/yy",
            "MM/dd/yyyy", "M/d/yyyy",
            "yyyy-MM-dd", "yyyy-M-d",
            "yyyy.MM.dd", "yyyy.M.d",
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyyMMdd"
        };

        public Type GetType(IList<string> values)
        {
            DataType type = DataType.String;

            if (values.Count > 0)
            {
                DataType previousType = GetDataValue(values[0]);

                foreach (string value in values)
                {
                    type = GetDataValue(value);

                    //Logger.LogActivity("{0} parsed as {1}", value, type);

                    //As soon as a difference is encountered use string
                    if (type == previousType)
                        previousType = type;
                    else
                        return typeof(string); 
                }
            }

            if (type == DataType.Date)
                return typeof(DateTime);
            else if (type == DataType.Numeric)
                return typeof(decimal);
            else
                return typeof(string);
        }

        /// <summary>
        /// 1) Do not use IsNumeric() first
        ///    Dates such as 20160101 would be incorrectly parsed as numbers
        /// 2) Do not use TryParse within IsDate()
        ///    Numbers between 01.NNNN and 12.NNNN are incorrectly parsed as dates
        /// </summary>
        private DataType GetDataValue(string value)
        {
            if (IsDate(value))
                return DataType.Date;
            else if (IsNumeric(value))
                return DataType.Numeric;
            else
                return DataType.String;
        }

        private bool IsNumeric(string value)
        {
            int intergerVal;
            decimal decimalVal;

            if (Int32.TryParse(value, out intergerVal))
                return true;
            else if (Decimal.TryParse(value, out decimalVal))
                return true;
            else
                return false;
        }

        private bool IsDate(string value)
        {
            DateTime date = GetAsDateTime(value);
            bool result = (date > DateTime.MinValue);

            return result;
        }

        public static DateTime GetAsDateTime(string value)
        {
            DateTime date = DateTime.MinValue;

            foreach (string format in _DateFormats)
            {
                if (DateTime.TryParseExact(value, format, 
                    DateTimeFormatInfo.InvariantInfo, 
                    DateTimeStyles.AllowWhiteSpaces, 
                    out date))
                    return date;
            }

            return date;
        }

    }
}
