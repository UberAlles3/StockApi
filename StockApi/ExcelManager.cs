using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace StockApi
{
    public class ExcelManager
    {
        public enum PositionColumns : int
        {
            Ticker = 0,
            QuantityHeld = 1,
            Price = 2,
            BuySell = 3,
            Metric = 33
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

        public List<ExcelPositions> GetPositionsListFromPositionsTable(string filePath)
        {
            var dataList = new List<ExcelPositions>();
            string importFilePath = Path.Combine(Path.GetDirectoryName(filePath) + "\\Import.xlsx");
            File.Copy(filePath, importFilePath, true);


            using (var stream = File.Open(importFilePath, FileMode.Open, FileAccess.Read))
            {
                var columnNames = new List<string>();

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Assuming the first row contains headers

                    while (reader.Read())
                    {
                        if (columnNames.Count == 0) // First row is columm names
                        {
                            for (int i = 0; i < 17; i++)
                            {
                                columnNames.Add(reader.GetString(i));
                            }
                        }
                        else
                        {
                            object val = null;
                            var item = new ExcelPositions();
                            for (int i = 0; i < 17; i++)
                            {
                                if(columnNames[i] != null)
                                {
                                    PropertyInfo pi = item.GetType().GetProperty(SanitizeName(columnNames[i]));
                                    val = reader.GetValue(i);

                                    if (val == null || val.ToString().Trim() == "")
                                    {
                                        if (pi.PropertyType.IsValueType)
                                        {
                                            val = Activator.CreateInstance(pi.PropertyType); // For value types, get default instance
                                        }
                                    }
                                    else
                                    {
                                        if (val.ToString().Contains("***"))
                                            break;
                                    }

                                    // Use reflection or a mapping dictionary to set property values
                                    pi.SetValue(item, Convert.ChangeType(val, pi.PropertyType), null);
                                }
                            }

                            if (val.ToString().Contains("***"))
                                continue;

                            if (item.Symbol == null || item.Symbol.Trim() == "")
                                break;

                            dataList.Add(item);
                        }
                    }
                }
            }

            return dataList;
        }



        public List<string> GetStockListFromPositionsTable(DataTable positionsDataTable)
        {
            List<string> stockList;
            EnumerableRowCollection<DataRow> positions = positionsDataTable.AsEnumerable().Where(x => x[(int)PositionColumns.Ticker].ToString().Trim() != "" && !x[(int)PositionColumns.Ticker].ToString().Contains("*") && x[(int)PositionColumns.QuantityHeld].ToString().Trim() != "" && x[(int)PositionColumns.QuantityHeld].ToString().Trim() != "0");
            stockList = positions.Select(x => x[(int)PositionColumns.Ticker].ToString().Trim()).ToList();
            stockList = stockList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList(); // remove blacks
            return stockList;
        }


        ////////////////////////////////////////////////////
        /// Generating class code from Excel columns
        /// 
        public void GenerateClassCodeFromExcelSheet(string filePath)
        {
            string importFilePath = Path.Combine(Path.GetDirectoryName(filePath) + "\\Import.xlsx");
            File.Copy(filePath, importFilePath, true);


            using (var stream = File.Open(importFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Assuming the first row contains headers
                    reader.Read();
                    var columnNames = new List<string>();
                    for (int i = 0; i < 8; i++)
                    {
                        columnNames.Add(reader.GetString(i));
                    }
                    GenerateClassCode("ExcelPositions", columnNames);
                }
            }
        }

        public static string GenerateClassCode(string className, List<string> columnNames)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");

            foreach (var columnName in columnNames)
            {
                // Sanitize column names for valid C# property names
                var sanitizedColumnName = SanitizeName(columnName);
                // You might infer type here based on data, otherwise default to string
                sb.AppendLine($"    public string {sanitizedColumnName} {{ get; set; }}");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string SanitizeName(string name)
        {
            // Implement logic to remove spaces, special characters, and ensure valid identifier
            // Example: "My Column Name" -> "MyColumnName"
            return Regex.Replace(name, @"[^a-zA-Z0-9_]", string.Empty);
        }
    }

    public class ExcelPositions
    {
        public string Symbol { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string BuySell { get; set; }
        public double BuyQuantity { get; set; }
        public double BuyPrice { get; set; }
        public double SellQuantity { get; set; }
        public double SellPrice { get; set; }
        public double SharesToBuy { get; set; }
        public double BuyTarget { get; set; }
        public double SharesToSell { get; set; }
        public double SellTarget { get; set; }
        public override string ToString()
        {
            return $"Symbol: {Symbol}, Quantity: {Quantity}";
        }
    }
}
