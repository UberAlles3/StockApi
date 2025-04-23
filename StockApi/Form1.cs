using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace StockApi
{
    public partial class Form1 : Form
    {
        List<Setting> _settings = new List<Setting>();
        private static bool _tickerFound = false;
        private static StockSummary _stockSummary = new StockSummary();
        private static StockFinancials _stockFinancials = new StockFinancials();
        private static StockHistory _stockHistory = new StockHistory();

        // Markets
        MarketData _marketData;
        public MarketData Market_SandP;
        public MarketData Market_Dow;
        public MarketData Market_Nasdaq;

        private static Analyze _analyze = new Analyze();
        public static DataTable TickerTradesDataTable = null;

        private static string _excelFilePath = "";
        private static DateTime _tradesImportDateTime = DateTime.Now.AddYears(-2);
        private static DateTime _positionsImportDateTime = DateTime.Now.AddYears(-2);
        private static DataTable _positionsDataTable = null;
        private static DataTable _tradesDataTable = null;
        public static DataTable PositionsDataTable 
        {
            get 
            {
                DateTime excelFileDateTime = System.IO.File.GetLastWriteTime(_excelFilePath);
                if (excelFileDateTime > _positionsImportDateTime)
                {
                    _positionsDataTable = (new ExcelManager()).ImportTrades(_excelFilePath, 0, 0);
                    _positionsImportDateTime = DateTime.Now; // Update when the last import took place
                }
                return _positionsDataTable;
            }
            set => _positionsDataTable = value; 
        }
        public static DataTable TradesDataTable 
        {
            get
            {
                DateTime excelFileDateTime = System.IO.File.GetLastWriteTime(_excelFilePath);
                if (excelFileDateTime > _tradesImportDateTime)
                {
                    _tradesDataTable = (new ExcelManager()).ImportTrades(_excelFilePath, 1, 40);
                    _tradesDataTable = _tradesDataTable.Rows.Cast<DataRow>().Where(row => row.ItemArray[0].ToString().Trim() != "").CopyToDataTable();
                    _tradesImportDateTime = DateTime.Now; // Update when the last import took place
                }
                return _tradesDataTable;
            }
            set => _tradesDataTable = value; 
        }

        public Form1()
        {
            InitializeComponent();
            _settings = ConfigurationManager.GetSection("Settings") as List<Setting>;
            _marketData = new MarketData();
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

            lblCompanyNameAndTicker.Text = txtSharesTraded.Text = "";

            // temporary for testing
            txtStockTicker.Text = "INTC";
            txtSharesTraded.Text = "80";

            // Revenue year labels TTM, 2 years ago, 4 years ago
            lblFin2YearsAgo.Text = DateTime.Now.AddYears(-2).ToString("yyyy");
            lblFin4YearsAgo.Text = DateTime.Now.AddYears(-4).ToString("yyyy");

            _excelFilePath = _settings.Find(x => x.Name == "ExcelTradesPath").Value;

            panelMarkets.Visible = false;
            //txtTickerList.Text = "AB" + Environment.NewLine + "ACB" + Environment.NewLine + "AG" + Environment.NewLine;
        }

        private async void btnGetOne_click(object sender, EventArgs e)
        {
  
            txtStockTicker.Text = txtStockTicker.Text.ToUpper();

            if (string.IsNullOrEmpty(txtStockTicker.Text))
            {
                MessageBox.Show("Enter a valid stock ticker.");
                return;
            }

            DataTable tradesDataTable = TradesDataTable;
            // Trades
            tradesDataTable.Columns[0].DataType = System.Type.GetType("System.DateTime");

            /////////// Set Bullish / Bearish scale
            // Get DOW level from a month ago
            var tickerTradesList = tradesDataTable.AsEnumerable().Where(x => DateTime.ParseExact(x[0].ToString(), "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) > DateTime.Now.AddDays(-30) && x[1].ToString().Trim() != "").ToList();
            // get first and last row's DOW
            float oneMonthAgoDOW = Convert.ToInt32(tickerTradesList.First().ItemArray[1].ToString());
            float currentDOW = Convert.ToInt32(tickerTradesList.Last().ItemArray[1].ToString());
            float perc = (currentDOW - oneMonthAgoDOW) / oneMonthAgoDOW * 120;
            if (perc < -5) perc = -5;
            trackBar1.Value = 5 + Convert.ToInt32(perc);

            PreSummaryWebCall(); // Sets the form display while the request is executing

            Market_SandP =  await _marketData.GetMarketData("^GSPC");
            Market_Dow =    await _marketData.GetMarketData("^DJI");
            Market_Nasdaq = await _marketData.GetMarketData("^IXIC");

            // Extract the individual data values from the html
            _tickerFound = await _stockSummary.GetSummaryData(txtStockTicker.Text);

            if (_tickerFound)
            {
                TickerTradesDataTable = new DataTable();
                int DateColumn = 0;
                DateTime outDate = DateTime.Now;
                List<StockHistory.HistoricPriceData> historicDisplayList = new List<StockHistory.HistoricPriceData>();
                EnumerableRowCollection<DataRow> tickerTrades;

                try
                {
                    GetFinancials();

                    // filter on stock ticker then order by date descending
                    tickerTrades = tradesDataTable.AsEnumerable().Where(x => x[4].ToString().ToLower() == txtStockTicker.Text.ToLower());
                    tickerTrades = tickerTrades.OrderByDescending(x => x[DateColumn]);

                    // get 3 year ago price
                    historicDisplayList = await _stockHistory.GetPriceHistoryForTodayWeekMonthYear(txtStockTicker.Text, _stockSummary, true, false, false);
                }
                catch (Exception ex)
                {
                    ResetFormControls();

                    MessageBox.Show("Error somewhere in GetFinancials() or GetPriceHistoryForTodayWeekMonthYear()"+ Environment.NewLine 
                                                                                                                + ex.Message + Environment.NewLine 
                                                                                                                + ex.StackTrace + Environment.NewLine + ex.InnerException);
                    return;
                }

                // bind data list to grid control
                BindListToHistoricPriceGrid(historicDisplayList);

                if (tradesDataTable.Rows.Count > 0 && tickerTrades.Count() > 0)
                {
                    TickerTradesDataTable = tickerTrades.Where(r => r[3].ToString() != "0").CopyToDataTable();

                    // bind data list to trades grid control
                    BindingSource tradeSource = new BindingSource();
                    tradeSource.DataSource = TickerTradesDataTable;
                    dataGridView2.DefaultCellStyle.ForeColor = Color.LightSteelBlue;
                    dataGridView2.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
                    dataGridView2.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
                    dataGridView2.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
                    dataGridView2.DataSource = tradeSource.DataSource;
                    dataGridView2.Columns[0].HeaderText = "Date";
                    //dataGridView2.Columns[0].DefaultCellStyle.Format = "MM/dd/yyyy";
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
                    if (TickerTradesDataTable != null && dataGridView2.Rows.Count > 0)
                        try
                        {
                            dataGridView2.Rows.Clear();
                        }
                        catch
                        { } // Do nothing
                    else
                        dataGridView2.DataSource = null;

                    dataGridView2.Refresh();
                }

                // Trends
                _stockHistory.SetTrends();
                SetTrendImages();
            }

            PostSummaryWebCall(); // displays the data returned

        }

        private void ResetFormControls()
        {
            btnGetOne.Enabled = true;
            picSpinner.Visible = false;
            Cursor.Current = Cursors.Default;
            panel1.Visible = panel2.Visible = panel3.Visible = false;
        }

        private void SetTrendImages()
        {
            if (_stockHistory.ThreeYearTrend != StockHistory.TrendEnum.Unknown)
            {
                pic3YearTrend.Visible = true;
                pic3YearTrend.Image = _stockHistory.ThreeYearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.ThreeYearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }
            else
                pic3YearTrend.Visible = false;

            if (_stockHistory.YearTrend != StockHistory.TrendEnum.Unknown)
            {
                picYearTrend.Visible = true;
                picYearTrend.Image = _stockHistory.YearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.YearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }
            else
                picYearTrend.Visible = false;


            if (_stockHistory.MonthTrend != StockHistory.TrendEnum.Unknown)
            {
                picMonthTrend.Visible = true;
                picMonthTrend.Image = _stockHistory.MonthTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.MonthTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }
            else
                picMonthTrend.Visible = false;

            if (_stockHistory.WeekTrend != StockHistory.TrendEnum.Unknown)
            {
                picWeekTrend.Visible = true;
                picWeekTrend.Image = _stockHistory.WeekTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockHistory.WeekTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }
            else
                picWeekTrend.Visible = false;
        }

        private void BindListToHistoricPriceGrid(List<StockHistory.HistoricPriceData> historicDisplayList)
        {
            var bindingList = new BindingList<StockHistory.HistoricPriceData>(historicDisplayList);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DefaultCellStyle.ForeColor = Color.LightSteelBlue;
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
            List<StockHistory.HistoricPriceData> historicDataList = new List<StockHistory.HistoricPriceData>();

            stockList = txtTickerList.Text.Split(Environment.NewLine).ToList();
            stockList = stockList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList(); // remove blacks

            txtTickerList.Text = ".";
            //builder.Append($"Ticker, Volatility, EarningsPerShare, OneYearTargetPrice, PriceBook, ProfitMargin, Dividend, 3YearPrice, 3YearPriceChange{Environment.NewLine}");
            foreach (string ticker in stockList)
            {
                _stockSummary.Ticker = ticker.Substring(0, (ticker + ",").IndexOf(",")).ToUpper();

                txtTickerList.Text += ".";

                // Extract the individual data values from the html
                _tickerFound = await _stockSummary.GetSummaryData(_stockSummary.Ticker);
                await _stockFinancials.GetFinancialData(_stockSummary.Ticker);

                // get 3 year ago price
                historicDataList = await _stockHistory.GetPriceHistoryForTodayWeekMonthYear(ticker, _stockSummary, true, false, false);

                if (historicDataList.Count > 0)
                    _stockHistory.HistoricData3YearsAgo = historicDataList.Last();
                else
                    _stockHistory.HistoricData3YearsAgo = new StockHistory.HistoricPriceData() { Ticker = _stockSummary.Ticker, Price = _stockSummary.PriceString.NumericValue };

                decimal percent_diff = _stockSummary.PriceString.NumericValue / _stockHistory.HistoricData3YearsAgo.Price - 1M;

                Analyze.AnalyzeInputs analyzeInputs = new Analyze.AnalyzeInputs();
                SetUpAnalyzeInputs(analyzeInputs);
                decimal totalMetric = _analyze.AnalyzeStockData(_stockSummary, _stockHistory, _stockFinancials, analyzeInputs, true);

                builder.Append($"{_stockSummary.Ticker}, {_stockSummary.VolatilityString.NumericValue}, {_stockSummary.EarningsPerShareString.NumericValue}, {_stockSummary.OneYearTargetPriceString.NumericValue}, {_stockSummary.PriceBookString.NumericValue}, {_stockSummary.ProfitMarginString.NumericValue}, {_stockSummary.DividendString.NumericValue}, {_stockFinancials.ShortInterestString.NumericValue}");
                builder.Append($", {_stockHistory.HistoricData3YearsAgo.Price}, {percent_diff.ToString("0.00")},{_stockSummary.YearsRangeLow.NumericValue},{_stockSummary.YearsRangeHigh.NumericValue},{totalMetric}{Environment.NewLine}");
                Thread.Sleep(800);
            }
            txtTickerList.Text = builder.ToString();
        }

        private void PreSummaryWebCall()
        {
            _stockSummary = new StockSummary(); // set values to zero
            btnGetAllHistory.Visible = true;
            txtAnalysisOutput.Text = "";
            lblValuation.Text = "";
            txtTickerList.Text = txtStockTicker.Text;
            pic3YearTrend.Image = picSidewaysTrend.Image;
            picYearTrend.Image = picSidewaysTrend.Image;
            picMonthTrend.Image = picSidewaysTrend.Image;
            picWeekTrend.Image = picSidewaysTrend.Image;

            btnGetOne.Enabled = false;
            panel1.Visible = panel2.Visible = panel3.Visible = false;
            picYearTrend.Visible = false;
            picMonthTrend.Visible = false;
            picWeekTrend.Visible = false;
            picSpinner.Visible = true;
            Cursor.Current = Cursors.WaitCursor;
        }

        private void PostSummaryWebCall()
        {
            string errorPlace = "";
            ResetFormControls();
            try
            {
                lblCompanyNameAndTicker.Text = _stockSummary.CompanyName;
                if (_tickerFound)
                {
                    errorPlace = "Setting labels #1";
                    lblPrice.Text = _stockSummary.PriceString.NumericValue.ToString("####.00");
                    lblVolatility.Text = _stockSummary.VolatilityString.StringValue;
                    lblEPS.Text = _stockSummary.EarningsPerShareString.StringValue;
                    lblEPS.ForeColor = _stockSummary.EPSColor;
                    lblPriceBook.Text = _stockSummary.PriceBookString.NumericValue.ToString();
                    lblPriceBook.ForeColor = _stockSummary.PriceBookColor;
                    lblDividend.Text = _stockSummary.DividendString.NumericValue.ToString() + "%";
                    lblDividend.ForeColor = _stockSummary.DividendColor;
                    lblProfitMargin.Text = _stockSummary.ProfitMarginString.StringValue.ToString() + "%";
                    lblProfitMargin.ForeColor = _stockSummary.ProfitMarginColor;
                    lblOneYearTarget.Text = _stockSummary.OneYearTargetPriceString.StringValue;
                    lblOneYearTarget.ForeColor = _stockSummary.OneYearTargetColor;
                    lbl52WeekLow.Text = _stockSummary.YearsRangeLow.StringValue;
                    lbl52WeekHigh.Text = _stockSummary.YearsRangeHigh.StringValue;
                    lblForwardPE.Text = _stockSummary.ForwardPEString.NumericValue.ToString();
                    lblForwardPE.ForeColor = _stockSummary.ForwardPEColor;
                    lblEarningsDate.Text = _stockSummary.EarningsDateString.DateTimeValue.ToString();
                    lblEarningsDate.ForeColor = _stockSummary.EarningsDateColor;
                    lblSector.Text = _stockSummary.Sector;
                    lblAvgSectorPE.Text = _stockSummary.AverageSectorPE.ToString();
                    lblBuyQuantity.Text = "0";
                    lblBuyPrice.Text = "0.00";
                    lblSellQuantity.Text = "0";
                    lblSellPrice.Text = "0.00";
                    errorPlace = "Setting labels #2";
                    /////////  Market Data
                    lblSandP500.Text = Market_SandP.CurrentLevel.NumericValue.ToString("N0");
                    lblSandP500Change.Text = Market_SandP.Change.ToString();
                    lblSandP500PercChange.Text = Market_SandP.PercentageChange.ToString("0.0") + "%";
                    lblSandP500Change.ForeColor = Market_SandP.MarketColor;
                    lblSandP500PercChange.ForeColor = Market_SandP.MarketColor;

                    lblDOW30.Text = Market_Dow.CurrentLevel.NumericValue.ToString("N0");
                    lblDOW30Change.Text = Market_Dow.Change.ToString();
                    lblDOW30PercChange.Text = Market_Dow.PercentageChange.ToString("0.0") + "%";
                    lblDOW30Change.ForeColor = Market_Dow.MarketColor;
                    lblDOW30PercChange.ForeColor = Market_Dow.MarketColor;

                    lblNasdaq.Text = Market_Nasdaq.CurrentLevel.NumericValue.ToString("N0");
                    lblNasdaqChange.Text = Market_Nasdaq.Change.ToString();
                    lblNasdaqPercChange.Text = Market_Nasdaq.PercentageChange.ToString("0.0") + "%";
                    lblNasdaqChange.ForeColor = Market_Nasdaq.MarketColor;
                    lblNasdaqPercChange.ForeColor = Market_Nasdaq.MarketColor;

                    panelMarkets.Visible = true;

                    ///////////  52 week range
                    if (_stockSummary.YearsRangeHigh.NumericValue > 0)
                    {
                        errorPlace = "52 week range";
                        lbl52WeekArrow.Visible = true;
                        int w52 = lbl52WeekHighArrow.Left - lbl52WeekLowArrow.Left; // distance between controls 
                        decimal range52 = _stockSummary.YearsRangeHigh.NumericValue - _stockSummary.YearsRangeLow.NumericValue; // total price range
                        decimal perc52 = (_stockSummary.PriceString.NumericValue - _stockSummary.YearsRangeLow.NumericValue) / range52; // percent above low
                        lbl52WeekArrow.Left = lbl52WeekLowArrow.Left + (int)(perc52 * w52) - 4; // set left of arrow current price
                    }
                    else
                    {
                        lbl52WeekArrow.Visible = false;
                    }

                    //////////  Ticker Trades
                    if (TickerTradesDataTable != null && TickerTradesDataTable.Rows.Count > 0)
                    {
                        errorPlace = "Ticker trades";
                        DataRow latestRow = TickerTradesDataTable.Rows[0];
                        txtSharesOwned.Text = latestRow.ItemArray[6].ToString();      // Total Shares
                        if (txtSharesOwned.Text.Trim() == "")
                            txtSharesOwned.Text = "1";
                        txtSharesTraded.Text = latestRow.ItemArray[3].ToString();     // Quan. Traded
                        txtSharesTradePrice.Text = ((double)latestRow.ItemArray[5]).ToString("0.00"); // Price
                        if (latestRow.ItemArray[2].ToString().Trim().ToLower() == "buy")
                        {
                            radBuy.Checked = true;
                            radSell.Checked = false;
                        }
                        else
                        {
                            radBuy.Checked = false;
                            radSell.Checked = true;
                        }

                        string min;
                        string max;
                        float previous;
                        float current;
                        int i = 0;
                        // Color trades based on groups of 5 rows, the high and low for the 5 rows
                        foreach (DataRow r in TickerTradesDataTable.Rows)
                        {
                            // Color Buy and Sells
                            if (r.ItemArray[2].ToString().Trim().ToLower().Contains("buy"))
                                dataGridView2.Rows[i].Cells[2].Style.ForeColor = Color.Lime;
                            else
                                dataGridView2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;

                            int i2 = i;
                            if (i % 1 == 0) // every 2nd pass, evaluate
                            {
                                // Find Min, Max trade price
                                min = TickerTradesDataTable.Select().Skip(i).Take(5).AsEnumerable()
                                    .Min(row => row[5])
                                    .ToString();
                                max = TickerTradesDataTable.Select().Skip(i).Take(5).AsEnumerable()
                                    .Max(row => row[5])
                                    .ToString();

                                for (i2 = i; i2 < i + Math.Min(5, (TickerTradesDataTable.Rows.Count)) && i < TickerTradesDataTable.Rows.Count - Math.Min(4, (TickerTradesDataTable.Rows.Count - 1)); i2++)
                                {
                                    if (TickerTradesDataTable.Rows[i2].ItemArray[5].ToString() == max) // High - Green coloring
                                    {
                                        current = previous = 0;
                                        try
                                        {
                                            if (i2 > 0)
                                                previous = float.Parse(TickerTradesDataTable.Rows[i2 - 1].ItemArray[5].ToString());
                                            current = float.Parse(TickerTradesDataTable.Rows[i2].ItemArray[5].ToString());
                                        }
                                        catch { } // eat the error
                                        if (current > previous)
                                        {
                                            dataGridView2.Rows[i2].Cells[5].Style.ForeColor = Color.Lime;
                                            if (i2 > 1 && dataGridView2.Rows[i2 - 1].Cells[5].Style.ForeColor == Color.Lime)
                                                dataGridView2.Rows[i2 - 1].Cells[5].Style.ForeColor = Color.Silver;
                                        }
                                    }
                                    if (TickerTradesDataTable.Rows[i2].ItemArray[5].ToString() == min) // Low - Red coloring
                                    {
                                        // get previous, if previous < than this low, don't color and leave previous alone
                                        current = previous = 0;
                                        try
                                        {
                                            if (i2 > 0)
                                                previous = float.Parse(TickerTradesDataTable.Rows[i2 - 1].ItemArray[5].ToString());
                                            else
                                                previous = 10000;
                                            current = float.Parse(TickerTradesDataTable.Rows[i2].ItemArray[5].ToString());
                                        }
                                        catch { } // eat the error
                                        if (current < previous)
                                        {
                                            dataGridView2.Rows[i2].Cells[5].Style.ForeColor = Color.Red;
                                            if (i2 > 1 && dataGridView2.Rows[i2 - 1].Cells[5].Style.ForeColor == Color.Red)
                                                dataGridView2.Rows[i2 - 1].Cells[5].Style.ForeColor = Color.Silver;
                                        }
                                    }
                                }
                            }

                            i++;
                        }
                    }
                    else
                    {
                        txtSharesOwned.Text = "1";
                        txtSharesTraded.Text = "1";
                        txtSharesTradePrice.Text = _stockSummary.PriceString.NumericValue.ToString("0.00");
                    }
                    panel1.Visible = panel2.Visible = panel3.Visible = true;
                } // Ticker found.
                else // Ticker not found.
                {
                    ResetFormControls();
                    MessageBox.Show("Stock ticker was not found.");
                }
            }
            catch (Exception e)
            {
                ResetFormControls();
                MessageBox.Show($"PostSummaryWebCall()\n {errorPlace}\n Error: {e.Message} {e.InnerException} {e.StackTrace}");
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            Analyze.AnalyzeInputs analyzeInputs = new Analyze.AnalyzeInputs();
            SetUpAnalyzeInputs(analyzeInputs);
            _analyze.AnalyzeStockData(_stockSummary, _stockHistory, _stockFinancials, analyzeInputs, false);

            txtAnalysisOutput.Text = _analyze.AnalysisMetricsOutputText;

            lblBuyQuantity.Text = _analyze.BuyQuantity.ToString();
            lblBuyPrice.Text = _analyze.BuyPrice.ToString();
            lblSellQuantity.Text = _analyze.SellQuantity.ToString();
            lblSellPrice.Text = _analyze.SellPrice.ToString();
        }

        private void SetUpAnalyzeInputs(Analyze.AnalyzeInputs analyzeInputs)
        {
            analyzeInputs.SharesOwned = Convert.ToInt32(txtSharesOwned.Text); 
            analyzeInputs.LastTradeBuySell = radBuy.Checked ? Analyze.BuyOrSell.Buy : Analyze.BuyOrSell.Sell;
            analyzeInputs.QuantityTraded = Convert.ToInt32(txtSharesTraded.Text);
            analyzeInputs.SharesTradedPrice = Convert.ToDecimal(txtSharesTradePrice.Text);
            analyzeInputs.MovementTargetPercent = Convert.ToInt32(txtMovementTargetPercent.Text);
            analyzeInputs.MarketHealth = 5;
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

        private async void GetFinancials()
        {
            lblShortInterest.Text = "...";

            panelFinancials.Visible = false;
            bool found = await _stockFinancials.GetFinancialData(txtStockTicker.Text);

            if (_stockFinancials._revenueInMillions == true)
                lblRevenueInMillions.Text = "(all number in millions)";
            else
                lblRevenueInMillions.Text = "(all number in thousands)";

            // Revenue
            lblFinRevTTM.Text = _stockFinancials.RevenueTtmString.StringValue;
            lblFinRevTTM.ForeColor = _stockFinancials.RevenueTtmColor;
            lblFinRev2YearsAgo.Text = _stockFinancials.Revenue2String.StringValue;
            lblFinRev2YearsAgo.ForeColor = _stockFinancials.Revenue2Color;
            lblFinRev4YearsAgo.Text = _stockFinancials.Revenue4String.StringValue;

            // Cost of Revenue
            lblFinCostRevTTM.Text = _stockFinancials.CostOfRevenueTtmString.StringValue;
            lblFinCostRev2YearsAgo.Text = _stockFinancials.CostOfRevenue2String.StringValue;
            lblFinCostRev4YearsAgo.Text = _stockFinancials.CostOfRevenue4String.StringValue;

            // Operating Expense
            lblOperExpTTM.Text = _stockFinancials.OperatingExpenseTtmString.StringValue;
            lblOperExp2YearsAgo.Text = _stockFinancials.OperatingExpense2String.StringValue;
            lblOperExp4YearsAgo.Text = _stockFinancials.OperatingExpense4String.StringValue;

            // Operating Profit / Loss
            lblOperProfitTTM.Text = $"{_stockFinancials.ProfitTTM:n0}";
            lblOperProfitTTM.ForeColor = _stockFinancials.ProfitTtmColor;
            lblOperProfit2YearsAgo.Text = $"{_stockFinancials.Profit2YearsAgo:n0}";
            lblOperProfit2YearsAgo.ForeColor = _stockFinancials.Profit2YearsAgoColor;
            lblOperProfit4YearsAgo.Text = $"{_stockFinancials.Profit4YearsAgo:n0}";
            lblOperProfit4YearsAgo.ForeColor = _stockFinancials.Profit4YearsAgoColor;

            // Total Cash
            lblFinTotalCash.Text = _stockFinancials.TotalCashString;
            // Total Debt
            lblFinTotalDebt.Text = _stockFinancials.TotalDebtString;
            lblFinTotalDebt.ForeColor = _stockFinancials.TotalDebtColor;
            // Debt Equity Ratio
            lblFinDebtEquity.Text = _stockFinancials.DebtEquityString.StringValue;
            lblFinDebtEquity.ForeColor = _stockFinancials.DebtEquityColor;

            // Short Interest
            lblShortInterest.Text = _stockFinancials.ShortInterestString.StringValue + "%";
            lblShortInterest.ForeColor = _stockFinancials.ShortInterestColor;
            panelFinancials.Visible = true;

            // Forward PE coloring is complex. It takes into account
            // 1. Average PE for the sector
            // 2. How large the profits are. We can use current profit margin. >15% is a high profit margin. -15% is a bad profit margin.
            // 3. How fast profits are growing/decreasing. (Current profit / Prior Profit)
            decimal profitTTM = _stockFinancials.ProfitTTM + (_stockFinancials.ProfitTTM + _stockFinancials.Profit4YearsAgo) / 3;
            decimal profit4Year = _stockFinancials.Profit4YearsAgo + (_stockFinancials.ProfitTTM + _stockFinancials.Profit4YearsAgo) / 3;

            if (profit4Year < 0)
            {
                profit4Year = 1;
            }

            decimal profitGrowth = 1;
            if (profit4Year + profitTTM == 0)
            {
                profitGrowth = 1;
            }
            else
            {
                profitGrowth = profitTTM / ((profitTTM + profit4Year) / 3); // Profit growth .5 - 2.0
            }

            if (profitGrowth > 2) // set max
                profitGrowth = 2;

            // Combine profit growth and margin into a number
            decimal marginFactor = 1 + (_stockSummary.ProfitMarginString.NumericValue / 100M);
            _stockSummary.CalculatedPEString.StringValue = (_stockSummary.ForwardPEString.NumericValue / (marginFactor * profitGrowth)).ToString("0.00");
            _stockSummary.Valuation = StockSummary.ValuationEnum.FairValue;
            lblCalculatedPE.Text = _stockSummary.CalculatedPEString.StringValue;

            if (_stockSummary.CalculatedPEString.NumericValue > 0 && _stockSummary.CalculatedPEString.NumericValue > (decimal)_stockSummary.AverageSectorPE * 1.3M) // Over valued
            {
                lblValuation.ForeColor = Color.Red;
                lblValuation.Text = "Overvalued";
                _stockSummary.Valuation = StockSummary.ValuationEnum.OverValued;
            }
            if (_stockSummary.CalculatedPEString.NumericValue > 0 && _stockSummary.CalculatedPEString.NumericValue < (decimal)_stockSummary.AverageSectorPE * .8M) // Under valued
            {
                lblValuation.ForeColor = Color.Lime;
                lblValuation.Text = "Undervalued";
                _stockSummary.Valuation = StockSummary.ValuationEnum.UnderValued;
            }
        }

        private async void btnGetAllHistory_Click(object sender, EventArgs e)
        {
            UseWaitCursor = true;
            Application.DoEvents();
            btnGetAllHistory.Visible = false;
            List<StockHistory.HistoricPriceData> historicDisplayList = await _stockHistory.GetPriceHistoryForTodayWeekMonthYear(txtStockTicker.Text, _stockSummary, true, true, true);
            BindListToHistoricPriceGrid(historicDisplayList);
            // Trends
            _stockHistory.SetTrends();
            SetTrendImages();
            UseWaitCursor = false;
        }

        private void lnkCompanyOverview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(_stockSummary.CompanyOverview, "Company Overview", MessageBoxButtons.OK);
        }


        //////////////////////////////
        //      Menu Items
        //////////////////////////////

        private void goldAndSilverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHyperlink("https://www.apmex.com/gold-price");
        }

        private void earningsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenHyperlink("https://earningshub.com/earnings-calendar/this-week");
        }

        private void yahooMarketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHyperlink("https://finance.yahoo.com/markets/");
        }

        private void yahooStockQuoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHyperlink($"https://finance.yahoo.com/quote/{txtStockTicker.Text}/");
        }

        private void finvizToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHyperlink($"https://finviz.com/");
        }
        private void apeWisdomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHyperlink($"https://apewisdom.io/");
        }
        //////////////////// Individual Stock Links  
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenHyperlink($"https://finance.yahoo.com/quote/{txtStockTicker.Text}/");
        }
        private void insiderTradingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHyperlink($"https://www.marketbeat.com/stocks/NASDAQ/{txtStockTicker.Text}/insider-trades/");
        }
        private void marketBeatAnaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHyperlink($"https://www.marketbeat.com/stocks/NASDAQ/{txtStockTicker.Text}/");
        }

        private static void OpenHyperlink(string url)
        {
            try
            {
                Process.Start("cmd", "/c start " + url);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnChart_Click(object sender, EventArgs e)
        {
            ChartForm chartForm = new ChartForm();
            chartForm.Owner = this;
            chartForm.Text = txtStockTicker.Text + " - 1 Year Chart";
            chartForm.Url = chartForm.Url1Year.Replace("?ticker?", txtStockTicker.Text);
            chartForm.Show();
        }

        private void btn3YearChart_Click(object sender, EventArgs e)
        {
            ChartForm chartForm = new ChartForm();
            chartForm.Owner = this;
            chartForm.Text = txtStockTicker.Text + " - 3 Year Chart";
            chartForm.Url = chartForm.Url3Year.Replace("?ticker?", txtStockTicker.Text);
            chartForm.Show();
        }

        private void btn10DayChart_Click(object sender, EventArgs e)
        {
            ChartForm chartForm = new ChartForm();
            chartForm.Owner = this;
            chartForm.Text = txtStockTicker.Text + " - 10 Day Chart";
            chartForm.Url = chartForm.Url10Day.Replace("?ticker?", txtStockTicker.Text);
            chartForm.Show();
        }

        private async void last20BuysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Performance performance = new Performance(_stockSummary);
            if(Market_Dow == null)
            {
                Market_Dow = await _marketData.GetMarketData("^DJI");
            }
            performance.GetLatestBuyPerformance(Market_Dow, PositionsDataTable, TradesDataTable);
            performance.ShowPerformanceForm(this);  
        }
    }
}
