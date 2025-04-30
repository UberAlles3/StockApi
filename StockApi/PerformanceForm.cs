using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockApi
{
    public partial class PerformanceForm : Form
    {
        private enum GridColumns
        {
            TradeDate,
            Ticker,
            Quantity,
            TradePrice, 
            CurrentPrice,
            Profit,
            TotalProfit,
            DowLevel,
            SoldAndBought
        }
        
        
        List<PerformanceItem> _performanceList = null;

        public PerformanceForm(List<PerformanceItem> performanceList)
        {
            InitializeComponent();
            _performanceList = performanceList;
        }

        private void PerformanceForm_Load(object sender, EventArgs e)
        {
            var bindingList = new BindingList<PerformanceItem>(_performanceList);
            dataGridView2.DataSource = new BindingSource(bindingList, null); 

            this.BackColor = dataGridView2.BackgroundColor;
            dataGridView2.DefaultCellStyle.ForeColor = Form1.TextForeColor;
            dataGridView2.DefaultCellStyle.SelectionForeColor = Form1.TextForeColor;
            dataGridView2.DefaultCellStyle.BackColor = Color.Black;
            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Black;
            // TradeDate
            dataGridView2.Columns[(int)GridColumns.TradeDate].Width = 80;
            // Ticker
            dataGridView2.Columns[(int)GridColumns.Ticker].Width = 50;
            // Quantity
            dataGridView2.Columns[(int)GridColumns.Quantity].Width = 60;
            dataGridView2.Columns[(int)GridColumns.Quantity].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[(int)GridColumns.Quantity].DefaultCellStyle.Format = "#####";
            // Bought Price
            dataGridView2.Columns[(int)GridColumns.TradePrice].Width = 75;
            dataGridView2.Columns[(int)GridColumns.TradePrice].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[(int)GridColumns.TradePrice].DefaultCellStyle.Format = "N2";
            // Current Price
            dataGridView2.Columns[(int)GridColumns.CurrentPrice].Width = 75;
            dataGridView2.Columns[(int)GridColumns.CurrentPrice].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[(int)GridColumns.CurrentPrice].DefaultCellStyle.Format = "N2";
            // Profit
            dataGridView2.Columns[(int)GridColumns.Profit].Width = 75;
            dataGridView2.Columns[(int)GridColumns.Profit].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[(int)GridColumns.Profit].DefaultCellStyle.Format = "N2";
            // Total Profit
            dataGridView2.Columns[(int)GridColumns.TotalProfit].Width = 75;
            dataGridView2.Columns[(int)GridColumns.TotalProfit].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[(int)GridColumns.TotalProfit].DefaultCellStyle.Format = "N2";
            // DOW
            dataGridView2.Columns[(int)GridColumns.DowLevel].Width = 70;
            dataGridView2.Columns[(int)GridColumns.DowLevel].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[(int)GridColumns.DowLevel].DefaultCellStyle.Format = "N0";
            // SoldAndBought
            dataGridView2.Columns[(int)GridColumns.SoldAndBought].Width = 0;
            dataGridView2.Columns[(int)GridColumns.SoldAndBought].Visible = false;

            dataGridView2.Refresh();

            // DOW gain / loss
            decimal dowGain = Convert.ToDecimal((_performanceList.First().DowLevel - _performanceList.Last().DowLevel)) / Convert.ToDecimal(_performanceList[0].DowLevel) * 100;
            lblDowGain.Text = dowGain.ToString("N1") + "%";

            // Total Profit
            decimal totProfit = _performanceList.Sum(x => x.TotalProfit);
            lblTotalProfit.Text = totProfit.ToString("0.00");

            // Total Cost
            decimal totCost = _performanceList.Sum(x => (x.Quantity) * x.TradePrice);
            lblCost.Text = totCost.ToString("N2");
            // Current Worth
            decimal totWorth = _performanceList.Sum(x => (x.Quantity) * x.CurrentPrice);
            lblWorth.Text = totWorth.ToString("N2");
            // Buy Gain
            decimal buyGain = ((totWorth - totCost) / totCost) * 100M;
            lblPortfolioGain.Text = buyGain.ToString("N1") + "%";

            // Color big gainers and losers
            int i = 0;
            foreach (PerformanceItem pi in _performanceList)
            {
                if (pi.TotalProfit > Math.Abs(totProfit) / 12)
                {
                    dataGridView2.Rows[i].Cells[5].Style.ForeColor = Color.LightGreen;
                    dataGridView2.Rows[i].Cells[6].Style.ForeColor = Color.LightGreen;
                }
                if (pi.TotalProfit < 0)
                {
                    dataGridView2.Rows[i].Cells[5].Style.ForeColor = Color.LightPink;
                    dataGridView2.Rows[i].Cells[6].Style.ForeColor = Color.LightPink;
                }
                i++;
            }
        }
    }
}
