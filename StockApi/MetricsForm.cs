using SqlLayer.SQL_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using YahooLayer;
using SqlLayer;

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
            comboBox1.Items.Add("Lastest");
            comboBox1.Items.Add("Last 2 Months");
            comboBox1.Items.Add("Last Year");
            comboBox1.SelectedIndex = 1;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<SqlMetric> metrics = new List<SqlMetric>();
            List<SqlMetric> bigMovers = new List<SqlMetric>();
            string ticker = txtTicker.Text;

            txtTicker.Text = ticker = ticker.ToUpper();

            if (ticker.Trim() == "")
                ticker = null;
            else
                ticker = ticker.ToUpper();

            // Get All the metric rows
            SqlCrudOperations sqlCrudOperations = new SqlCrudOperations();

            if (comboBox1.SelectedIndex == 0)
                metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddDays(-1), ticker);
            if (comboBox1.SelectedIndex == 1)
                metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddMonths(-1), ticker);
            if (comboBox1.SelectedIndex == 2)
                metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddMonths(-11), ticker);

            metrics = metrics.OrderBy(x => x.Ticker).ThenBy(x => x.Year).ThenBy(x => x.Month).ToList();
            bigMovers = GetBigMovers(metrics);

            if (chkBigChanges.Checked == true)
                metrics = bigMovers;

            BindListToMetricGrid(metrics);
            ColorGrid(metrics);
        }

        private void BindListToMetricGrid(List<SqlMetric> metrics)
        {
            var bindingList = new BindingList<SqlMetric>(metrics);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DefaultCellStyle.ForeColor = Form1.TextForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            dataGridView1.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView1.DataSource = source.DataSource;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Ticker";
            dataGridView1.Columns[1].Width = 60;
            dataGridView1.Columns[2].HeaderText = "Year";
            dataGridView1.Columns[2].Width = 50;
            dataGridView1.Columns[3].HeaderText = "Month";
            dataGridView1.Columns[3].Width = 50;
            
            dataGridView1.Columns[4].HeaderText = "Price Trend";
            dataGridView1.Columns[4].Width = 55;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[4].DefaultCellStyle.Format = "N3";
            
            dataGridView1.Columns[5].HeaderText = "Earnings Per Share";
            dataGridView1.Columns[5].Width = 55;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[5].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[6].HeaderText = "Target Price";
            dataGridView1.Columns[6].Width = 55;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[6].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[7].HeaderText = "Price Book";
            dataGridView1.Columns[7].Width = 55;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[7].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[8].HeaderText = "Dividend";
            dataGridView1.Columns[8].Width = 60;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[8].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[9].HeaderText = "Profit Margin";
            dataGridView1.Columns[9].Width = 55;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[9].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[10].HeaderText = "Revenue";
            dataGridView1.Columns[10].Width = 60;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[10].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[11].HeaderText = "Profit";
            dataGridView1.Columns[11].Width = 55;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[11].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[12].HeaderText = "Basic EPS";
            dataGridView1.Columns[12].Width = 55;
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[12].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[13].HeaderText = "Cash Debt";
            dataGridView1.Columns[13].Width = 55;
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[13].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[14].HeaderText = "Valuation";
            dataGridView1.Columns[14].Width = 60;
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[14].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[15].HeaderText = "Cash Flow";
            dataGridView1.Columns[15].Width = 55;
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[15].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[16].HeaderText = "Final Metric";
            dataGridView1.Columns[16].Width = 55;
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[16].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[17].HeaderText = "Update Date";
            dataGridView1.Columns[17].Width = 80;
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[17].DefaultCellStyle.Format = "MM/dd/yyyy";

            dataGridView1.Refresh();
        }

        private List<SqlMetric> GetBigMovers(List<SqlMetric> metrics)
        {
            List<SqlMetric> bigMovers = new List<SqlMetric>();

            string ticker = "";
            double previous = 0;
            SqlMetric p = null;
            foreach (SqlMetric r in metrics)
            {
                if (ticker != r.Ticker)
                {
                    p = r;
                    ticker = r.Ticker;
                    previous = r.FinalMetric;
                }

                if (r.FinalMetric > previous * 1.032)
                {
                    bigMovers.Add(p);
                    bigMovers.Add(r);
                }
                else if (r.FinalMetric < previous * .97)
                {
                    bigMovers.Add(p);
                    bigMovers.Add(r);
                }
            }

            return bigMovers;
        }

        private void ColorGrid(List<SqlMetric> metrics)
        {
            string ticker = "";
            double previous = 0;
            int i = 0;
            foreach (SqlMetric r in metrics)
            {
                if (ticker != r.Ticker)
                {
                    ticker = r.Ticker;
                    previous = r.FinalMetric;
                }

                if (r.FinalMetric > previous * 1.032)
                {
                    dataGridView1.Rows[i].Cells[16].Style.ForeColor = Color.Lime;
                }
                else if (r.FinalMetric > previous * 1.01)
                {
                    dataGridView1.Rows[i].Cells[16].Style.ForeColor = Color.FromArgb(205, 242, 202);
                }
                else if (r.FinalMetric < previous * .97)
                {
                    dataGridView1.Rows[i].Cells[16].Style.ForeColor = Color.Red;
                }
                else if (r.FinalMetric < previous * .99)
                {
                    dataGridView1.Rows[i].Cells[16].Style.ForeColor = Color.FromArgb(242, 202, 202); 
                }

                i++;
            }
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
