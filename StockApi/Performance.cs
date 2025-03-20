using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Drake.Extensions;

namespace StockApi
{
    public class Performance
    {
        private List<PerformanceItem> _performanceList = new List<PerformanceItem>();

        public Performance()
        {

        }

        public void GetLatestBuyPerformance(DataTable _positionsDataTable, DataTable _tradesDataTable)
        {
            int DateColumn = 0;

            // Get last 20 buys
            var positions = _positionsDataTable.AsEnumerable().Where(x => x[1].ToString().Trim() != "0" && x[1].ToString().Trim() !="");
            
            var tickerTrades = _tradesDataTable.AsEnumerable().Where(x => x[2].ToString() == "Buy" && x[1].ToString().Trim() != "").Skip(700);
            tickerTrades = tickerTrades.OrderByDescending(x => x[DateColumn]).Take(20);

            _performanceList.Clear();
            foreach (DataRow dr in tickerTrades)
            {
                // Search in positions for ticker to get current price 
                
                string ticker = dr.ItemArray[4].ToString();
                string temp = dr.ItemArray[3].ToString();
                int quantity = 0;
                if (temp._IsInt())
                {
                    quantity = Convert.ToInt32(temp);
                }

                temp = positions.Where(x => x[0].ToString() == ticker).First().ItemArray[2].ToString();
                decimal currentPrice = 0;
                if(temp._IsDecimal())
                {
                    currentPrice = Convert.ToDecimal(temp);
                }

                // Get the buy price
                temp = dr.ItemArray[5].ToString();
                decimal buyPrice = 0;
                if (temp._IsDecimal())
                {
                    buyPrice = Convert.ToDecimal(temp);
                }

                // profit/ loss   
                decimal profit = currentPrice - buyPrice;

                PerformanceItem pi = new PerformanceItem()
                {
                    BuyDate = Convert.ToDateTime(dr.ItemArray[0].ToString()),
                    Ticker = ticker,
                    Quantity = quantity,
                    BuyPrice = buyPrice,
                    CurrentPrice = currentPrice,
                    Profit = profit,
                    TotalProfit = quantity * profit
                };

                _performanceList.Add(pi);
            }
        }
        public void ShowPerformanceForm(Form1 form1)
        {
            PerformanceForm pf = new PerformanceForm();
            pf.Owner = form1;
            pf.Text = "Buy Performance";

            pf.Show();

            var bindingList = new BindingList<PerformanceItem>(_performanceList);
            var source = new BindingSource(bindingList, null);

            pf.dataGridView2.DefaultCellStyle.ForeColor = Color.LightSteelBlue;
            pf.dataGridView2.DefaultCellStyle.SelectionForeColor = Color.LightSteelBlue; 
            pf.dataGridView2.DefaultCellStyle.BackColor = Color.Black;
            pf.dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Black;
            
            pf.dataGridView2.DataSource = source;
            pf.dataGridView2.Columns[0].HeaderText = "Date";
            pf.dataGridView2.Columns[0].Width = 80;
            ////pf.dataGridView2.Columns[0].DefaultCellStyle.Format = "MM/dd/yyyy";
            pf.dataGridView2.Columns[1].HeaderText = "Ticker";
            pf.dataGridView2.Columns[1].Width = 50;
            pf.dataGridView2.Columns[2].HeaderText = "Quan.";
            pf.dataGridView2.Columns[2].Width = 60;
            pf.dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            pf.dataGridView2.Columns[2].DefaultCellStyle.Format = "#####";

            pf.dataGridView2.Columns[3].HeaderText = "Bought";
            pf.dataGridView2.Columns[3].Width = 77;
            pf.dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            pf.dataGridView2.Columns[3].DefaultCellStyle.Format = "N2";
            
            pf.dataGridView2.Columns[4].HeaderText = "Current";
            pf.dataGridView2.Columns[4].Width = 77;
            pf.dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            pf.dataGridView2.Columns[4].DefaultCellStyle.Format = "N2";

            pf.dataGridView2.Columns[5].HeaderText = "Profit";
            pf.dataGridView2.Columns[5].Width = 77;
            pf.dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            pf.dataGridView2.Columns[5].DefaultCellStyle.Format = "N2";

            pf.dataGridView2.Columns[6].HeaderText = "Tot. Profit";
            pf.dataGridView2.Columns[6].Width = 77;
            pf.dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            pf.dataGridView2.Columns[6].DefaultCellStyle.Format = "N2";

            pf.dataGridView2.Refresh();

            pf.lblTotalProfit.Text = _performanceList.Sum(x => x.TotalProfit).ToString("0.00");
        }
    }

    public class PerformanceItem
    {
        [DisplayName("Date")] 
        public DateTime BuyDate { get; set; }
        [DisplayName("Ticker")]
        public string Ticker { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
        [DisplayName("Bought")]
        public decimal BuyPrice { get; set; }
        [DisplayName("Current")]
        public decimal CurrentPrice { get; set; }
        [DisplayName("Profit")]
        public decimal Profit { get; set; }
        [DisplayName("Tot. Profit")]
        public decimal TotalProfit { get; set; }
    }
}
