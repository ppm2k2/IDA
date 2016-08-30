using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using ConceptONE.Infrastructure;
using ConceptONE.Infrastructure.Extensions;
using Tamir.SharpSsh;

namespace ConceptONE.Infrastructure.Net
{
    public class DataExportUploader
    {

        private List<string> _FilesToUpload = new List<string>();
        private string _ExportRootFolder;
        private string _ArchiveFolder;
        private string _TempFolder;
        private string _CompressedArchiveFullPath;
        private string _OutputFilePrefix;

        private int _Port;
        private string _FtpDirectory;
        private string _FtpUrl;
        private string _Password;
        private string _UserName;

        private const int DEFAULT_SFTP_PORT = 22;

        /// <summary>
        /// Loads all paths needed
        /// </summary>
        /// <example>
        /// _FilesToUpload:             E:\Applications\Current\PAM\QA\Export\NTRS_OUT_PAM_NT-22_20141218_20141218_213012.csv + 3 more
        /// _ExportRootFolder:          E:\Applications\Current\PAM\QA\Export
        /// _ArchiveFolder:             E:\Applications\Current\PAM\QA\Export\Archive\2014\12
        /// _TempFolder:                E:\Applications\Current\PAM\QA\Export\Archive\2014\12\2014.12.19-03.31.08_NT-22
        /// _CompressedArchiveFullPath: NTRS_OUT_NT-22_20141218_20141218_213012.csv
        /// </example>
        public void Load(List<string> filesToUpload, string outputFilePrefix)
        {
            _FilesToUpload = filesToUpload;
            _OutputFilePrefix = outputFilePrefix;
            _ExportRootFolder = Path.GetDirectoryName(_FilesToUpload[0]);
            _ArchiveFolder = Path.Combine(_ExportRootFolder, GetArchiveFolder());
            _TempFolder = Path.Combine(_ExportRootFolder, GetTempFolder());
            _CompressedArchiveFullPath = GetCompressedArchiveFullPath();
        }

        public void LoadFtpSettingsFromConfigFile()
        {
            _Port = ConfigurationManager.AppSettings["FTPPort"].ToInt(DEFAULT_SFTP_PORT);
            _FtpDirectory = ConfigurationManager.AppSettings["FTPDirectory"];
            _FtpUrl = ConfigurationManager.AppSettings["FTPURL"];
            _Password = ConfigurationManager.AppSettings["FTPPassword"];
            _UserName = ConfigurationManager.AppSettings["FTPUserName"];
        }

        public void LoadFtpSettingsFromPropertyCollection(Dictionary<string, string> properties)
        {
            _Port = properties["FTPPort"].ToInt(DEFAULT_SFTP_PORT);
            _FtpDirectory = properties["FTPDirectory"];
            _FtpUrl = properties["FTPURL"];
            _Password = properties["FTPPassword"];
            _UserName = properties["FTPUserName"];
        }

        public void UploadFiles()
        {
            CreateDirectory(_TempFolder);
            ArchiveFiles();
            CompressFiles();
            TransmitFiles();
            CleanUp();
        }

        /// <summary>
        /// Files are archived to ..\PamExport\PAM_TEST_PFA-2014.10.29-09.31.42
        /// </summary>
        private void ArchiveFiles()
        {
            foreach (string sourceFileFullPath in _FilesToUpload)
            {
                string sourceFile = Path.GetFileName(sourceFileFullPath);
                string targetFileFullPath = Path.Combine(_TempFolder, sourceFile);

                File.Move(sourceFileFullPath, targetFileFullPath);
                Logger.LogActivity("File archived: {0}", sourceFile);
            }
        }

        private void CompressFiles()
        {
            if (File.Exists(_CompressedArchiveFullPath))
                File.Delete(_CompressedArchiveFullPath);

            ZipFile.CreateFromDirectory(_TempFolder, _CompressedArchiveFullPath);
        }

        private void TransmitFiles()
        {
            string ftpFileName = Path.GetFileName(_CompressedArchiveFullPath);

            Sftp sftp = new Sftp(_FtpUrl, _UserName, _Password);
            sftp.Connect(_Port);

            if (DebugMode)
                Logger.LogActivity("Upload skipped in Debug Mode");
            else
            {
                sftp.Put(_CompressedArchiveFullPath, _FtpDirectory + "/" + ftpFileName);
                Logger.LogActivity("File {0} was successfully transmitted", _CompressedArchiveFullPath);
            }

            sftp.Close();
        }

        private void CleanUp()
        {
            IOExtensions.DeleteDirectory(_TempFolder);
        }

        private string GetArchiveFolder()
        {
            const string ARCHIVE_FOLDER = "Archive\\{0:yyyy}\\{0:MM}";
            string result = string.Format(ARCHIVE_FOLDER, DateTime.Today);

            return result;
        }

        private string GetTempFolder()
        {
            const string TEMP_FOLDER = "{0}\\{1:yyyyMMdd_HHmmss}";
            string result = string.Format(TEMP_FOLDER, _ArchiveFolder, DateTime.Now);

            return result;
        }

        private string GetCompressedArchiveFullPath()
        {
            const string FILE_NAME = "{0}_{1}.zip";

            string firstUploadFile = Path.GetFileNameWithoutExtension(_FilesToUpload[0]);
            string dateTimeStamps = firstUploadFile.Substring(_OutputFilePrefix.Length + 1);
            string prefix = _OutputFilePrefix.Replace("XXX_", "");
            string compressedArchive = string.Format(FILE_NAME, prefix, dateTimeStamps);
            string result = Path.Combine(_ArchiveFolder, compressedArchive);

            return result;
        }

        private static bool DebugMode
        {
            get
            {
                bool result = (ConfigurationManager.AppSettings["DebugMode"] == "true");
                return result;
            }
        }

        private void CreateDirectory(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Logger.LogActivity("Folder created: {0}", folder);
            }
        }

    }
}
