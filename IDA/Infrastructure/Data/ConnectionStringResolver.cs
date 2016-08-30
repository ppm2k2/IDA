using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using ConceptONE.Infrastructure.Encryption;
using ConceptONE.Infrastructure.Extensions;

namespace ConceptONE.Infrastructure.Data
{
    public class ConnectionStringResolver
    {
        public static string GetConnectionString(string name, string resourcesPath)
        {
            ConnectionStringSettings connString = ConfigurationManager.ConnectionStrings[name];
            Assembly assembly = Assembly.GetCallingAssembly();

            if (connString == null || String.IsNullOrEmpty(connString.ConnectionString))
            {
                Logger.LogActivity("Invalid connection string: {0}", name);
                return "";
            }

            string template = connString.ConnectionString;
            string userId = GetUserId(template);
            string fullPath = String.Format("{0}.{1}.txt", resourcesPath, userId);
            string password = GetPassword(assembly, fullPath);
            string result = String.Format(template, password);

            if (String.IsNullOrEmpty(userId))
                Logger.LogActivity("User ID parameter not found");
            else if (String.IsNullOrEmpty(resourcesPath))
                Logger.LogActivity("Resource Path not found");
            else if (String.IsNullOrEmpty(password))
                Logger.LogActivity("Password not found");

            return result;
        }

        private static string GetPassword(Assembly assembly, string fullPath)
        {
            using (Stream fileStream = assembly.GetManifestResourceStream(fullPath))
            {
                string result = "";

                if (fileStream == null)
                {
                    Logger.LogActivity("Password file resource not found (Expected: {0})", fullPath);
                }
                else
                {
                    StreamReader fileStreamReader = new StreamReader(fileStream);
                    string line = fileStreamReader.ReadLine();
                    result = PasswordEncrypter.Decrypt(line);
                }

                return result;
            }
        }

        private static string GetUserId(string connString)
        {
            Dictionary<string, string> parameters = connString.ToDictionary(';', '=');


            foreach (string key in parameters.Keys)
            {
                if (key.ToLower() == "user id")
                    return parameters[key];
            }

            return "";
        }
    }
}
