using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ConceptONE.Infrastructure;
using CsvHelper;
using IDALibrary.DataLoaders.Interfaces;

namespace IDALibrary.DataLoaders
{
    internal class CsvDataLoader : IDataLoader
    {
        public void Load(IExcelService service) 
        {
            CsvParser parser = new CsvParser(new StreamReader(service.InputFileName));
            CsvReader reader = new CsvReader(parser);
            DataRow row = null;
            bool isTableColCreated = false;
            string exceptionText = null;

            try
            {
                int recordCount = 0;
                while (reader.Read())
                {
                    if (!isTableColCreated)
                    {
                        service.Records = new DataTable();
                        service.ColumnNames = new Dictionary<int, string>(); 
                        reader.FieldHeaders.ToList().ForEach(c =>
                        {
                            service.Records.Columns.Add(c);
                            service.ColumnNames.Add(recordCount++, c);
                        });
                        recordCount = 0;
                        isTableColCreated = !isTableColCreated;
                    }

                    if (!string.IsNullOrEmpty(exceptionText)) continue;
                    row = service.Records.NewRow();
                    for (int columnIndex = 0; columnIndex < reader.CurrentRecord.Length; columnIndex++)
                    {
                        string header = reader.FieldHeaders[columnIndex].Trim();

                        string value = reader.CurrentRecord[columnIndex].Trim();
                        if (value == "" || value.ToLower() == "null") continue;
                        row[header] = value;
                    }

                    service.Records.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            finally
            {
                reader.Dispose();
                parser.Dispose();
            }

            if (!string.IsNullOrEmpty(exceptionText))
                throw new ApplicationException(exceptionText);
        }
    }
}
