using System.Data.Common;
using System.Data.SqlClient;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class DataReaderExtensions
    {

        public static string ToStringOrNull(this DbDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(index))
                return null;
            else
                return reader[columnName].ToString();
        }

        public static string ToStringOrNullLiteral(this DbDataReader reader, string columnName)
        {
            int index = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(index))
                return "NULL";
            else
                return reader[columnName].ToString();
        }

    }
}
