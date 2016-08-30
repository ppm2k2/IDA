using System;
using System.Configuration;
using System.IO;

namespace IDADataAccess
{
    /// <summary>
    /// This class is duplicated here from IDALibrary to avoid a circular reference.
    /// We need a better solution to share the code somewhere else
    /// </summary>
    public class Settings
    {
        public static string AppType
        {
            get
            {
                string result = ConfigurationManager.AppSettings["AppType"];
                return result;
            }
        }

        public static string DemiConnectionString
        {
            get
            {
                string result = ConfigurationManager.ConnectionStrings["DemiConnection"].ConnectionString;

                return result;
            }
        }

        public static string InputFilesPath
        {
            get
            {
                string subfolders = String.Format("{0:00}\\{1:00}", DateTime.Now.Year, DateTime.Now.Month);
                string result = Path.Combine(ConfigurationManager.AppSettings["InputFilesPath-DEMI"], subfolders);

                return result;
            }
        }

    }
}
