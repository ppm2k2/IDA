using System;
using System.Text;

namespace ConceptONE.Infrastructure.Encryption
{
    public class Base64Encoder
    {
        public static string Encode(string plainText)
        {
            if (String.IsNullOrEmpty(plainText))
                return "";

            byte[] encodedBytes = Encoding.ASCII.GetBytes(plainText);
            string result = Convert.ToBase64String(encodedBytes);

            return result;
        }

        public static string Decode(string encodedString)
        {
            if (String.IsNullOrEmpty(encodedString))
                return "";

            byte[] decodedBytes = Convert.FromBase64String(encodedString);
            string result = Encoding.ASCII.GetString(decodedBytes);

            return result;
        }
    }
}
