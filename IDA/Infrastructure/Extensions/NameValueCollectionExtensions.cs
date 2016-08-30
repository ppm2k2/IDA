using System;
using System.Collections.Specialized;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class NameValueCollectionExtensions
    {

        public static string[] GetAsStringArray(this NameValueCollection collection, string setting)
        {
            string settingValue = collection[setting];
            string[] result = new string[] { };

            if (!String.IsNullOrEmpty(settingValue))
                result = settingValue.Split(',');

            return result;
        }

        public static bool GetAsBoolean(this NameValueCollection collection, string setting)
        {
            string settingValue = collection[setting];
            bool result = settingValue.ToBoolean();

            return result;
        }

    }
}
