using System;
using System.Globalization;
using System.Text.RegularExpressions;

using ConceptONE.Infrastructure.Extensions;

namespace ConceptONE.Infrastructure.Net
{

    public class FtpFileDetails
    {

        private const string START = "^";
        private const string END = "$";
        private const string SPACES = @"\s+";
        private const string FILE_TYPE = @"(?<Type>[\-d]{1})";
        private const string FILE_PERMISSIONS = @"(?<Permissions>[rwx]{9})";
        private const string FILE_CODE = @"(?<Code>\d+)";
        private const string FILE_OWNER = @"(?<Owner>\w+)";
        private const string FILE_GROUP = @"(?<Group>\w+)";
        private const string FILE_SIZE = @"(?<Size>\w+)";
        private const string DATE_MONTH = @"(?<Month>\w+)";
        private const string DATE_DAY = @"(?<Day>\d+)";
        private const string DATE_TIME = @"(?<Time>\d+:\d+)";
        private const string FILE_NAME = @"(?<FileName>.+)";

        private static string _Pattern =
            START +
            FILE_TYPE + FILE_PERMISSIONS + SPACES +
            FILE_CODE + SPACES +
            FILE_OWNER + SPACES +
            FILE_GROUP + SPACES +
            FILE_SIZE + SPACES +
            DATE_MONTH + SPACES +
            DATE_DAY + SPACES +
            DATE_TIME + SPACES +
            FILE_NAME +
            END;

        public string FileType { get; set; }
        public string OwnerPermissions { get; set; }
        public string GroupPermissions { get; set; }
        public string WorldPermissions { get; set; }
        public string FileCode { get; set; }
        public string Owner { get; set; }
        public string Group { get; set; }
        public int Size { get; set; }
        public DateTime DateAndTime { get; set; }
        public string FileName { get; set; }

        /// <summary>
        /// Note:
        /// There is no standard way to parse file details. Eery FTP server is different.
        /// 
        /// This version assumes details line looks like this:
        /// "-rwxrwxrwx   1 owner    group               0 Aug 18 14:02 A0KLGUNN.Y2015230.EDI"
        /// 
        /// If different ones are needed we'll have to extend this class to support multiple standards
        /// </summary>
        public FtpFileDetails(string detailString)
        {
            Match parsed = new Regex(_Pattern).Match(detailString);

            if (parsed.Groups.Count == 1)
                return;

            FileType = parsed.Groups["Type"].ToString();

            OwnerPermissions = parsed.Groups["Permissions"].ToString().Substring(0, 3);
            GroupPermissions = parsed.Groups["Permissions"].ToString().Substring(3, 3);
            WorldPermissions = parsed.Groups["Permissions"].ToString().Substring(6, 3);

            FileCode = parsed.Groups["Code"].ToString();
            Owner = parsed.Groups["Owner"].ToString();
            Group = parsed.Groups["Group"].ToString();
            Size = parsed.Groups["Size"].ToString().ToInt(0);

            string monthName = parsed.Groups["Month"].ToString();
            int month = DateTime.ParseExact(monthName, "MMM", CultureInfo.CurrentCulture).Month;
            int day = parsed.Groups["Day"].ToString().ToInt(0);

            string time = parsed.Groups["Time"].ToString();
            string[] timeParts = time.Split(':');
            int hours = timeParts[0].ToInt(0);
            int minutes = timeParts[1].ToInt(0);

            DateAndTime = new DateTime(DateTime.Today.Year, month, day, hours, minutes, 0);
            FileName = parsed.Groups["FileName"].ToString();
        }

        public bool IsFile()
        {
            return FileType == "-";
        }

        public bool IsDirectory()
        {
            return FileType == "d";
        }
    }

}
