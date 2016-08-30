using System;
using System.Threading;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace ConceptONE.Infrastructure
{
    public class Logger
    {
        #region Constants

        private const string LOG_FILE = "{0}{1}.csv";
        private const string HEADER = "Date,Time,Log,ThreadID,ThreadName";
        private const string LOG_ENTRY = "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"";

        private const int MAX_LENGTH = 10000;

        #endregion

        #region Private Fields

        private static readonly string _AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
        private static readonly string _Year = DateTime.Now.ToString("yyyy");

        private static readonly object Locker = new object();

        private static List<string> _CachedLogs = new List<string>();
        private static bool _CachingEnabled { get; set; }
        private static string _LogPath = null;

        #endregion

        #region Public Properties

        public static string CustomName { get; set; }

        /// <summary>
        /// If a custom LogPath is set: use that one
        /// If one is found in the config file: use that one
        /// If none is found: use the bin folder
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (!String.IsNullOrEmpty(_LogPath))
                    return _LogPath;
                else if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["LogPath"]))
                    return ConfigurationManager.AppSettings["LogPath"];
                else
                    return Path.Combine(_AppPath, "Logs");
            }
            set
            {
                _LogPath = value;
            }
        }

        #endregion

        #region Public Methods

        public static void EnableCaching()
        {
            _CachingEnabled = true;
        }

        public static void DisableCaching()
        {
            _CachingEnabled = false;
        }

        public static void LogActivity(List<string> logs)
        {
            foreach (string log in logs)
                LogActivity(log);
        }

        public static void LogActivity(string logEntry, params object[] arguments)
        {
            string combinedLogEntry = string.Format(logEntry, arguments);
            LogActivity(combinedLogEntry);
        }

        public static void LogActivity(string logEntry)
        {
            string fullLogEntry = string.Empty;
            string fullPath = GetFullLogPath();
            logEntry = (logEntry.Length > MAX_LENGTH) ? logEntry.Substring(0, MAX_LENGTH) : logEntry;

            try
            {
                PopulateLogHeader(ref fullLogEntry, fullPath);
                PopulateLogEntry(ref fullLogEntry, logEntry);
                Console.WriteLine(logEntry);

                if (_CachingEnabled)
                    _CachedLogs.Add(fullLogEntry);
                else
                {
                    lock (Locker)
                    {
                        using (StreamWriter logWriter = new StreamWriter(fullPath, true))
                        {
                            logWriter.WriteLine(fullLogEntry, true);
                            logWriter.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("---------------------------");
                Console.WriteLine("Log error: " + ex.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        public static void LogException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            LogActivity("Exception: " + ex.ToString());

            if (ex.InnerException != null)
            {
                LogActivity("InnerException: " + ex.InnerException.ToString());
                if (ex.InnerException.InnerException != null)
                {
                    LogActivity("InnerException.InnerException: " + ex.InnerException.InnerException.ToString());
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Flush()
        {
            string fullPath = GetFullLogPath();

            while (LogFileInUse(fullPath))
                Thread.Sleep(1);

            lock (Locker)
            {
                bool fileExists = File.Exists(fullPath);

                using (StreamWriter logWriter = new StreamWriter(fullPath, true))
                {
                    if (!fileExists)
                        logWriter.WriteLine(HEADER);

                    foreach (string cachedLog in _CachedLogs)
                        logWriter.WriteLine(cachedLog, true);

                    logWriter.Close();
                }

                _CachedLogs.Clear();
            }

        }

        public static void Unlock()
        {
            try
            {
                string logPath = GetFullLogPath();
                string renamedLogPath = logPath + "_RENAMED";

                if (File.Exists(logPath))
                {
                    File.Move(logPath, renamedLogPath);
                    File.Move(renamedLogPath, logPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static string GetLogPath()
        {
            string result = Path.Combine(LogPath, _Year);

            result = result.Replace("file:\\", "");

            if (!Directory.Exists(result))
                Directory.CreateDirectory(result);

            return result;
        }

        public static void CombineWithMainLog(string mainFileNameFormat, string partialFileNamePostFix)
        {
            CombineWithMainLog(mainFileNameFormat, partialFileNamePostFix, DateTime.Today.AddDays(-1));
            CombineWithMainLog(mainFileNameFormat, partialFileNamePostFix, DateTime.Today);
        }

        #endregion

        #region Private Methods

        private static void PopulateLogHeader(ref string fullLogEntry, string fullPath)
        {
            if (!_CachingEnabled && !File.Exists(fullPath))
                fullLogEntry = HEADER + "\n";
        }

        private static void PopulateLogEntry(ref string fullLogEntry, string logEntry)
        {
            string dateStamp = DateTime.Now.ToString("MM/dd/yy");
            string timeStamp = DateTime.Now.ToString("HH:mm:ss");
            string log = logEntry.Replace("\"", "'").Replace("\r", " ").Replace("{", "{{").Replace("}", "}}");
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            string threadName = System.Threading.Thread.CurrentThread.Name;

            fullLogEntry += string.Format(LOG_ENTRY, dateStamp, timeStamp, log, threadID, threadName);
        }

        private static string GetFullLogPath()
        {
            string dateStamp = DateTime.Now.ToString("yyyy.MM.dd");
            string fileName = string.Format(LOG_FILE, dateStamp, CustomName);
            string result = Path.Combine(GetLogPath(), fileName);

            return result;
        }

        private static void CombineWithMainLog(string mainFileNameFormat, string partialFileNamePostFix, DateTime date)
        {
            string partialFileDate = date.ToString("yyyy.MM.dd");
            string partialFile = string.Format(LOG_FILE, partialFileDate, partialFileNamePostFix);
            string partialFilePath = Path.Combine(GetLogPath(), partialFile);

            string mainFileDate = string.Format(mainFileNameFormat, DateTime.Now);
            string mainFile = string.Format(LOG_FILE, mainFileDate, "");
            string mainFilePath = Path.Combine(GetLogPath(), mainFile);

            string line = string.Empty;
            bool sameAsMainFile = partialFilePath.Equals(mainFilePath, StringComparison.InvariantCultureIgnoreCase);
            bool fileExists = File.Exists(partialFilePath);
            int attempts = 0;
            const int MAX_ATTEMPTS = 10;

            try
            {

                //If we pass MAX_ATTEMPTS the method will fail but at least it won't be stuck in an infinite loop
                while (LogFileInUse(mainFilePath) && attempts < MAX_ATTEMPTS)
                {
                    Thread.Sleep(1000);
                    attempts++;
                }

                if (fileExists && !sameAsMainFile)
                {
                    using (StreamReader partial = new StreamReader(partialFilePath))
                    {
                        using (StreamWriter main = new StreamWriter(mainFilePath, true))
                        {
                            if (!partial.EndOfStream)
                                partial.ReadLine();

                            while (!partial.EndOfStream)
                            {
                                line = partial.ReadLine();
                                main.WriteLine(line);
                            }
                        }
                    }

                    File.Delete(partialFilePath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

        }

        private static bool LogFileInUse(string mainFilePath)
        {
            bool result = false;

            try
            {
                using (StreamWriter main = new StreamWriter(mainFilePath, true))
                    result = false;
            }
            catch
            {
                result = true;
            }

            return result;
        }

        #endregion
    }
}