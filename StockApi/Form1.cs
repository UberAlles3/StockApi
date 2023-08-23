﻿using Newtonsoft.Json;
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
using CommonClasses;

namespace StockApi
{
    public partial class Form1 : Form
    {
        private static StockSummary.StockData _stockData = new StockSummary.StockData();
        private static Analyze.AnalyzeResults _analyzeResults = new Analyze.AnalyzeResults();

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
            picSidewaysTrend.Visible = false;
            picDownTrend.Visible = false;

            lblTicker.Text = "";

            // temporary for testing
            txtStockTicker.Text = "intc";
            txtTickerList.Text = "AB" + Environment.NewLine + "ACB" + Environment.NewLine + "AG" + Environment.NewLine;

            // for testing
            //StockSummary.Ticker = "INTC";
            //Analyze.PersonalStockData personalStockData = Analyze.PreFillAnalyzeFormData();
            //txtSharesOwned.Text = personalStockData.SharesOwned.ToString();

            //Analyze.AnalyzeResults analyzeResults = Analyze.AnalyzeStockData();
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

            // Get some price history
            List<StockHistory.HistoricData> historicDisplayList = await StockHistory.GetPriceHistoryForTodayWeekMonthYear(txtStockTicker.Text);

            // bind data list to grid control
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
            picYearTrend.Image = StockHistory.YearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : StockHistory.YearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            picMonthTrend.Image = StockHistory.MonthTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : StockHistory.MonthTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image; 
            picWeekTrend.Image = StockHistory.WeekTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : StockHistory.WeekTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
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
            lblDividend.Text = stockData.Dividend;
            lblEstimatedReturn.Text = stockData.EstimatedReturn.ToString() + "%";
            lblEstimatedReturn.ForeColor = YahooFinance.EstReturnColor;
            lblOneYearTarget.Text = stockData.OneYearTargetPrice;
            lblOneYearTarget.ForeColor = YahooFinance.OneYearTargetColor;
            panel1.Visible = true;
            panel2.Visible = true;
            picSpinner.Visible = false;
            Cursor.Current = Cursors.Default;

            // Analyze form fields
            Analyze.PersonalStockData personalStockData = Analyze.PreFillAnalyzeFormData();
            txtSharesOwned.Text = personalStockData.SharesOwned.ToString();

        }

        /////////////////////////////////////////
        ///            Analyze Stock
        ////////////////////////////////////////
        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            // combine trends with
            // one year target
            // EPS
            // Volatility
            // Fair Value
            // Estimated Return %
            // Should we read in excel file?
            Analyze.AnalyzeInputs analyzeInputs = new Analyze.AnalyzeInputs();
            analyzeInputs.SharesOwned = Convert.ToInt32(txtSharesOwned.Text);
            analyzeInputs.LastTradeBuySell = radBuy.Checked ? Analyze.BuyOrSell.Buy : Analyze.BuyOrSell.Sell;
            analyzeInputs.SharesTraded = Convert.ToInt32(txtSharesTraded.Text);
            analyzeInputs.MovementTargetPercent = Convert.ToInt32(txtMovementTargetPercent.Text);

            _analyzeResults = Analyze.AnalyzeStockData(analyzeInputs);
            txtAnalysisOutput.Text = _analyzeResults.AnalysisOutput;
        }
    }
}
