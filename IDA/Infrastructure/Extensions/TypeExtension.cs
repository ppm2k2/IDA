using System;
using System.Collections.Generic;
using System.Reflection;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class TypeExtension
    {

        #region Set Value

        public static void SetValue(this Type input, string propertyName, object value, ref object record, bool flagTypeConvertion = true)
        {
            PropertyInfo pi = input.GetProperty(propertyName);
            if (flagTypeConvertion)
            {
                Type propertyType = pi.PropertyType;
                Type targetType = propertyType.IsNullableType() ? Nullable.GetUnderlyingType(propertyType) : propertyType;
                value = value == null ? null : Convert.ChangeType(value, targetType);
            }
            try
            {
                pi.SetValue(record, value, null);
            }
            catch
            {
                throw new ArgumentException(string.Format("Invalid value found for this Property Name {0} - Value {1} - Property Type {2}", propertyName, value.ToString(), pi.PropertyType.ToString()));
            }
        }

        public static bool IsNullableType(this Type type)
        {
            bool result = type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
            return result;
        }

        #endregion                          //Set Value 

        #region Get Value

        public static string GetValue(this Type input, string propertyName, object value)
        {
            PropertyInfo propertyInfo = input.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);
            string result = propertyInfo.GetValue(value) == null ? null : propertyInfo.GetValue(value).ToString();

            return result;
        }

        public static string GetValue(this Type input, string propertyName, object value, bool ParseEmptyStrings)
        {

            string result;

            PropertyInfo propertyInfo = input.GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase);

            if (propertyInfo.GetValue(value) == null)
            {
                result = null;
            }
            else if (ParseEmptyStrings == true && (propertyInfo.GetValue(value).ToString() == "" || propertyInfo.GetValue(value).ToString() == "NULL"))
            {
                propertyInfo.SetValue(value, null);
                result = null;
            }
            else
            {
                result = propertyInfo.GetValue(value).ToString();
            }

            return result;
        }

        #endregion                          //Get Value

        #region Merge records

        public static void MergeRecords(this Type input, IList<string> visibleColumns, object existingRecord, ref object newRecord)
        {

            foreach (PropertyInfo propertyInfo in input.GetProperties())
            {
                if (visibleColumns.Contains(propertyInfo.Name)) continue;
                string valueER = input.GetValue(propertyInfo.Name, existingRecord);
                if (!string.IsNullOrEmpty(valueER))
                {
                    string valueNR = input.GetValue(propertyInfo.Name, newRecord);
                    if (string.IsNullOrEmpty(valueNR))
                    {
                        input.SetValue(propertyInfo.Name, valueER, ref newRecord);
                    }
                }
            }
        }

        #endregion

        public static bool IsDecimal(this Type type)
        {
            if (type.Name.EndsWith("Decimal"))
                return true;
            else if (type.IsNullableType())
                return type.UnderlyingSystemType.FullName.IndexOf("Decimal") >= 0;
            else
                return false;
        }

    }
}
