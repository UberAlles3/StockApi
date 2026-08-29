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
        //public enum PositionColumns : int
        //{
        //    Ticker = 0,
        //    QuantityHeld = 1,
        //    Price = 2,
        //    BuySell = 3,
        //    Metric = 33
        //}

        public static string PositionSymbolColumn = "Column0";
        public static string PositionQuantityColumn = "Column1";
        List<Setting> _settings = new List<Setting>();

        // Excel files
        private static string _excelFilePath = "";
        private static DateTime _jointPositionsImportDateTime = DateTime.Now.AddYears(-2);
        private static DateTime _jointTradesImportDateTime = DateTime.Now.AddYears(-2);


        public enum TradeColumns : int
        {
            TradeDate = 0,
            DowLevel = 1,
            BuySell = 2,
            QuantityTraded = 3,
            //Ticker = 4,
            TradePrice = 5,
            QuantityHeld = 6,
            AccountValue = 7,
            Notes = 8
        }

        private static DataTable _jointPositionsDataTable = null;
        public static DataTable JointPositionsDataTable
        {
            get
            {
                DateTime excelFileDateTime = System.IO.File.GetLastWriteTime(_excelFilePath);
                if (excelFileDateTime > _jointPositionsImportDateTime)
                {
                    _jointPositionsDataTable = (new ExcelManager()).ImportExcelSheet(_excelFilePath, 3, 0, 18);
                    _jointPositionsImportDateTime = DateTime.Now; // Update when the last import took place

                    _jointPositionsDataTable.AsEnumerable()
                        .Where(row => row.Field<string>(ExcelManager.PositionSymbolColumn).Contains("*")  // Symbol
                                    || row.Field<string>(ExcelManager.PositionSymbolColumn).Trim() == ""  // Symbol
                                    || row.Field<double>(ExcelManager.PositionQuantityColumn) == 0        // Quantiyy
                        )
                        .ToList().ForEach(row => row.Delete());

                    _jointPositionsDataTable.AcceptChanges();

                    // old way // _positionsDataTable = _positionsDataTable.AsEnumerable().Where(x => x[(int)PC.Ticker].ToString().Trim() != "" && !x[(int)PC.Ticker].ToString().Contains("*") && x[(int)PC.QuantityHeld].ToString().Trim() != "" && x[(int)PC.QuantityHeld].ToString().Trim() != "0").CopyToDataTable();

                    _jointPositionList = (new ExcelManager()).GetPositionsListFromPositionsTable(_excelFilePath, "JointPositions", 17);
                    _jointPositionList = _jointPositionList.Where(x => x.Quantity > 0 || (x.Quantity == 0 && x.BuyQuantity == 0)).ToList();
                }
                return _jointPositionsDataTable;
            }
            set => _jointPositionsDataTable = value;
        }

        private static List<ExcelPosition> _jointPositionList;
        public static List<ExcelPosition> JointPositionList
        {
            get
            {
                // Refresh this list if underlying Excel file was updated.
                var dummy = JointPositionsDataTable;

                return _jointPositionList;
            }
            set => _jointPositionList = value;
        }
        public ExcelManager()
        {
            _excelFilePath = AppConfig.Settings.Find(x => x.Name == "ExcelTradesPath").Value;
        }

        public DataTable ImportExcelSheet(string filePath, int sheetIdx, int startRow, int columns = 10)
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

        public List<ExcelPosition> GetPositionsListFromPositionsTable(string filePath, string targetSheetName, int columnCount)
        {
            var dataList = new List<ExcelPosition>();
            string importFilePath = Path.Combine(Path.GetDirectoryName(filePath) + "\\Import.xlsx");
            File.Copy(filePath, importFilePath, true);


            using (var stream = File.Open(importFilePath, FileMode.Open, FileAccess.Read))
            {
                var columnNames = new List<string>();
                PropertyInfo pi;

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Assuming the first row contains headers
                    bool sheetFound = false;

                    do
                    {
                        // reader.Name contains the current worksheet's name
                        if (reader.Name == targetSheetName)
                        {
                            sheetFound = true;
                            break;
                        }
                    } while (reader.NextResult()); // Moves to the next worksheet forward

                    if (!sheetFound)
                        throw new Exception("Sheet not found.");

                    while (reader.Read())
                    {
                        if (columnNames.Count == 0) // First row is columm names
                        {
                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames.Add(reader.GetString(i));
                            }
                        }
                        else
                        {
                            object val = null;
                            var item = new ExcelPosition();
                            for (int i = 0; i < columnCount; i++)
                            {
                                if (i > 18 && i < 30)
                                    continue;

                                if (columnNames[i] != null)
                                {
                                    pi = item.GetType().GetProperty(SanitizeName(columnNames[i]));

                                    if (pi == null)
                                        continue;

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
                                    try
                                    {
                                        pi.SetValue(item, Convert.ChangeType(val, pi.PropertyType), null);
                                    }
                                    catch
                                    {
                                        pi.SetValue(item, Convert.ChangeType("0", pi.PropertyType), null);
                                    }
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

        public List<ExcelTrade> GetTradeListFromTradeTable(string filePath, string targetSheetName)
        {
            var dataList = new List<ExcelTrade>();
            string importFilePath = Path.Combine(Path.GetDirectoryName(filePath) + "\\Import.xlsx");
            File.Copy(filePath, importFilePath, true);


            using (var stream = File.Open(importFilePath, FileMode.Open, FileAccess.Read))
            {
                var columnNames = new List<string>();
                PropertyInfo pi;

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    //string targetSheetName = "Trades";
                    bool sheetFound = false;

                    do
                    {
                        // reader.Name contains the current worksheet's name
                        if (reader.Name == targetSheetName)
                        {
                            sheetFound = true;
                            break;
                        }
                    } while (reader.NextResult()); // Moves to the next worksheet forward

                    if (!sheetFound)
                        throw new Exception("Sheet not found.");
                    // Assuming the first row contains headers

                    while (reader.Read())
                    {
                        if (columnNames.Count == 0) // First row is columm names
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                columnNames.Add(reader.GetString(i));
                            }
                        }
                        else
                        {
                            object val = null;
                            var item = new ExcelTrade();
                            for (int i = 0; i < 10; i++)
                            {
                                if (columnNames[i] != null)
                                {
                                    pi = item.GetType().GetProperty(SanitizeName(columnNames[i]));

                                    if (pi == null)
                                        continue;

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
                                        if (val.ToString().Contains("***") || val.ToString().Contains("Skip"))
                                            break;
                                    }

                                    // Use reflection or a mapping dictionary to set property values
                                    try
                                    {
                                        pi.SetValue(item, Convert.ChangeType(val, pi.PropertyType), null);
                                    }
                                    catch
                                    {
                                        pi.SetValue(item, Convert.ChangeType("0", pi.PropertyType), null);
                                    }
                                }
                            }

                            if (val.ToString().Contains("***") || val.ToString().Contains("Skip"))
                                continue;

                            if ((item.Symbol == null || item.Symbol.Trim() == "") && item.Symbol != "Skip")
                                break;

                            dataList.Add(item);
                        }
                    }
                }
            }

            return dataList;
        }


        public List<string> GetStockListFromPositionsTable(List<ExcelPosition> positionList)
        {
            List<string> stockList;
            stockList = positionList.Select(x => x.Symbol).ToList();

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

    public class ExcelPosition
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
        public double Dividend { get; set; }
        public double PastYearHigh { get; set; }
        public double PastYearLow { get; set; }
        public double TotalMetric { get; set; }
        public override string ToString()
        {
            return $"Symbol: {Symbol}, Quantity: {Quantity}";
        }
    }

    public class ExcelTrade
    {
        public DateTime TradeDate { get; set; }
        public double DOW { get; set; }
        public string BuySell { get; set; }
        public int Quantity { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public int QuantityHeld { get; set; }
        public double AccountBalance { get; set; }
        public string Notes { get; set; }
        public double BalanceAdjusted { get; set; }

        public override string ToString()
        {
            return $"TradeDatel: {TradeDate}, BuySell: {BuySell}, Quantity: {Quantity}, Symbol: {Symbol}, Price: {Price}, QuantityHeld: {QuantityHeld}";
        }
    }
}
