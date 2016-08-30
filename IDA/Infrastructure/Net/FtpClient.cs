using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ConceptONE.Infrastructure.Extensions;

namespace ConceptONE.Infrastructure.Net
{
    public class FtpClient
    {
        private const string FULL_URI_WITH_FOLDER = "ftp://{0}:{1}/{2}/{3}";
        private const string FULL_URI_NO_FOLDER = "ftp://{0}:{1}/{2}";
        private const int DEFAULT_PORT = 21;

        private string _ServerUri;
        private string _Username;
        private string _Password;
        private int _Port;

        public int Port
        {
            get
            {
                if (_Port == 0)
                    return DEFAULT_PORT;
                else
                    return _Port;
            }
            set
            {
                _Port = value;
            }
        }

        private readonly char[] charsToRemove = new char[] { '/', '\\' };

        public FtpClient(string serverUri, string username, string password)
        {
            _ServerUri = serverUri.Trim(charsToRemove);
            _Username = username;
            _Password = password;
        }

        public bool DownloadFile(string remoteFolder, string file, string localPath)
        {
            string fullRemotePath = GetFullUri(remoteFolder, file);
            string fullLocalPath = Path.Combine(localPath, file);

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fullRemotePath);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_Username, _Password);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    responseStream.SaveTo(fullLocalPath);
                    responseStream.Close();
                }

                return true;
            }
            catch (WebException ex)
            {
                if (FileNotFound(ex))
                    return false;
                else
                    throw;
            }

        }

        public List<FtpFileDetails> GetDirectoryListing(string remoteFolder)
        {
            string fullRemotePath = GetFullUri(remoteFolder, "");
            List<FtpFileDetails> result = new List<FtpFileDetails>();

            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(fullRemotePath);

                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(_Username, _Password);

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        FtpFileDetails details = new FtpFileDetails(line);

                        if (String.IsNullOrEmpty(details.FileName))
                            Logger.LogActivity("Could not parse file details: {0}", line);
                        else
                            result.Add(details);
                    }
            }
            catch (Exception ex)
            {
                Logger.LogActivity("Error getting directory listing from: {0}", fullRemotePath);
                Logger.LogException(ex);
            }

            return result;
        }

        private bool FileNotFound(WebException ex)
        {
            FtpWebResponse response = (FtpWebResponse)ex.Response;
            bool result = (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable);

            return result;
        }

        private string GetFullUri(string remoteFolder, string file)
        {
            string result = "";

            if (string.IsNullOrEmpty(remoteFolder))
                result = string.Format(FULL_URI_NO_FOLDER, _ServerUri, Port, file.Trim(charsToRemove));
            else
                result = string.Format(FULL_URI_WITH_FOLDER, _ServerUri, Port, remoteFolder.Trim(charsToRemove), file.Trim(charsToRemove));

            return result;
        }

    }
}
