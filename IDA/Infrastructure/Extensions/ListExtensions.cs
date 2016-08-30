using System.Collections.Generic;
using System.Text;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class ListExtensions
    {
        public static string ToCommaSeparatedList(this List<string> stringList, string format = "")
        {
            StringBuilder result = new StringBuilder(" ");

            foreach (string stringItem in stringList)
                if (string.IsNullOrEmpty(stringItem))
                    result.Append(", ");
                else if (string.IsNullOrEmpty(format))
                    result.AppendFormat("{0}, ", stringItem.Trim());
                else 
                    result.AppendFormat(format + ", ", stringItem.Trim());

            return result.TrimEndChars(2).ToString().Trim(); 
        }

        public static string ToCharSeparatedList(this List<string> stringList, char separator)
        {
            string result = "";

            foreach (string stringItem in stringList)
                if (string.IsNullOrEmpty(stringItem))
                    result += separator ;
                else
                    result += stringItem.Trim() + separator ;

            return result.Trim().Trim(separator);
        }

        public static string ToCommaSeparatedList(this List<int> stringList)
        {
            string result = " ";

            foreach (int intItem in stringList)
                result += intItem.ToString() + ", ";

            return result.Trim().Trim(',');
        }

        public static string ToCharSeparatedList(this List<int> stringList, char separator)
        {
            string result = "";

            foreach (int intItem in stringList)
                result += intItem.ToString() + separator;

            return result.Trim().Trim(separator);
        }

        public static string ToCommaSeparatedList(this List<double> stringList)
        {
            string result = " ";

            foreach (double intItem in stringList)
                result += intItem.ToString() + ", ";

            return result.Trim().Trim(',');
        }

        public static string ToCommaSeparatedListWithSquareBrackets(this List<string> stringList)
        {
            string result = " ";

            foreach (string stringItem in stringList)
            {
                result += string.Format("[{0}], ", stringItem.Trim());
            }

            return result.Trim().Trim(',');
        }

        public static string ToCommaSeparatedListWithSingleQuotes(this List<string> stringList)
        {
            string result = " ";

            foreach (string stringItem in stringList)
            {
                result += string.Format("'{0}', ", stringItem.Trim());
            }

            return result.Trim().Trim(',');
        }

        public static string GetItem(this List<string> list, int index)
        {
            if (list.Count > index)
                return list[index];
            else
                return string.Empty;
        }

        public static void AddFormat(this List<string> list, string format, params object[] parameters)
        {
            string item = string.Format(format, parameters);
            list.Add(item);
        }

        public static string GetPart(this List<string> list, int index)
        { 
            if (list.Count > index)
                return list[index];
            else
                return "";
        }

    }
}
