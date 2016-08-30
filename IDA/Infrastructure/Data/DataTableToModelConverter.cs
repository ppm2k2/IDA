using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace ConceptONE.Infrastructure.Data
{
    public class DataTableToModelConverter
    {
        private Assembly _callingAssembly;


        public DataTableToModelConverter(){}

        public DataTableToModelConverter(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
                _callingAssembly = Assembly.LoadFrom(assemblyPath);
            else
                Logger.LogActivity("Assembly not found: {0}", assemblyPath);
        }

        public List<T> GetDataTableAsCustomObjectList<T>(Type type, DataTable table)
        {
            List<T> result = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                T customObject = GetDataRowAsCustomObject<T>(type, row);
                result.Add(customObject);
            }

            return result;
        }

        public List<T> GetDataTableAsCustomObjectList<T>(string className, DataTable table)
        {
            List<T> result = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                T customObject = GetDataRowAsCustomObject<T>(className, row);
                result.Add(customObject);
            }
                        
            return result;
        }

        private T GetDataRowAsCustomObject<T>(string className, DataRow row)
        {
            object instance = GetClassInstance(className);

            foreach (DataColumn column in row.Table.Columns)
            {
                string columnName = column.ColumnName;
                PropertyInfo property = instance.GetType().GetProperty(columnName);

                if (property != null)
                {
                    if (row.IsNull(columnName))
                        property.SetValue(instance, null);
                    else
                        property.SetValue(instance, row[columnName]);
                }
            }

            T result = (T)instance;

            return result;
        }

        private T GetDataRowAsCustomObject<T>(Type type, DataRow row)
        {
            object instance = Activator.CreateInstance(type);

            foreach (DataColumn column in row.Table.Columns)
            {
                string columnName = column.ColumnName;
                PropertyInfo property = instance.GetType().GetProperty(columnName);

                if (property != null)
                {
                    if (row.IsNull(columnName))
                        property.SetValue(instance, null);
                    else
                        property.SetValue(instance, row[columnName]);
                }
            }

            T result = (T)instance;

            return result;
        }

        private object GetClassInstance(string className)
        {
            object result = null;

            foreach (Type type in _callingAssembly.GetTypes())
            {
                if (type.IsClass && type.FullName.EndsWith(className))
                {
                    result = Activator.CreateInstance(type);
                    break;
                }
            }

            return result;
        }

    }
}
