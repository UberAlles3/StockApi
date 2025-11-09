using SqlLayer.SQL_Models;
using StockApi.Downloads;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace StockApi
{
    public partial class MetricsForm : Form
    {
        private static StockDownloads _stockDownloads = new StockDownloads("");
        private static Analyze _analyze = new Analyze();
        private string _ticker;

        public MetricsForm(string ticker)
        {
            _ticker = ticker;
            InitializeComponent();
        }

        private void MetricsForm_Load(object sender, EventArgs e)
        {
            txtTickerList.Text = _ticker;

            
        }


        private void BindListToHistoricPriceGrid(List<SqlMetric> metrics)
        {
            var bindingList = new BindingList<SqlMetric>(metrics);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DefaultCellStyle.ForeColor = Form1.TextForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            dataGridView1.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
            dataGridView1.DataSource = source.DataSource;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].HeaderText = "Date";
            dataGridView1.Columns[3].Width = 120;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[4].Width = 120;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Refresh();
        }


        private async void btnGetAll_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            List<string> stockList = new List<string>();
            bool _tickerFound = false;

            bool networkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (networkUp == false)
            {
                MessageBox.Show("Your network connection is unavailable.");
                return;
            }

            stockList = txtTickerList.Text.Split(Environment.NewLine).ToList();
            stockList = stockList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList(); // remove blacks

            txtTickerList.Text = "";
            //builder.Append($"Ticker, Volatility, EarningsPerShare, OneYearTargetPrice, PriceBook, ProfitMargin, Dividend, 3YearPrice, 3YearPriceChange{Environment.NewLine}");
            foreach (string ticker in stockList)
            {
                _stockDownloads.stockSummary.Ticker = ticker.Substring(0, (ticker + ",").IndexOf(",")).ToUpper();

                txtTickerList.Text += $"{ticker}" + Environment.NewLine;

                // Extract the individual data values from the html
                _tickerFound = await _stockDownloads.stockSummary.GetStockData(_stockDownloads.stockSummary.Ticker);

                if (_stockDownloads.stockSummary.LastException != null)
                {
                    txtTickerList.Text = _stockDownloads.stockSummary.Error;
                    continue;
                }

                if (_stockDownloads.stockSummary.EarningsPerShareString.StringValue == "--")
                {
                    Thread.Sleep(1000);
                    _tickerFound = await _stockDownloads.stockSummary.GetStockData(_stockDownloads.stockSummary.Ticker);

                    if (_stockDownloads.stockSummary.LastException != null)
                    {
                        txtTickerList.Text = _stockDownloads.stockSummary.Error;
                        continue;
                    }
                }

                _stockDownloads.stockIncomeStatement = new StockIncomeStatement();
                bool found = await _stockDownloads.stockIncomeStatement.GetStockData(ticker);

                // Calculated PE can only be figured after both summary and finacial data is combined
                _stockDownloads.stockSummary.SetCalculatedPE(_stockDownloads);

                // get 3 year ago price
                await _stockDownloads.stockHistory.GetPriceHistoryFor3Year(ticker, _stockDownloads.stockSummary);

                if (_stockDownloads.stockHistory.HistoricDisplayList.Count > 0)
                    _stockDownloads.stockHistory.HistoricData3YearsAgo = _stockDownloads.stockHistory.HistoricDisplayList.Last();
                else
                    _stockDownloads.stockHistory.HistoricData3YearsAgo = new StockHistory.HistoricPriceData() { Ticker = _stockDownloads.stockSummary.Ticker, Price = _stockDownloads.stockSummary.PriceString.NumericValue };

                decimal percent_diff = _stockDownloads.stockSummary.PriceString.NumericValue / _stockDownloads.stockHistory.HistoricData3YearsAgo.Price - 1M;

                Analyze.AnalyzeInputs analyzeInputs = new Analyze.AnalyzeInputs();
                SetUpAnalyzeInputs(analyzeInputs);
                decimal totalMetric = _analyze.AnalyzeStockData(_stockDownloads, analyzeInputs, true);

                builder.Append($"{_stockDownloads.stockSummary.Ticker}, {_stockDownloads.stockSummary.VolatilityString.NumericValue}, {_stockDownloads.stockSummary.EarningsPerShareString.NumericValue}, {_stockDownloads.stockSummary.OneYearTargetPriceString.NumericValue}, {_stockDownloads.stockSummary.PriceBookString.NumericValue}, {_stockDownloads.stockSummary.ProfitMarginString.NumericValue}, {_stockDownloads.stockSummary.DividendString.NumericValue}, {_stockDownloads.stockStatistics.ShortInterestString.NumericValue}");
                builder.Append($", {_stockDownloads.stockHistory.HistoricData3YearsAgo.Price}, {percent_diff.ToString("0.00")},{_stockDownloads.stockSummary.YearsRangeLow.NumericValue},{_stockDownloads.stockSummary.YearsRangeHigh.NumericValue},{totalMetric}{Environment.NewLine}");
                Thread.Sleep(800);
            }
            
            txtTickerList.Text = builder.ToString();
        }

        private void SetUpAnalyzeInputs(Analyze.AnalyzeInputs analyzeInputs)
        {
            analyzeInputs.SharesOwned = 1;
            analyzeInputs.LastTradeBuySell = Analyze.BuyOrSell.Buy;
            analyzeInputs.QuantityTraded = 1;
            analyzeInputs.SharesTradedPrice = 1;
            analyzeInputs.MovementTargetPercent = 20;
        }
    }
}
