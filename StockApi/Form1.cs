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
using System.Configuration;

namespace StockApi
{
    public partial class Form1 : Form
    {
        List<Setting> _settings = new List<Setting>();
        private static StockSummary _stockSummary = new StockSummary();
        private static StockHistory _stockHistory = new StockHistory();
        private static Analyze _analyze = new Analyze();
        private static DataTable _trades = null;
        private static DataTable _tickerTrades = null;
        private static string _tradesExcelFilePath = "";
        private static DateTime _tradesImportDateTime = DateTime.Now;

        public Form1()
        {
            InitializeComponent();
            _settings = ConfigurationManager.GetSection("Settings") as List<Setting>;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ApplyStyles();

            label37.BackColor = label38.BackColor = trackBar1.BackColor;

            panel1.BackColor = Color.FromArgb(100, 0, 0, 0);
            panel1.Visible = false;

            panel2.BackColor = panel1.BackColor;
            panel2.Visible = false;

            panel3.BackColor = panel1.BackColor;
            panel3.Visible = false;

            picSpinner.Visible = false;
            //picSpinner.Left = 220;
            //picSpinner.Top = 8; 
            picSpinner.Left = (this.Width / 2) + 56;
            picSpinner.Top = this.Height / 2 - 56; 

            picUpTrend.Visible = false;
            picSidewaysTrend.Visible = false;
            picDownTrend.Visible = false;

            //picYearTrend.BackColor = Color.FromArgb(10, 0, 0, 0);

            lblCompanyNameAndTicker.Text = txtSharesTraded.Text = "";

            // temporary for testing
            txtStockTicker.Text = "INTC";
            txtSharesTraded.Text = "80";

            //txtTickerList.Text = "AB" + Environment.NewLine + "ACB" + Environment.NewLine + "AG" + Environment.NewLine;
        }

        private async void btnGetOne_click(object sender, EventArgs e)
        {
            //WebView wb = new WebView();
            //wb.Show();
            //wb.Navigate("https://finance.yahoo.com/quote/UEC?p=UEC");
            //while(wb.NavigationComplete == false)
            //{
            //    Application.DoEvents();
            //}

            //string html = wb.Html;

            //return;

            if (string.IsNullOrEmpty(txtStockTicker.Text))
            {
                MessageBox.Show("Enter a valid stock ticker.");
                return;
            }


            // Trades
            _tradesExcelFilePath = _settings.Find(x => x.Name == "ExcelTradesPath").Value;
            DateTime tradesExcelFileDateTime = System.IO.File.GetLastWriteTime(_tradesExcelFilePath);
            if (_trades == null || tradesExcelFileDateTime > _tradesImportDateTime)
            {
                _trades = (new ExcelManager()).ImportTrades(_settings.Find(x => x.Name == "ExcelTradesPath").Value);
                _trades = _trades.Rows.Cast<DataRow>().Where(row => row.ItemArray[0].ToString().Trim() != "").CopyToDataTable();
                _trades.Columns[0].DataType = System.Type.GetType("System.DateTime");
                _tradesImportDateTime = DateTime.Now;
            }

            PreSummaryWebCall(); // Sets the form display while the request is executing

            // Extract the individual data values from the html
            bool found = await _stockSummary.GetSummaryData(txtStockTicker.Text);

            if(found)
            {
                // Get some price history. Todays price will be replaced with summary data's latest price
                List<StockHistory.HistoricData> historicDisplayList = await _stockHistory.GetPriceHistoryForTodayWeekMonthYear(txtStockTicker.Text, _stockSummary);
                // await _stockHistory.GetPriceHistoryForTodayWeekMonthYear(txtStockTicker.Text, _stockSummary);

                // bind data list to grid control
                var bindingList = new BindingList<StockHistory.HistoricData>(historicDisplayList);
                var source = new BindingSource(bindingList, null);
                dataGridView1.DefaultCellStyle.ForeColor = Color.LightSteelBlue;
                dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
                dataGridView1.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
                dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
                dataGridView1.DataSource = source.DataSource;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "Date";
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                dataGridView1.Columns[2].DefaultCellStyle.Format = "N2";
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                dataGridView1.Refresh();

                _tickerTrades = null;
                var tickertrades = _trades.AsEnumerable().Where(x => x[4].ToString().ToLower() == txtStockTicker.Text.ToLower());
                if (_trades.Rows.Count > 0 && tickertrades.Count() > 0)
                {
                    _tickerTrades = tickertrades.CopyToDataTable();

                    // bind data list to trades grid control
                    BindingSource tradeSource = new BindingSource();
                    tradeSource.DataSource = _tickerTrades;
                    dataGridView2.DefaultCellStyle.ForeColor = Color.LightSteelBlue;
                    dataGridView2.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
                    dataGridView2.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
                    dataGridView2.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
                    dataGridView2.DataSource = tradeSource.DataSource;
                    dataGridView2.Columns[0].HeaderText = "Date";
                    //dataGridView2.Columns[1].ValueType = System.Type.GetType("System.DateTime");
                    dataGridView2.Columns[1].Visible = false;
                    dataGridView2.Columns[2].HeaderText = "Buy/Sell";
                    dataGridView2.Columns[2].Width = 60;
                    dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dataGridView2.Columns[3].HeaderText = "Quan.";
                    dataGridView2.Columns[3].Width = 60;
                    dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridView2.Columns[3].DefaultCellStyle.Format = "#####";
                    dataGridView2.Columns[4].Visible = false; // Hide ticker

                    dataGridView2.Columns[5].HeaderText = "Price";
                    dataGridView2.Columns[5].Width = 77;
                    dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridView2.Columns[5].DefaultCellStyle.Format = "N2";
                    dataGridView2.Columns[6].HeaderText = "Tot. Shares";
                    dataGridView2.Columns[6].Width = 68;
                    dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridView2.Columns[6].DefaultCellStyle.Format = "#####";

                    dataGridView2.Columns[7].Visible = false;
                    dataGridView2.Columns[8].Visible = false;
                    dataGridView2.Columns[9].Visible = false;

                    //dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
                    dataGridView2.Refresh();
                }
                else
                {
                    if (_tickerTrades != null)
                        dataGridView2.Rows.Clear();
                    else
                        dataGridView2.DataSource = null;
                    //dataGridView2.Rows.Clear();
                    dataGridView2.Refresh();
                }

                // Trends
                _stockHistory.SetTrends();
                pic3YearTrend.Image = _stockHistory.ThreeYearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.ThreeYearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
                picYearTrend.Image = _stockHistory.YearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.YearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
                picMonthTrend.Image = _stockHistory.MonthTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.MonthTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
                picWeekTrend.Image = _stockHistory.WeekTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.WeekTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }

            PostSummaryWebCall(); // displays the data returned

        }

        private async void btnGetAll_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            
            List<string> stockList = new List<string>();
            stockList = txtTickerList.Text.Split(Environment.NewLine).ToList();
            stockList = stockList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList(); // remove blacks

            txtTickerList.Text = ".";
            builder.Append($"Ticker, Volatility, EarningsPerShare, OneYearTargetPrice, PriceBook, ProfitMargin, Dividend{Environment.NewLine}");
            foreach (string ticker in stockList)
            {
                _stockSummary.Ticker = ticker.Substring(0, (ticker + ",").IndexOf(",")).ToUpper();

                txtTickerList.Text += ".";

                // Extract the individual data values from the html
                await _stockSummary.GetSummaryData(_stockSummary.Ticker);
                await _stockSummary.GetShortInterest(_stockSummary.Ticker);

                builder.Append($"{_stockSummary.Ticker}, {_stockSummary.Volatility}, {_stockSummary.EarningsPerShare}, {_stockSummary.OneYearTargetPrice}, {_stockSummary.PriceBook}, {_stockSummary.ProfitMargin}, {_stockSummary.Dividend}, {_stockSummary.ShortInterest}{Environment.NewLine}");
            }
            txtTickerList.Text = builder.ToString();
        }

        private async void btnGet3YearTrend_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            List<StockHistory.HistoricData> historicDataList = new List<StockHistory.HistoricData>();

            List<string> stockList = new List<string>();
            stockList = txtTickerList.Text.Split(Environment.NewLine).ToList();
            stockList = stockList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList(); // remove blacks

            txtTickerList.Text = ".";
            foreach (string ticker in stockList)
            {
                _stockSummary.Ticker = ticker.Substring(0, (ticker + ",").IndexOf(",")).ToUpper();

                txtTickerList.Text += "."; // show progress 

                // Get today's price 
                await _stockSummary.GetSummaryData(_stockSummary.Ticker, verbose: false); // only get price

                // Extract the individual data values from the html
                /////// Get price history for 3 years ago to determine long trend
                historicDataList = await _stockHistory.GetHistoricalDataForDateRange(_stockSummary.Ticker, DateTime.Now.AddYears(-3).AddDays(-1), DateTime.Now.AddYears(-3).AddDays(4));
                if (historicDataList.Count > 0)
                    _stockHistory.HistoricData3YearsAgo = historicDataList.First();
                else
                    _stockHistory.HistoricData3YearsAgo = _stockHistory.HistoricDataYearAgo;

                float percent_diff = _stockSummary.Price / _stockHistory.HistoricData3YearsAgo.Price - 1;

                //builder.Append($"{_stockSummary.Ticker}, {_stockHistory.HistoricData3YearsAgo.Price}, {_stockSummary.Price}, {percent_diff}{Environment.NewLine}");
                builder.Append($"{_stockHistory.HistoricData3YearsAgo.Price}, {percent_diff.ToString("0.00")}{Environment.NewLine}");
            }
            txtTickerList.Text = builder.ToString();
        }

        private void PreSummaryWebCall()
        {
            _stockSummary = new StockSummary(); // set values to zero
            pic3YearTrend.Image = picSidewaysTrend.Image;
            picYearTrend.Image = picSidewaysTrend.Image;
            picMonthTrend.Image = picSidewaysTrend.Image; 
            picWeekTrend.Image = picSidewaysTrend.Image; 

            btnGetOne.Enabled = false;
            panel1.Visible = panel2.Visible = panel3.Visible = false;
            btnGetShortInterest.Visible = true;
            picSpinner.Visible = true;
            Cursor.Current = Cursors.WaitCursor;
        }

        private void PostSummaryWebCall()
        {
            try
            {
                btnGetOne.Enabled = true;
                lblCompanyNameAndTicker.Text = _stockSummary.CompanyName;
                lblPrice.Text = _stockSummary.Price.ToString("####.##");
                lblVolatility.Text = _stockSummary.VolatilityString;
                lblEPS.Text = _stockSummary.EarningsPerShareString;
                lblEPS.ForeColor = _stockSummary.EPSColor;
                lblPriceBook.Text = _stockSummary.PriceBook.ToString();
                lblPriceBook.ForeColor = _stockSummary.PriceBookColor;
                lblDividend.Text = _stockSummary.Dividend.ToString() + "%";
                lblDividend.ForeColor = _stockSummary.DividendColor;
                lblProfitMargin.Text = _stockSummary.ProfitMargin.ToString() + "%";
                lblProfitMargin.ForeColor = _stockSummary.ProfitMarginColor;
                lblOneYearTarget.Text = _stockSummary.OneYearTargetPriceString;
                lblOneYearTarget.ForeColor = _stockSummary.OneYearTargetColor;
                panel1.Visible = panel2.Visible = panel3.Visible = true;
                picSpinner.Visible = false;
                Cursor.Current = Cursors.Default;

                // Set some analyze form fields for later use
                //PersonalStock personalStock = new PersonalStock();
                //PersonalStock.PersonalStockData personalStockData = personalStock.GetPersonalDataForTicker(_stockSummary.Ticker);
                if(_tickerTrades != null && _tickerTrades.Rows.Count > 0)
                {
                    DataRow lastRow = _tickerTrades.Rows[_tickerTrades.Rows.Count - 1];
                    txtSharesOwned.Text = lastRow.ItemArray[6].ToString();      // Total Shares
                    txtSharesTraded.Text = lastRow.ItemArray[3].ToString();     // Quan. Traded
                    txtSharesTradePrice.Text = lastRow.ItemArray[5].ToString(); // Price
                    if (lastRow.ItemArray[2].ToString().Trim().ToLower() == "buy")
                    {
                        radBuy.Checked = true;
                        radSell.Checked = false;
                    }
                    else
                    {
                        radBuy.Checked = false;
                        radSell.Checked = true;
                    }

                    // Find Min,Max trade price
                    string min = _tickerTrades.AsEnumerable()
                        .Min(row => row[5])
                        .ToString();
                    string max = _tickerTrades.AsEnumerable()
                        .Max(row => row[5])
                        .ToString();
                    decimal minval = Convert.ToDecimal(min);
                    decimal maxval = Convert.ToDecimal(max);
                    decimal startVal = Convert.ToDecimal(_tickerTrades.AsEnumerable().First().ItemArray[5]);
                    decimal endVal   = Convert.ToDecimal(_tickerTrades.AsEnumerable().Last().ItemArray[5]);
                    if (_tickerTrades.Rows.Count > 4)
                    {
                        startVal = (startVal + Convert.ToDecimal(_tickerTrades.Rows[1].ItemArray[5])) / 2.0M;
                        endVal = (endVal + Convert.ToDecimal(_tickerTrades.Rows[_tickerTrades.Rows.Count - 2].ItemArray[5])) / 2.0M;
                    }

                    decimal slideVal = ((endVal - startVal) / _tickerTrades.Rows.Count) *.9M;
                    decimal currentSlide = startVal;
                    decimal currentPrice;
                    int i = 0;
                    Color lastColor = Color.Black;
                    foreach (DataRow r in _tickerTrades.Rows)
                    {
                        // Color Buy and Sells
                        if (r.ItemArray[2].ToString().Trim().ToLower() == "buy")
                            dataGridView2.Rows[i].Cells[2].Style.ForeColor = Color.Lime;
                        else
                            dataGridView2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;

                        // Color high price low price
                        currentSlide += slideVal;
                        currentPrice = Convert.ToDecimal(r.ItemArray[5]);
                        if (currentPrice > maxval * .98M || currentPrice > currentSlide * 1.1M)
                        {
                            if (lastColor != Color.Lime)
                            {
                                dataGridView2.Rows[i].Cells[5].Style.ForeColor = Color.Lime;
                                lastColor = Color.Lime;
                            }
                            if (currentPrice > maxval * .98M)
                            {
                                dataGridView2.Rows[i].Cells[5].Style.ForeColor = Color.Lime;
                                lastColor = Color.Lime;
                            }
                        }
                        if (currentPrice < minval * 1.02M || currentPrice < currentSlide * .9M)
                        {
                            if (lastColor != Color.Red)
                            {
                                dataGridView2.Rows[i].Cells[5].Style.ForeColor = Color.Red;
                                lastColor = Color.Red;
                            }
                            if (currentPrice < minval * 1.02M)
                            {
                                dataGridView2.Rows[i].Cells[5].Style.ForeColor = Color.Red;
                                lastColor = Color.Red;
                            }
                        }

                        i++;
                    }
                }
                else
                {
                    txtSharesOwned.Text = "1";
                    txtSharesTraded.Text = "1";
                    txtSharesTradePrice.Text = _stockSummary.Price.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message} {e.InnerException} {e.StackTrace}");
            }
        }
        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            Analyze.AnalyzeInputs analyzeInputs = new Analyze.AnalyzeInputs();

            analyzeInputs.SharesOwned = Convert.ToInt32(txtSharesOwned.Text);
            analyzeInputs.LastTradeBuySell = radBuy.Checked ? Analyze.BuyOrSell.Buy : Analyze.BuyOrSell.Sell;
            analyzeInputs.QuantityTraded = Convert.ToInt32(txtSharesTraded.Text);
            analyzeInputs.SharesTradedPrice = Convert.ToSingle(txtSharesTradePrice.Text);
            analyzeInputs.MovementTargetPercent = Convert.ToInt32(txtMovementTargetPercent.Text);
            analyzeInputs.EconomyHealth = trackBar1.Value;

            _analyze.AnalyzeStockData(_stockSummary, _stockHistory, analyzeInputs);
            txtAnalysisOutput.Text = _analyze.AnalysisMetricsOutputText;

            lblBuyQuantity.Text = _analyze.BuyQuantity.ToString();
            lblBuyPrice.Text = _analyze.BuyPrice.ToString();
            lblSellQuantity.Text = _analyze.SellQuantity.ToString();
            lblSellPrice.Text = _analyze.SellPrice.ToString();
        }
        
        private void ApplyStyles()
        {
            Color forecolor = Color.FromArgb(220, 235, 245);

            foreach (Control control in this.Controls)
            {
                if (control.GetType() == typeof(System.Windows.Forms.Label))
                {
                    control.ForeColor = forecolor;
                }
            }
            foreach (Control control in panel1.Controls)
            {
                if (control.GetType() == typeof(System.Windows.Forms.Label))
                {
                    control.ForeColor = forecolor;
                }
            }
            foreach (Control control in panel2.Controls)
            {
                if (control.GetType() == typeof(System.Windows.Forms.Label))
                {
                    control.ForeColor = forecolor;
                }
            }
        }

        private async void btnGetShortInterest_Click(object sender, EventArgs e)
        {
            btnGetShortInterest.Visible = false;
            lblShortInterest.Text = "...";

            bool found = await _stockSummary.GetShortInterest(txtStockTicker.Text);

            if (found)
            {
                lblShortInterest.Text = _stockSummary.ShortInterest.ToString() + "%";
                lblShortInterest.ForeColor = _stockSummary.ShortInterestColor;
            }
        }

    }
}
