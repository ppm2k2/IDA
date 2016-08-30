using System.Collections.Generic;
using System.Data;
using IDALibrary.Enums;

namespace IDALibrary.DataLoaders.Interfaces
{
    public interface IExcelService
    {
        DataTable Records { get; set; }
        IDictionary<int, string> ColumnNames { get; set; }
        string InputFileName { get; set; }

        DataTable GetDataTableFromExcel(string sourceFilePath, FileType fileType);
        void LoadData(string userName, FileType fileType);
    }
}
