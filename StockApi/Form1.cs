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

            PreSummaryWebCall(); // Sets the form display while the request is executing

            // Extract the individual data values from the html
            bool found = await _stockSummary.GetSummaryData(txtStockTicker.Text);

            if(found)
            {
                // Get some price history. Todays price will be replaced with summary data's latest price
                List<StockHistory.HistoricData> historicDisplayList = await _stockHistory.GetPriceHistoryForTodayWeekMonthYear(txtStockTicker.Text, _stockSummary);

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

                // Trends
                _stockHistory.SetTrends();
                pic3YearTrend.Image = _stockHistory.ThreeYearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.ThreeYearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
                picYearTrend.Image = _stockHistory.YearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.YearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
                picMonthTrend.Image = _stockHistory.MonthTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.MonthTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
                picWeekTrend.Image = _stockHistory.WeekTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.WeekTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }

            PostSummaryWebCall(); // displays the data returned

            if (_trades == null)
            {
                _trades = (new ExcelManager()).ImportTrades(_settings.Find(x => x.Name == "ExcelTradesPath").Value);
                _trades = _trades.Rows.Cast<DataRow>().Where(row => row.ItemArray[0].ToString().Trim() != "").CopyToDataTable();
            }


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

                builder.Append($"{_stockSummary.Ticker}, {_stockSummary.Volatility}, {_stockSummary.EarningsPerShare}, {_stockSummary.OneYearTargetPrice}, {_stockSummary.PriceBook}, {_stockSummary.ProfitMargin}, {_stockSummary.Dividend}{Environment.NewLine}");
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
                PersonalStock personalStock = new PersonalStock();
                PersonalStock.PersonalStockData personalStockData = personalStock.GetPersonalDataForTicker(_stockSummary.Ticker);
                if(personalStockData != null)
                {
                    txtSharesOwned.Text = personalStockData.SharesOwned.ToString();
                }
                else
                {
                    txtSharesOwned.Text = "1";
                }
                txtSharesTradePrice.Text = _stockSummary.Price.ToString();
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

        private void importTradesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ExcelManager excelManager = new ExcelManager();
            //excelManager.Import("C:\\Users\\Uber\\Desktop\\Stocks 2023_w_Trades.xlsx");


        }
    }
}
