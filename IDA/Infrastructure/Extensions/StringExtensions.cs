using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class StringExtensions
    {

        #region Fields

        private static string[][] TimeZones = new string[][] {
            new string[] {"ACDT", "-1030", "Australian Central Daylight"},
            new string[] {"ACST", "-0930", "Australian Central Standard"},
            new string[] {"ADT", "+0300", "(US) Atlantic Daylight"},
            new string[] {"AEDT", "-1100", "Australian East Daylight"},
            new string[] {"AEST", "-1000", "Australian East Standard"},
            new string[] {"AHDT", "+0900", ""},
            new string[] {"AHST", "+1000", ""},
            new string[] {"AST", "+0400", "(US) Atlantic Standard"},
            new string[] {"AT", "+0200", "Azores"},
            new string[] {"AWDT", "-0900", "Australian West Daylight"},
            new string[] {"AWST", "-0800", "Australian West Standard"},
            new string[] {"BAT", "-0300", "Bhagdad"},
            new string[] {"BDST", "-0200", "British Double Summer"},
            new string[] {"BET", "+1100", "Bering Standard"},
            new string[] {"BST", "+0300", "Brazil Standard"},
            new string[] {"BT", "-0300", "Baghdad"},
            new string[] {"BZT2", "+0300", "Brazil Zone 2"},
            new string[] {"CADT", "-1030", "Central Australian Daylight"},
            new string[] {"CAST", "-0930", "Central Australian Standard"},
            new string[] {"CAT", "+1000", "Central Alaska"},
            new string[] {"CCT", "-0800", "China Coast"},
            new string[] {"CDT", "+0500", "(US) Central Daylight"},
            new string[] {"CED", "-0200", "Central European Daylight"},
            new string[] {"CET", "-0100", "Central European"},
            new string[] {"CST", "+0600", "(US) Central Standard"},
            new string[] {"EAST", "-1000", "Eastern Australian Standard"},
            new string[] {"EDT", "+0400", "(US) Eastern Daylight"},
            new string[] {"EED", "-0300", "Eastern European Daylight"},
            new string[] {"EET", "-0200", "Eastern Europe"},
            new string[] {"EEST", "-0300", "Eastern Europe Summer"},
            new string[] {"EST", "+0500", "(US) Eastern Standard"},
            new string[] {"FST", "-0200", "French Summer"},
            new string[] {"FWT", "-0100", "French Winter"},
            new string[] {"GMT", "+0000", "Greenwich Mean"},
            new string[] {"GST", "-1000", "Guam Standard"},
            new string[] {"HDT", "+0900", "Hawaii Daylight"},
            new string[] {"HST", "+1000", "Hawaii Standard"},
            new string[] {"IDLE", "-1200", "Internation Date Line East"},
            new string[] {"IDLW", "+1200", "Internation Date Line West"},
            new string[] {"IST", "-0530", "Indian Standard"},
            new string[] {"IT", "-0330", "Iran"},
            new string[] {"JST", "-0900", "Japan Standard"},
            new string[] {"JT", "-0700", "Java"},
            new string[] {"MDT", "+0600", "(US) Mountain Daylight"},
            new string[] {"MED", "-0200", "Middle European Daylight"},
            new string[] {"MET", "-0100", "Middle European"},
            new string[] {"MEST", "-0200", "Middle European Summer"},
            new string[] {"MEWT", "-0100", "Middle European Winter"},
            new string[] {"MST", "+0700", "(US) Mountain Standard"},
            new string[] {"MT", "-0800", "Moluccas"},
            new string[] {"NDT", "+0230", "Newfoundland Daylight"},
            new string[] {"NFT", "+0330", "Newfoundland"},
            new string[] {"NT", "+1100", "Nome"},
            new string[] {"NST", "-0630", "North Sumatra"},
            new string[] {"NZ", "-1100", "New Zealand "},
            new string[] {"NZST", "-1200", "New Zealand Standard"},
            new string[] {"NZDT", "-1300", "New Zealand Daylight"},
            new string[] {"NZT", "-1200", "New Zealand"},
            new string[] {"PDT", "+0700", "(US) Pacific Daylight"},
            new string[] {"PST", "+0800", "(US) Pacific Standard"},
            new string[] {"ROK", "-0900", "Republic of Korea"},
            new string[] {"SAD", "-1000", "South Australia Daylight"},
            new string[] {"SAST", "-0900", "South Australia Standard"},
            new string[] {"SAT", "-0900", "South Australia Standard"},
            new string[] {"SDT", "-1000", "South Australia Daylight"},
            new string[] {"SST", "-0200", "Swedish Summer"},
            new string[] {"SWT", "-0100", "Swedish Winter"},
            new string[] {"USZ3", "-0400", "USSR Zone 3"},
            new string[] {"USZ4", "-0500", "USSR Zone 4"},
            new string[] {"USZ5", "-0600", "USSR Zone 5"},
            new string[] {"USZ6", "-0700", "USSR Zone 6"},
            new string[] {"UT", "+0000", "Universal Coordinated"},
            new string[] {"UTC", "+0000", "Universal Coordinated"},
            new string[] {"UZ10", "-1100", "USSR Zone 10"},
            new string[] {"WAT", "+0100", "West Africa"},
            new string[] {"WET", "+0000", "West European"},
            new string[] {"WST", "-0800", "West Australian Standard"},
            new string[] {"YDT", "+0800", "Yukon Daylight"},
            new string[] {"YST", "+0900", "Yukon Standard"},
            new string[] {"ZP4", "-0400", "USSR Zone 3"},
            new string[] {"ZP5", "-0500", "USSR Zone 4"},
            new string[] {"ZP6", "-0600", "USSR Zone 5"}
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Used to parse strings encoded via JSON.stringify
        /// </summary>
        public static List<string> GetAsStringList(this string jsonString)
        {
            List<string> result = new List<string>();
            string stringValues = jsonString.RemoveBrackets();
            string[] stringArray = stringValues.Split(',');

            if (!string.IsNullOrEmpty(stringValues))
            {
                for (int i = 0; i < stringArray.Length; i++)
                    stringArray[i] = stringArray[i].Trim('\"');
                result = stringArray.ToList();
            }

            return result;
        }

        public static List<int> GetAsIntegerList(this string input)
        {
            List<string> stringList = input.GetAsStringList();
            List<int> result = stringList.Select(s => int.Parse(s)).ToList();
            return result;
        }

        public static DateTime? ToDateTime(this string input)
        {
            DateTime result;
                                
            // Parse date with TimeZone: example: "Wed May 8 00:00:00 EDT 2013" OR "Wed May 24 00:00:00 EDT 2013"
            string[] timeZone = GetTimeZone(input).FirstOrDefault();

            input = input.Replace(timeZone[0], timeZone[1]);

            if (DateTime.TryParseExact(input, "ddd MMM dd HH:mm:ss K yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result)) return result;
            if (DateTime.TryParseExact(input, "ddd MMM d HH:mm:ss K yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result)) return result;
                        
            // Handle javascript date format.
            if (input.Contains('-'))
            {
                input = input.Substring(0, input.LastIndexOf("-"));
                input = input.Substring(0, input.Length - 5);

                if (DateTime.TryParseExact(input, "ddd MMM dd yyyy HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out result)) return result;
            }

            // Parse date based on current culture and settings
            if (DateTime.TryParse(input, out result)) return result;

            return null;
        }

        public static string GetPart(this string whole, char separator, int index)
        {
            string result = string.Empty;
            string[] parts = whole.Split(separator);

            if (index < parts.Length)
            {
                result = parts[index];
            }

            return result;
        }

        public static bool IsNumeric(this string input)
        {
            long dummyLong = 0;
            double dummyDouble = 0;

            bool isLong = long.TryParse(input, out dummyLong);
            bool isDouble = double.TryParse(input, out dummyDouble);
            bool result = isLong || isDouble;

            return result;
        }

        public static int ToInt(this string input, int defaultValue)
        {
            int result = defaultValue;

            if (int.TryParse(input, out result))
                return result;
            else
                return defaultValue;
        }

        public static double ToDouble(this string input, double defaultValue)
        {
            double result = defaultValue;

            if (double.TryParse(input, out result))
                return result;
            else
                return defaultValue;
        }

        public static decimal ToDecimal(this string input, decimal defaultValue)
        {
            decimal result = defaultValue;

            if (decimal.TryParse(input, out result))
                return result;
            else
                return defaultValue;
        }

        public static bool ToBoolean(this string input)
        {
            input = input.ToLower();
            bool result = (input == "y" || input == "yes" || input == "true" || input == "1");

            return result;
        }

        public static string AddBrackets(this string input)
        {
            string result = string.Format("[{0}]", input);
            return result;
        }

        public static string AddDoubleQuotes(this string input)
        {
            string result = string.Format("\"{0}\"", input);
            return result;
        }

        public static string AddSingleQuotes(this string input)
        {
            string result = string.Format("'{0}'", input);
            return result;
        }

        public static string RemoveBrackets(this string input)
        {
            string result = input.Trim(new char[] { '[', ']' });
            return result;
        }

        public static string RemoveSpaceAfterComma(this string stringList)
        {
            string result = stringList.Replace(", ", ",");
            return result;
        }

        public static List<string> ToStringList(this string list, char separator)
        {
            List<string> result = new List<string>();

            string[] parts = list.Split(separator);

            foreach (string part in parts)
                result.Add(part.Trim());

            return result;
        }

        public static List<int> ToIntList(this string list, char separator)
        {
            List<int> result = new List<int>();

            string[] parts = list.Split(separator);

            foreach (string part in parts)
                if (!string.IsNullOrEmpty(part))
                    result.Add(part.ToInt(0));

            return result;
        }

        public static string ToDbSafeString(this string original)
        {
            string result = original.Trim();

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Replace("''", "'");
                result = result.Replace("'", "''");
            }

            return result;
        }

        public static bool IgnoreCaseEquals(this string original, string compare)
        {
            bool result = original.Equals(compare, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        public static string ToShortenedString(this string original, int lenght, string ending)
        {
            int effectiveLength = lenght - ending.Length;
            string result = original;

            if (original.Length > lenght)
                result = original.Substring(0, effectiveLength) + ending;

            return result;
        }

        public static string ToShortenedString(this string original, int trimLength)
        {
            if (trimLength == 0)
                return original;
            else if (trimLength >= original.Length)
                return "";
            else
                return original.Substring(0, original.Length - trimLength);
        }

        public static bool Contains(this string source, string toCheck, StringComparison comparisonType)
        {
            bool result = source.IndexOf(toCheck, comparisonType) >= 0;
            return result;
        }

        public static bool StartsWithAndNotWith(this string source, string prefix, string notPrefix)
        {
            if (string.IsNullOrEmpty(notPrefix))
                return source.StartsWith(prefix);
            else
                return source.StartsWith(prefix) && !source.StartsWith(notPrefix);
        }

        public static string GetStringWithSpaces(this string stringNoSpaces)
        {
            char[] characters = stringNoSpaces.ToCharArray();
            StringBuilder result = new StringBuilder();

            if (stringNoSpaces.Length > 2)
            {
                char first = characters[0];
                result.Append(first);

                for (int index = 1; index < characters.Length - 1; index++)
                {
                    char current = characters[index];
                    char previous = characters[index - 1];
                    char next = characters[index + 1];

                    if (index == 0 || index == characters.Length)
                        result.Append(current);
                    else if (IsUpperCase(current) && IsNotAlphaWithSpace(previous))
                        //(A
                        result.Append(current);
                    else if (IsUpperCase(current) && IsLowerCase(previous))
                        //aA
                        result.AppendFormat(" {0}", current);
                    else if (IsUpperCase(current) && IsLowerCase(next))
                        //Aa
                        result.AppendFormat(" {0}", current);
                    else if (IsNotAlphaWithoutSpace(current))
                        //'
                        result.Append(current);
                    else if (IsNotAlphaWithSpace(current) && current != ' ')
                        //(
                        result.AppendFormat(" {0}", current);
                    else
                        result.Append(current);
                }

                char last = characters[characters.Length - 1];
                result.Append(last);
            }
            else
            {
                result.Append(stringNoSpaces);
            }

            return result.ToString().TrimStart().Replace("  ", " ");
        }

        public static void AppendQuotedString(this StringBuilder sb, string field)
        {
            sb.AppendFormat("\"{0}\"", field);
            sb.AppendLine();
        }

        public static void AppendQuotedStringList<T>(this StringBuilder sb, List<T> fields)
        {
            foreach (T field in fields)
                sb.AppendFormat("\"{0}\",", field);

            if (fields.Count > 0)
                sb.Remove(sb.Length - 1, 1);

            sb.AppendLine();
        }

        public static string ToCleanJson(this string input)
        {
            string result = input.Replace("\\", "");
            result = result.Substring(1, result.Length - 2);

            if (!result.StartsWith("{"))
                result = "{" + result;

            if (!result.EndsWith("}"))
                result = result + "}";

            return result;
        }

        public static string ToTitleCase(this string input)
        {
            if (input.Length == 0)
                return "";
            else if (input.Length == 1)
                return input.ToUpper();
            else 
                return input.Substring(0,1).ToUpper() + input.Substring(1).ToLower();
        }

        public static StringBuilder TrimEndChars(this StringBuilder stringBuilder, int charCountTotrim)
        {
            if (charCountTotrim > stringBuilder.Length)
                return stringBuilder;
            else
                return stringBuilder.Remove(stringBuilder.Length - charCountTotrim, charCountTotrim);
        }

        public static T ToEnum<T>(this string s) where T : struct
        {
            T enumValue;

            if (Enum.TryParse(s, out enumValue))
                return enumValue;
            else
                return default(T);
        }

        public static bool IsNumeric(this char character)
        {
            int ascii = (int)character;

            if ((ascii >= (int)ConsoleKey.D0 && ascii <= (int)ConsoleKey.D9) || character == '.' || character == ',')
                return true;
            else
                return false;
        }

        public static Dictionary<string, string> ToDictionary(this string collectionString, char itemSeparator, char keyValueSeparator)
        {
            List<string> collectionList = collectionString.Trim().Split(itemSeparator).ToList();

            Dictionary<string, string> result = collectionList.
                Where(i => !String.IsNullOrEmpty(i.Trim())).
                ToDictionary(
                    k => k.Substring(0, k.IndexOf(keyValueSeparator)).Trim(),
                    v => v.Substring(v.IndexOf('=') + 1).Trim()
                );

            return result;
        }

        public static List<string> GetSubstringList(this string fullString, string startMarker, string endMarker, bool includeMarkers)
        {
            List<string> result = new List<string>();

            string lineToParse = fullString;

            while (lineToParse.Contains(startMarker))
            {
                int start = lineToParse.IndexOf(startMarker);
                int end = lineToParse.IndexOf(endMarker);

                if (includeMarkers)
                {
                    result.Add(lineToParse.Substring(start, end - start + 1));
                    lineToParse = lineToParse.Substring(end);
                }
                else
                {
                    result.Add(lineToParse.Substring(start + 1, end - start - 1));
                    lineToParse = lineToParse.Substring(end + 1);
                }
            }

            return result;
        }

        #endregion

        #region Private Methods

        private static IEnumerable<string[]> GetTimeZone(string input)
        {
            foreach (var item in TimeZones)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(input, item[0])) yield return item;
            }
        }

        private static bool IsDigit(char character)
        {
            int asciiCode = (int)character;
            bool result = (asciiCode >= 48) && (asciiCode <= 57);

            return result;
        }

        private static bool IsUpperCase(char character)
        {
            int asciiCode = (int)character;
            bool result = (asciiCode >= 65) && (asciiCode <= 90);

            return result;
        }

        private static bool IsLowerCase(char character)
        {
            int asciiCode = (int)character;
            bool result = (asciiCode >= 97) && (asciiCode <= 122);

            return result;
        }

        private static bool IsNotAlphaWithSpace(char character)
        {
            bool result = !IsUpperCase(character) && !IsLowerCase(character) && !IsDigit(character);

            return result;
        }

        private static bool IsNotAlphaWithoutSpace(char character)
        {
            char[] symbolsNoSpace = new char[] { '\'' };
            bool result = symbolsNoSpace.Contains(character);

            return result;
        }

        #endregion

    }
}
