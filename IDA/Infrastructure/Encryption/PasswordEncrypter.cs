namespace ConceptONE.Infrastructure.Encryption
{
    /// <summary>
    /// Phase 1: for now instead of encryption we are simply encoding the password using Base64
    /// Phase 2: the next step will be to implement proper encryption
    /// 
    /// Note: encryption != encoding!
    /// </summary>
    public class PasswordEncrypter
    {
        public static string Encrypt(string plainTextPassword)
        {
            string result = Base64Encoder.Encode(plainTextPassword);
            return result;
        }

        public static string Decrypt(string encodedPassword)
        {
            string result = Base64Encoder.Decode(encodedPassword);
            return result;
        }
    }
}
