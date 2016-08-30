using System.Collections.Generic;
using System.Text;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class DictionaryExtensions
    {

        public static void AddDictionary(this Dictionary<string, string> dictionary, Dictionary<string, string> itemsToAdd)
        {
            foreach (string key in itemsToAdd.Keys)
                dictionary.Add(key, itemsToAdd[key]);
        }

        public static void AddDictionary(this Dictionary<int, string> dictionary, Dictionary<int, string> itemsToAdd)
        {
            foreach (int key in itemsToAdd.Keys)
                dictionary.Add(key, itemsToAdd[key]);
        }

        public static void AddDictionary<K, V>(this Dictionary<K, V> dictionary, Dictionary<K, V> itemsToAdd)
        {
            foreach (K key in itemsToAdd.Keys)
                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, itemsToAdd[key]);
        }

        public static V GetValue<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return defaultValue;
        }

        public static string GetValue(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else
                return null;
        }

        public static List<string> ToSetList<K, V>(this Dictionary<K, V> dictionary, bool singleQuote)
        {
            List<string> result = new List<string>();
            string format = singleQuote ? "{0}='{1}'" : "{0}={1}";

            foreach (K key in dictionary.Keys)
                result.AddFormat(format, key, dictionary[key]);

            return result;
        }

        public static string ToAmpersandSeparatedList(this Dictionary<string, string> dictionary)
        {
            StringBuilder result = new StringBuilder();

            foreach (string key in dictionary.Keys)
                result.AppendFormat("{0}={1}&", key, dictionary[key]);

            result.Remove(result.Length - 1, 1);

            return result.ToString();
        }

        public static void RemoveAndAdd<K, V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);

            dictionary.Add(key, value);
        }
    }
}
