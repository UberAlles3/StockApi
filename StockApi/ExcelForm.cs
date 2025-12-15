using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PC = StockApi.ExcelManager.PositionColumns;

namespace StockApi
{
    public partial class ExcelForm : Form
    {
        List<ExcelPositions> positions;
        public ExcelForm(string excelPositionsPath)
        {
            positions = (new ExcelManager()).GetPositionsListFromPositionsTable(excelPositionsPath);
            InitializeComponent();
        }

        private void ExeclForm_Load(object sender, EventArgs e)
        {
        }

        ////V VISA INC CLASS                  A Filled  Buy	1 Shares Limit $323.85	Day	$323.65 
        ////NICE NICE LTD FSPONSORED ADR           1 ADR REPS    1  ORD SHS    Filled Buy	2 Shares Limit $112.74	Day + ext	$112.71 
        ////DUOL DUOLINGO INC CLASS                  A Filled  Buy	1 Shares Limit $176.90	Day	$176.89 
        private void btnParse_Click(object sender, EventArgs e)
        {
            string[] lines;
            string[] cells;
            double q = 0;
            StringBuilder sb = new StringBuilder();
            ExcelPositions excelPosition = null;

            lines = textBox1.Text.Split("\r\n");

            sb.Append("\r\n\r\nPositions\r\n-------------------------------------------------------------\r\n");

            //Symbol Quantity    Price    BuySell    Buy Quantiy    Buy Price    Sell Quantity   Sell Price

            // Postions new data
            foreach (string line in lines)
            {
                cells = line.Split("\t");
                if (cells.GetLength(0) > 5)
                {
                    excelPosition = positions.Where(x => x.Symbol.ToUpper().Trim() == cells[0].Trim()).FirstOrDefault();
                    if (excelPosition != null)
                    {
                        if (cells[4].ToUpper() == "BUY")
                            q = excelPosition.Quantity + Convert.ToDouble(cells[5].Replace("Shares", "").Trim()); // Shares bought added existing quantity
                        else
                            q = excelPosition.Quantity - Convert.ToDouble(cells[5].Replace("Shares", "").Trim()); // Shares bought added existing quantity
                    }
                    else
                    {
                        excelPosition = new ExcelPositions();
                        q = Convert.ToDouble(cells[5].Replace("Shares", ""));
                        MessageBox.Show("First add the new stock to the positions sheet. Exiting.");
                        return;
                    }

                    sb.Append(cells[0] + "\t"); // Ticker
                    sb.Append(q.ToString() + "\t"); // Shares bought added 
                    sb.Append(excelPosition.Price.ToString() + "\t"); // Current price, from spreadsheet
                    sb.Append(cells[4] + "\t"); // Buy/Sell
                    if(cells[4].ToUpper() == "BUY")
                    {

                        sb.Append(cells[5].Replace("Shares", "").Trim() + "\t"); // Shares bought
                        sb.Append(cells[8].Replace("$", "") + "\t"); // fill Price
                        if(excelPosition.SellQuantity == 0)
                            sb.Append("" + "\t"); // Sell Quantity
                        else
                            sb.Append(excelPosition.SellQuantity.ToString() + "\t"); // Sell Quantity

                        if (excelPosition.SellPrice == 0)
                            sb.Append("" + "\t"); // Sell Quantity
                        else
                            sb.Append(excelPosition.SellPrice.ToString()); // Sell Price
                    }
                    else
                    {
                        q = excelPosition.Quantity - Convert.ToDouble(cells[5].Replace("Shares", "").Trim()); // Shares bought added existing quantity

                        sb.Append(excelPosition.BuyQuantity.ToString() + "\t"); // Buy Quantity
                        sb.Append(excelPosition.BuyPrice.ToString() + "\t"); // Buy Price
                        sb.Append(cells[5].Replace("Shares", "").Trim() + "\t"); // Sell Quantity
                        sb.Append(cells[8].Replace("$", "")); // fill Price
                    }

                    sb.Append("\r\n"); 
                }
            }
            sb.Append("\r\nTrades\r\n-------------------------------------------------------------\r\n");

            foreach (string line in lines)
            {
                cells = line.Split("\t");
                if (cells.GetLength(0) > 5)
                {
                    excelPosition = positions.Where(x => x.Symbol.ToUpper().Trim() == cells[0].Trim()).First();
                    if (cells[4].ToUpper() == "BUY")
                    {
                        q = excelPosition.Quantity + Convert.ToDouble(cells[5].Replace("Shares", "").Trim()); // Shares bought added existing quantity
                    }
                    else
                    {
                        q = excelPosition.Quantity - Convert.ToDouble(cells[5].Replace("Shares", "").Trim()); // Shares bought added existing quantity
                    }

                    sb.Append(cells[4] + "\t"); // Buy/Sell
                    sb.Append(cells[5].Replace("Shares", "").Trim() + "\t"); // Quantity
                    sb.Append(cells[0] + "\t"); // Ticker
                    sb.Append(cells[8].Replace("$", "") + "\t"); // fill Price

                    sb.Append(q + "\r\n"); // Shares Owned
                }
            }
            textBox1.Text += sb.ToString();
        }
    }
}
