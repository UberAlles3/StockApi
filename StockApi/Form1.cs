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
// 2dadcc0b -- API key

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
            picSpinner.Left = 220; // panel1.Left + panel1.Width/2 - 50;
            picSpinner.Top = 8;    //panel1.Top + panel1.Height / 2 - 30;

            picUpTrend.Visible = false;
            picDownTrend.Visible = false;

            //picYearTrend.BackColor = Color.FromArgb(100, 0, 0, 0);

            lblTicker.Text = "";
            txtTickerList.Text = "AB" + Environment.NewLine + "ACB" + Environment.NewLine + "AG" + Environment.NewLine + "AM" + Environment.NewLine;
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

            // Get price history, Today, week ago, month ago, year ago to determine long and short trend
            List<StockHistory.HistoricData> historicData = await StockHistory.GetHistoricalDataForDate(txtStockTicker.Text,DateTime.Now.AddMonths(-1), DateTime.Now);
            StockHistory.HistoricData historicDataToday = historicData.Last();
            StockHistory.HistoricData historicDataWeekAgo = historicData.Find(x => x.PriceDate.Date == DateTime.Now.AddDays(-7).Date);

            DateTime monthAgo = DateTime.Now.AddMonths(-1).Date;
            if (monthAgo.DayOfWeek == DayOfWeek.Saturday)
                monthAgo = monthAgo.AddDays(-1);
            if (monthAgo.DayOfWeek == DayOfWeek.Sunday)
                monthAgo = monthAgo.AddDays(-2);

            StockHistory.HistoricData historicDataMonthAgo = historicData.Find(x => x.PriceDate.Date == monthAgo.Date);

            historicData = await StockHistory.GetHistoricalDataForDate(txtStockTicker.Text, DateTime.Now.AddYears(-1), DateTime.Now.AddYears(-1).AddDays(2));
            DateTime yearAgo = DateTime.Now.AddYears(-1).Date;
            if (yearAgo.DayOfWeek == DayOfWeek.Saturday)
                yearAgo = yearAgo.AddDays(-1); // Get Friday
            if (yearAgo.DayOfWeek == DayOfWeek.Sunday)
                yearAgo = yearAgo.AddDays(1); // Get Monday

            StockHistory.HistoricData historicDataYearAgo = historicData.Find(x => x.PriceDate.Date == yearAgo.Date);

            List<StockHistory.HistoricData> historicDataList = new List<StockHistory.HistoricData>();
            historicDataList.Add(historicDataToday);
            historicDataList.Add(historicDataWeekAgo);
            historicDataList.Add(historicDataMonthAgo);
            historicDataList.Add(historicDataYearAgo);

            var bindingList = new BindingList<StockHistory.HistoricData>(historicDataList);
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

            // Trend
            if (historicDataToday.Price < historicDataYearAgo.Price) // year
                picYearTrend.Image = picDownTrend.Image;
            else
                picYearTrend.Image = picUpTrend.Image;

            if (historicDataToday.Price < historicDataMonthAgo.Price) // month
                picMonthTrend.Image = picDownTrend.Image;
            else
                picMonthTrend.Image = picUpTrend.Image;

            if (historicDataToday.Price < historicDataWeekAgo.Price) // week
                picWeekTrend.Image = picDownTrend.Image;
            else
                picWeekTrend.Image = picUpTrend.Image;


            PostWebCall(_stockData); // displays the data returned
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

                builder.Append($"{_stockData.Ticker}, {_stockData.Beta}, {_stockData.EarningsPerShare}, {_stockData.OneYearTargetPrice}, {_stockData.FairValue}, {_stockData.EstimatedReturn}{Environment.NewLine}");
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
            lblBeta.Text = stockData.Beta;
            lblEPS.Text = stockData.EarningsPerShare;
            lblPrice.Text = stockData.Price.ToString();
            panel1.Visible = true;
            panel2.Visible = true;
            picSpinner.Visible = false;
            Cursor.Current = Cursors.Default;
        }

        //private async void btnGetHistory_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrEmpty(txtStockTicker.Text))
        //    {
        //        MessageBox.Show("Enter a valid stock ticker.");
        //        return;
        //    }

        //    HistoricData historicData = await GetHistoricalDataForDate(DateTime.Now);

        //}
    }
}
