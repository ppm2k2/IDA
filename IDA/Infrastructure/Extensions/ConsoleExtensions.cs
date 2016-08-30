using System;
using System.Data;
using System.Text;

namespace ConceptONE.Infrastructure.Extensions
{
    //The Console class is static and cannot be extended
    //but conceptually this code is used to extend the Console
    public class ConsoleExtensions
    {

        public static void WriteDataTable(DataTable table, bool writeTableName = false, int colWidth = 20)
        {
            string cellFormat = "{0,-" + colWidth + "} ";
            int separatorWidth = ((colWidth + 1) * table.Columns.Count);
            string separator = new string('-', separatorWidth);

            if (writeTableName)
                Console.WriteLine(table.TableName);

            //Header
            Console.WriteLine(separator);
            for (int col = 0; col < table.Columns.Count; col++)
            {
                Console.Write(string.Format(cellFormat, table.Columns[col].ToString().ToUpper()));
            }
            Console.Write("\n" + separator);

            foreach (DataRow row in table.Rows)
            {

                //Data
                Console.Write("\n");
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    Console.Write(string.Format(cellFormat, row[col].ToString()));
                }
            }

            //Write footer
            Console.WriteLine("\n" + separator);
        }

        public static void LogDataTable(DataTable table, bool writeTableName = false, int colWidth = 20)
        {
            string cellFormat = "{0,-" + colWidth + "} ";
            int separatorWidth = ((colWidth + 1) * table.Columns.Count);
            string separator = new string('-', separatorWidth);
            StringBuilder output = new StringBuilder();

            if (writeTableName)
                output.AppendLine(table.TableName);

            //Header
            output.AppendLine(separator);
            for (int col = 0; col < table.Columns.Count; col++)
            {
                output.Append(string.Format(cellFormat, table.Columns[col].ToString().ToUpper()));
            }
            output.Append("\n" + separator);

            foreach (DataRow row in table.Rows)
            {

                //Data
                output.Append("\n");
                for (int col = 0; col < table.Columns.Count; col++)
                {
                    output.Append(string.Format(cellFormat, row[col].ToString()));
                }
            }

            //Write footer
            output.Append("\n" + separator);

            Logger.LogActivity(output.ToString());
        }

    }
}
