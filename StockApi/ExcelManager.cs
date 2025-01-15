using ExcelDataReader;
using System.Data;
using System.IO;

namespace StockApi
{
    public class ExcelManager
    {

        public ExcelManager()
        {

        }

        public DataTable ImportTrades(string filePath)
        {
            string importFilePath = Path.Combine(Path.GetDirectoryName(filePath) + "\\Import.xlsx");

            File.Copy(filePath, importFilePath, true);

            using (var stream = File.Open(importFilePath, FileMode.Open, FileAccess.Read))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    //reader.NextResult();
                    //while (reader.Read())
                    //{
                    //    reader.GetDouble(0);
                    //}

                    //do
                    //{
                    //    while (reader.Read())
                    //    {
                    //        // reader.GetDouble(0);
                    //    }
                    //} while (reader.NextResult());

                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {

                        // Gets or sets a callback to determine whether to include the current sheet
                        // in the DataSet. Called once per sheet before ConfigureDataTable.

                        FilterSheet = (tableReader, sheetIndex) => (sheetIndex == 1),
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                        {
                            FilterRow = rowReader => rowReader.RowCount > 40,
                            FilterColumn = (rowReader, columnIndex) => columnIndex < 10,
                        }
                    });  // The result of each spreadsheet is in result.Tables

                    //var newTable = result.Tables[0].AsEnumerable().Where(x => x[0].ToString().Contains("2024"));
                    //var newTable = result.Tables[0].AsEnumerable().Where(x => x[4].ToString() == "GFI").CopyToDataTable();
                    //string json = JsonConvert.SerializeObject(newTable, Formatting.Indented);
                    //Debug.Print(json);
                    return result.Tables[0];
                }
            }
        }
    }
}
