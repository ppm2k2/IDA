using System;
using ConceptONE.Infrastructure.Extensions;
using Microsoft.Practices.Unity;

namespace IDADataAccess.DEMI
{
    internal class DataLookup
    {
        [Dependency]
        public DEMIContext Context { get; internal set; }

        internal string GetLookupResult(LookupItem LookupItem)
        {
            object whereValue = GetWhereValue(LookupItem);
            string result = "NULL";

            if (whereValue != null)
            {
                Console.WriteLine(whereValue);
                result = whereValue.ToString().AddSingleQuotes();
            }

            return result;
        }

        private object GetWhereValue(LookupItem LookupItem)
        {
            const string SQL = "SELECT {0} FROM {1} WHERE {1}_ID={2}";

            string table = Context.SourceDataTable.TableName;
            string sql = String.Format(SQL, LookupItem.WhereValue, table, LookupItem.Id);
            object result = Context.GetScalarValue(sql);

            return result;
        }
    }
}