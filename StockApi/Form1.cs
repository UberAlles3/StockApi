using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StockApi
{
    public partial class Form1 : Form
    {
        private static StockSummary.StockData _stockData = new StockSummary.StockData();
        
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(100, 0, 0, 0);
            panel1.Visible = false;

            panel2.BackColor = Color.FromArgb(100, 0, 0, 0);
            panel2.Visible = false;

            picSpinner.Visible = false;
            picSpinner.Left = 220;
            picSpinner.Top = 8; 

            picUpTrend.Visible = false;
            picDownTrend.Visible = false;

            lblTicker.Text = "";

            // temporary for testing
            txtStockTicker.Text = "intc";
            txtTickerList.Text = "AB" + Environment.NewLine + "ACB" + Environment.NewLine + "AG" + Environment.NewLine;
        }

        private async void btnGetOne_click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStockTicker.Text))
            {
                MessageBox.Show("Enter a valid stock ticker.");
                return;
            }

            PreWebCall(); // Sets the form display while the request is executing


            // Execute the request to get html from Yahoo Finance
            string html = await StockSummary.GetHtmlForTicker(txtStockTicker.Text);
            // Extract the individual data values from the html
            StockSummary.ExtractDataFromHtml(_stockData, html);

            /////// Get price history, today, week ago, month ago to determine short trend
            List<StockHistory.HistoricData> historicDataList = await StockHistory.GetHistoricalDataForDateRange(txtStockTicker.Text, DateTime.Now.AddMonths(-1), DateTime.Now);

            // Today will be the last in the list
            StockHistory.HistoricDataToday = historicDataList.Last();

            // Last Week
            DateTime findDate = GetMondayIfWeeekend(DateTime.Now.AddDays(-7).Date);
            StockHistory.HistoricDataWeekAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.AddDays(1));

            // Last Month (really 31 days ago)
            findDate = GetMondayIfWeeekend(DateTime.Now.AddDays(-31).Date);
            StockHistory.HistoricDataMonthAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.Date.AddDays(1));

            /////// Get price history for a year ago to determine long trend
            historicDataList = await StockHistory.GetHistoricalDataForDateRange(txtStockTicker.Text, DateTime.Now.AddYears(-1), DateTime.Now.AddYears(-1).AddDays(2));
            // Last Year
            findDate = GetMondayIfWeeekend(DateTime.Now.AddYears(-1).Date);
            StockHistory.HistoricDataYearAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.Date.AddDays(1));

            List<StockHistory.HistoricData> historicDisplayList = new List<StockHistory.HistoricData>();
            historicDisplayList.Add(StockHistory.HistoricDataToday);
            historicDisplayList.Add(StockHistory.HistoricDataWeekAgo);
            historicDisplayList.Add(StockHistory.HistoricDataMonthAgo);
            historicDisplayList.Add(StockHistory.HistoricDataYearAgo);

            var bindingList = new BindingList<StockHistory.HistoricData>(historicDisplayList);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DefaultCellStyle.ForeColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            dataGridView1.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
            dataGridView1.DataSource = source.DataSource;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Date";
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Refresh();

            // Trends
            StockHistory.SetTrends();
            picYearTrend.Image = StockHistory.YearTrend ? picUpTrend.Image : picDownTrend.Image;
            picMonthTrend.Image = StockHistory.MonthTrend ? picUpTrend.Image : picDownTrend.Image;
            picWeekTrend.Image = StockHistory.WeekTrend ? picUpTrend.Image : picDownTrend.Image;

            PostWebCall(_stockData); // displays the data returned
        }

        private static DateTime GetMondayIfWeeekend(DateTime theDate)
        {
            if (theDate.DayOfWeek == DayOfWeek.Saturday)
                theDate = theDate.AddDays(2); // return Monday

            if (theDate.DayOfWeek == DayOfWeek.Sunday)
            {
                theDate = theDate.AddDays(1); // return Monday
            }

            return theDate;
        }

        private async void btnGetAll_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            
            List<string> stockList = new List<string>();
            stockList = txtTickerList.Text.Split(Environment.NewLine).ToList();
            stockList = stockList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList(); // remove blacks

            txtTickerList.Text = ".";
            foreach (string ticker in stockList)
            {
                _stockData.Ticker = ticker.Substring(0, (ticker + ",").IndexOf(",")).ToUpper();

                string html = await StockSummary.GetHtmlForTicker(_stockData.Ticker);
                txtTickerList.Text += ".";

                // Extract the individual data values from the html
                StockSummary.ExtractDataFromHtml(_stockData, html);

                builder.Append($"{_stockData.Ticker}, {_stockData.Volatility}, {_stockData.EarningsPerShare}, {_stockData.OneYearTargetPrice}, {_stockData.FairValue}, {_stockData.EstimatedReturn}{Environment.NewLine}");
            }
            txtTickerList.Text = builder.ToString();
        }

        private void PreWebCall()
        {
            btnGetOne.Enabled = false;
            panel1.Visible = false;
            panel2.Visible = false;
            picSpinner.Visible = true;
            Cursor.Current = Cursors.WaitCursor;
        }
        private void PostWebCall(StockSummary.StockData stockData)
        {
            btnGetOne.Enabled = true;
            lblTicker.Text = stockData.CompanyName;
            lblVolatility.Text = stockData.Volatility;
            lblEPS.Text = stockData.EarningsPerShare;
            lblEPS.ForeColor = YahooFinance.EPSColor;
            lblFairValue.Text = stockData.FairValue.ToString();
            lblFairValue.ForeColor = YahooFinance.FairValueColor;
            lblPrice.Text = stockData.Price.ToString();
            panel1.Visible = true;
            panel2.Visible = true;
            picSpinner.Visible = false;
            Cursor.Current = Cursors.Default;
        }
    }
}
