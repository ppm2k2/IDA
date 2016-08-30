using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using IDALibrary.DataLoaders.Interfaces;
using OfficeOpenXml;

namespace IDALibrary.DataLoaders
{
    internal class XlsxDataLoader : IDataLoader
    {
        public void Load(IExcelService service)
        {
            StreamReader reader = null;
            string exceptionText = null;
            DataRow row = null;

            try
            {
                reader = new StreamReader(service.InputFileName);
                Stream baseStream = reader.BaseStream;

                service.Records = new DataTable();
                service.ColumnNames = new Dictionary<int, string>();

                ExcelPackage excel = new ExcelPackage(baseStream);
                Thread.Sleep(1000);
                ExcelWorksheet worksheet = excel.Workbook.Worksheets.FirstOrDefault();
                string header = null;

                for (int rowIndex = worksheet.Dimension.Start.Row; rowIndex <= worksheet.Dimension.End.Row; rowIndex++)
                {
                    if (!string.IsNullOrEmpty(exceptionText))
                        continue;

                    try
                    {
                        if (rowIndex > 1)
                            row = service.Records.NewRow();

                        for (int columnIndex = worksheet.Dimension.Start.Column; columnIndex <= worksheet.Dimension.End.Column; columnIndex++)
                        {
                            header = worksheet.Cells[worksheet.Dimension.Start.Row, columnIndex].Value.ToString().Trim();
                            if (rowIndex == 1)
                            {
                                service.Records.Columns.Add(header);
                                service.ColumnNames.Add(columnIndex, header);
                            }
                            else
                            {
                                row[header] = worksheet.Cells[rowIndex, columnIndex].Value;
                            }

                        }
                        if (rowIndex > 1)
                            service.Records.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(String.Format("{0} - header {1} - record number ", ex.Message, header));
                    }
                }
                excel.Stream.Close();
                excel.Stream.Dispose();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't load excel file!", ex);
            }
            finally
            {
                reader.Close();
                reader.Dispose();
            }

            if (!string.IsNullOrEmpty(exceptionText))
                throw new ApplicationException(exceptionText);
        }

    }
}
