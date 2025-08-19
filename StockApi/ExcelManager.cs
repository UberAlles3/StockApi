using ExcelDataReader;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace StockApi
{
    public class ExcelManager
    {
        public enum PositionColumns : int
        {
            Ticker = 0,
            QuantityHeld = 1,
            Price = 2,
            Metric = 32
        }

        public enum TradeColumns : int
        {
            TradeDate = 0,
            DowLevel = 1,
            BuySell = 2,
            QuantityTraded = 3,
            Ticker = 4,
            TradePrice = 5,
            QuantityHeld = 6,
            AccountValue = 7,
            Splits = 8
        }

        public ExcelManager()
        {

        }

        public DataTable ImportExceelSheet(string filePath, int sheetIdx, int startRow, int columns = 10)
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

                        FilterSheet = (tableReader, sheetIndex) => (sheetIndex == sheetIdx),
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                        {
                            //FilterRow = rowReader => rowReader.RowCount > startRow,
                            FilterRow = rowReader => rowReader.Depth > startRow,
                            FilterColumn = (rowReader, columnIndex) => columnIndex < columns,
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
        public List<string> GetStockListFromPositionsTable(DataTable positionsDataTable)
        {
            List<string> stockList;
            EnumerableRowCollection<DataRow> positions = positionsDataTable.AsEnumerable().Where(x => x[(int)PositionColumns.Ticker].ToString().Trim() != "" && !x[(int)PositionColumns.Ticker].ToString().Contains("*") && x[(int)PositionColumns.QuantityHeld].ToString().Trim() != "" && x[(int)PositionColumns.QuantityHeld].ToString().Trim() != "0");
            stockList = positions.Select(x => x[(int)PositionColumns.Ticker].ToString().Trim()).ToList();
            stockList = stockList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList(); // remove blacks
            return stockList;
        }
    }
}
