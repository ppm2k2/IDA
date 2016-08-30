using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ConceptONE.Infrastructure;
using ConceptONE.Infrastructure.Extensions;
using IDADataAccess.DEMI;
using IDALibrary.DataLoaders.Interfaces;
using IDALibrary.DEMI.Utilities;
using IDALibrary.Enums;
using OfficeOpenXml;

namespace IDALibrary.DataLoaders
{

    public class ExcelService : IExcelService
    {

        private const int SAMPLE_COLUMN_COUNT = 20;
        #region Fields

        private int _SetId;
        private DEMIContext _DemiContext = new DEMIContext();

        #endregion

        public ExcelService(int setId)
        {
            _SetId = setId;
        }

        public ExcelService() { }

        #region Properties

        public DataTable Records { get; set; }
        public IDictionary<int, string> ColumnNames { get; set; }
        public string InputFileName { get; set; }

        #endregion 

        #region Public

        public void LoadData(string userName, FileType fileType)
        {
            IDataLoader loader = CreateDataLoader(Path.GetExtension(InputFileName));

            loader.Load(this);
            DataTable sourceTable = GetDataTableFromExcel(InputFileName, fileType);
            SaveTableData(sourceTable, userName, fileType);
        }

        private void SaveTableData(DataTable sourceTable, string userName, FileType fileType)
        {
            if (fileType == FileType.Source)
            {
                _DemiContext.CreateSourceDataTable(sourceTable, userName);
                _DemiContext.LoadSourceDataTable(sourceTable, userName);
            }
            else
            {
                _DemiContext.CreateTransformedDataTable(sourceTable, userName);
            }
        }

        public DataTable GetDataTableFromExcel(string sourceFilePath, FileType fileType)
        {
            Logger.LogActivity("Creating source table from: {0}", sourceFilePath);

            using (ExcelPackage package = new ExcelPackage())
            {
                using (FileStream stream = File.OpenRead(sourceFilePath))
                    package.Load(stream);

                ExcelWorksheet worksheet = package.Workbook.Worksheets.First();
                DataTable result = new DataTable(worksheet.Name);

                AppendColumns(result, worksheet, fileType);
                AppendRows(result, worksheet);

                //ConsoleExtensions.LogDataTable(result, true);

                return result;
            }

        }

        #endregion            

        #region Private

        private void AppendColumns(DataTable sourceDataTable, ExcelWorksheet worksheet, FileType fileType)
        {
            int lastCol = worksheet.Dimension.End.Column;
            int lastRow = worksheet.Dimension.End.Row;

            DataTypeParser parser = new DataTypeParser();

            for (int col = 1; col <= lastCol; col++)
            {
                Type columnType;

                if (fileType == FileType.Destination)
                {
                    //Destination: data type not needed
                    columnType = typeof(string);
                }
                else if (lastRow == 1)
                {
                    //Source: with no data (cannot be used)
                    throw new Exception("Source data file empty");
                }
                else
                {
                    //Source: with data (guess data type using sample values)
                    ExcelRangeBase sampleCells = worksheet.Cells[2, col, lastRow, col];
                    columnType = GetColumnDataType(sampleCells, parser);
                }

                string columnName = worksheet.Cells[1, col].Text;
                sourceDataTable.Columns.Add(columnName, columnType);

                Logger.LogActivity("Adding column: {0} [{1}]", columnName, columnType);
            }
        }

        private Type GetColumnDataType(ExcelRangeBase sampleCells, DataTypeParser parser)
        {
            IEnumerable<ExcelRangeBase> sampleCellsEnumerable = (IEnumerable<ExcelRangeBase>)sampleCells;
            IList<ExcelRangeBase> sampleCellsList = sampleCellsEnumerable.ToList<ExcelRangeBase>();
            IList<string> sampleValues = sampleCellsList.
                Select(c => c.Text).
                Where(t => !String.IsNullOrEmpty(t)).
                Distinct().
                //Take(SAMPLE_COLUMN_COUNT).
                ToList();

            Type result = parser.GetType(sampleValues);

            return result;
        }

        private void AppendRows(DataTable sourceDataTable, ExcelWorksheet worksheet)
        {
            const int FIRST_ROW = 2;

            for (int row = FIRST_ROW; row <= worksheet.Dimension.End.Row; row++)
            {
                ExcelRangeBase sheetRow = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column];
                DataRow dataRow = sourceDataTable.NewRow();
                foreach (ExcelRangeBase cell in sheetRow)
                {
                    DataColumn column = sourceDataTable.Columns[cell.Start.Column - 1];

                    try
                    {
                        if (String.IsNullOrEmpty(cell.Text))
                            dataRow[cell.Start.Column - 1] = DBNull.Value;
                        else if (column.DataType == typeof(DateTime))
                            dataRow[cell.Start.Column - 1] = DataTypeParser.GetAsDateTime(cell.Text);
                        else
                            dataRow[cell.Start.Column - 1] = cell.Text; 
                    }
                    catch 
                    {
                        Logger.LogActivity( "{0} [{1}]: could not add {2}", column.ColumnName, column.DataType, cell.Text);
                    }
                }
                sourceDataTable.Rows.Add(dataRow);
            }
        }

        private IDataLoader CreateDataLoader(string fileExtension)
        {
            IDataLoader result = null;

            if (fileExtension.Contains(".xlsx"))
                result = new XlsxDataLoader();
            else if (fileExtension.Contains(".csv"))
                result = new CsvDataLoader();
            else
                throw new NotImplementedException();

            return result;
        }

        #endregion               

    }
}