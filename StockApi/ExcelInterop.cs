using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace StockApi
{
    public class ExcelInterop
    {
        public partial class RequestImport
        {
            static string _filename = @"E:\Source Code\StockAPI\StockApi\StockApi\bin\Debug\netcoreapp3.1";
            static string _sheetname;
            public static TextBox textBox;
            public static bool abort = false;

            public static void Import(int startingRecord, string filename, string sheetname)
            {

                int record = 0;
                _filename = filename;
                _sheetname = sheetname;

                OleDbConnection con = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _filename + ";Extended Properties=Excel 8.0");
                OleDbDataAdapter da = new OleDbDataAdapter("select * from " + sheetname, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                textBox.Text = filename + Environment.NewLine;
                textBox.Text += sheetname + Environment.NewLine;
                Application.DoEvents();

                foreach (DataRow dr in dt.Rows)
                {
                    record++;
                    if ((record % 10) == 0)
                    {
                        Console.WriteLine(record);
                        if ((record % 100) == 0)
                            textBox.Text += record + Environment.NewLine;
                        else
                            textBox.Text += record;
                    }
                    else
                    {
                        Console.Write(".");
                        textBox.Text += ".";
                    }

                    Application.DoEvents();

                    if (record >= startingRecord)
                    {
                        //RequestImport.SaveRequest(dr, record);
                        //if (abort)
                        //    break;
                    }
                    Application.DoEvents();
                }
                da.Dispose();
                dt.Dispose();
                con.Dispose();
            }

        }
    }
}
