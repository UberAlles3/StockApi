using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace StockApi
{
    public class ExcelInterop
    {
        public void OpenExcel()
        {
            using (var package = new ExcelPackage(new FileInfo("Stocks.xlsx")))
            {
                var firstSheet = package.Workbook.Worksheets["Trades"];
                Console.WriteLine("Sheet 1 Data");
                Console.WriteLine($"Cell A2 Value   : {firstSheet.Cells["A2"].Text}");
                Console.WriteLine($"Cell A2 Color   : {firstSheet.Cells["A2"].Style.Font.Color.LookupColor()}");
                Console.WriteLine($"Cell B2 Formula : {firstSheet.Cells["B2"].Formula}");
                Console.WriteLine($"Cell B2 Value   : {firstSheet.Cells["B2"].Text}");
                Console.WriteLine($"Cell B2 Border  : {firstSheet.Cells["B2"].Style.Border.Top.Style}");
                Console.WriteLine("");

                var secondSheet = package.Workbook.Worksheets["Second Sheet"];
                Console.WriteLine($"Sheet 2 Data");
                Console.WriteLine($"Cell A2 Formula : {secondSheet.Cells["A2"].Formula}");
                Console.WriteLine($"Cell A2 Value   : {secondSheet.Cells["A2"].Text}");
            }








            //string filename = @"D:\Test.xlsx";
            //int row = 1;
            //int column = 1;
            //Application excelApplication = new Application();
            //Workbook excelWorkBook = excelApplication.Workbooks.Open(filename);
            //string workbookName = excelWorkBook.Name;
            //int worksheetcount = excelWorkBook.Worksheets.Count;

            //if (worksheetcount > 0)
            //{
            //    Worksheet worksheet = (Worksheet)excelWorkBook.Worksheets[1];
            //    string firstworksheetname = worksheet.Name;
            //    var data = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[row, column]).Value;
            //    excelApplication.Quit();
            //    return data;
            //}
            //else
            //{
            //    Console.WriteLine("No worksheets available");
            //    excelApplication.Quit();
            //    return 0;
            //}


            //string excelFileName = "Stocks.xls";
            //string excelConnectString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=Book2.xls;Extended Properties=""Excel 8.0;HDR=YES;""";
            ////string excelConnectString = @"Provider = Microsoft.Jet.OLEDB.4.0;Data Source = " + excelFileName + ";" + "Extended Properties = Excel 8.0; HDR=Yes;IMEX=1";

            //OleDbConnection objConn = new OleDbConnection(excelConnectString);
            //OleDbCommand objCmd = new OleDbCommand("Select * From [Sheet1$]", objConn);

            //OleDbDataAdapter objDatAdap = new OleDbDataAdapter();
            //objDatAdap.SelectCommand = objCmd;
            //DataSet ds = new DataSet();
            //objDatAdap.Fill(ds);

        }
    }
}
