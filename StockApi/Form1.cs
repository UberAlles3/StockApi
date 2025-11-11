using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using SqlLayer;
using SqlLayer.SQL_Models;
using YahooLayer;

namespace StockApi
{
    public partial class Form1 : Form
    {
        public static Color TextForeColor = Color.LightSteelBlue;
        
        List<Setting> _settings = new List<Setting>();
        private static bool _tickerFound = false;
        private static StockDownloads _stockDownloads = new StockDownloads("");
        private static Analyze _analyze = new Analyze();
        public static DataTable TickerTradesDataTable = null;

        // Markets
        Markets _markets = new Markets();
        
        // Excel files
        private static string _excelFilePath = "";
        private static DateTime _tradesImportDateTime = DateTime.Now.AddYears(-2);
        private static DateTime _positionsImportDateTime = DateTime.Now.AddYears(-2);
        private static DataTable _positionsDataTable = null;
        private static DataTable _tradesDataTable = null;

        // News
        private static string _news = "";
        private static DateTime newsDate = DateTime.Now.AddDays(-1);

        public static DataTable PositionsDataTable 
        {
            get 
            {
                DateTime excelFileDateTime = System.IO.File.GetLastWriteTime(_excelFilePath);
                if (excelFileDateTime > _positionsImportDateTime)
                {
                    _positionsDataTable = (new ExcelManager()).ImportExceelSheet(_excelFilePath, 0, 0, 36);
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
                    _tradesDataTable = (new ExcelManager()).ImportExceelSheet(_excelFilePath, 1, 40);
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
            SetupDailyTimer();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            ////// Testing section
            //FpmAPI fpmAPI = new FpmAPI();
            //fpmAPI.Test();
            //Analyze.CrunchThreeResult crt = Analyze.CrunchThree(10, 9, 7D);
            //                          crt = Analyze.CrunchThree(10D, 12D, 20D);
            // For dark menu.
            //Program.logger.Error("testing");
            this.menuStrip1.RenderMode = ToolStripRenderMode.Professional;
            this.menuStrip1.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());
 
            panel1.BackColor = Color.FromArgb(100, 0, 0, 0);
            panel1.Visible = false;
            panel2.BackColor = panel1.BackColor;
            panel2.Visible = false;
            panel3.BackColor = panel1.BackColor;
            panel3.Visible = false;

            // Wait gif
            picSpinner.Visible = false;
            picSpinner.Left = (this.Width / 2) + 56;
            picSpinner.Top = this.Height / 2 - 56;

            picUpTrend.Visible = false;
            picSidewaysTrend.Visible = false;
            picDownTrend.Visible = false;

            lblCompanyNameAndTicker.Text = txtSharesTraded.Text = "";
            txtMessages.Text = "";

            // temporary for testing
            txtStockTicker.Text = "AAPL";

            // Revenue year labels TTM, 2 years ago, 4 years ago
            lblFin2YearsAgo.Text = DateTime.Now.AddYears(-2).ToString("yyyy");
            lblFin4YearsAgo.Text = DateTime.Now.AddYears(-4).ToString("yyyy");

            _excelFilePath = _settings.Find(x => x.Name == "ExcelTradesPath").Value;

            GetNewsEarnings();
            _markets = new Markets();
            await _markets.GetAllMarketData();
            DisplayMarketData();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Dark menu.
            using (Pen pen = new Pen(Color.DarkGray, 1))
            {
                e.Graphics.DrawLine(pen, 0, menuStrip1.Bottom + 1, this.ClientSize.Width, menuStrip1.Bottom + 1);
            }
        }

        private async void btnGetOne_click(object sender, EventArgs e)
        {
            bool networkUp = NetworkInterface.GetIsNetworkAvailable();
            if(networkUp == false)
            {
                MessageBox.Show("Your network connection is unavailable.");
                return;
            }

            txtStockTicker.Text = txtStockTicker.Text.ToUpper();

            if (string.IsNullOrEmpty(txtStockTicker.Text))
            {
                MessageBox.Show("Enter a valid stock ticker.");
                return;
            }

            DataTable tradesDataTable = TradesDataTable;
            // Trades
            tradesDataTable.Columns[0].DataType = System.Type.GetType("System.DateTime");

            PreSummaryWebCall(); // Sets the form display while the request is executing

            try
            {
                _markets.GetAllMarketData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Market data failed." + Environment.NewLine + ex.Message);
            }

            ////////////////////////////////////////////////////////
            ///  Get all the stock's data from yahoo finance website
            ////////////////////////////////////////////////////////
            _stockDownloads = new StockDownloads(txtStockTicker.Text);
            try
            {
                _tickerFound = await _stockDownloads.GetAllStockData();
            }
            catch (Exception ex)
            {
                ResetFormControls();
                MessageBox.Show($"Error in getting all data from internet.\n{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}\n");
                return;
            }

            if (_stockDownloads.stockSummary.LastException != null)
            {
                txtMessages.Text = "Error:" + Environment.NewLine + _stockDownloads.stockSummary.Error;
            }

            if (_tickerFound)
            {
                TickerTradesDataTable = new DataTable();
                int DateColumn = 0;
                DateTime outDate = DateTime.Now;
                EnumerableRowCollection<DataRow> tickerTrades;

                // filter on stock ticker then order by date descending
                tickerTrades = tradesDataTable.AsEnumerable().Where(x => x[4].ToString().ToLower() == txtStockTicker.Text.ToLower());
                tickerTrades = tickerTrades.OrderByDescending(x => x[DateColumn]);

                // bind data list to grid control
                BindListToHistoricPriceGrid(_stockDownloads.stockHistory.HistoricDisplayList);

                if (tradesDataTable.Rows.Count > 0 && tickerTrades.Count() > 0)
                {
                    TickerTradesDataTable = tickerTrades.Where(r => r[3].ToString() != "0").CopyToDataTable();
                    ApplyStockSplits(TickerTradesDataTable);

                    // bind data list to trades grid control
                    BindingSource tradeSource = new BindingSource();
                    tradeSource.DataSource = TickerTradesDataTable;
                    dataGridView2.DefaultCellStyle.ForeColor = Form1.TextForeColor;
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
                    {
                        TickerTradesDataTable.Rows.Clear();
                        dataGridView2.DataSource = null;
                    }

                    dataGridView2.Refresh();
                }
            }

            PostSummaryWebCall(); // displays the data returned

        }

        private void btnGetHtml_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "StockSummaryHtml.txt");

            try
            {
                File.WriteAllText(filePath, _stockDownloads.stockSummary._html);
            }
            catch (Exception ex)
            {
                Program.logger.Error($"{ex.Message}  {ex.StackTrace}", ex);
                MessageBox.Show($"Error writing to file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit if file writing fails
            }

            try
            {
                Process.Start("notepad.exe", filePath);
            }
            catch (Exception ex)
            {
                Program.logger.Error($"{ex.Message}  {ex.StackTrace}", ex);
                MessageBox.Show($"Error opening Notepad: {ex.Message}", "Process Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyStockSplits(DataTable TickerTradesDataTable)
        {
            DataRow splitDR = TickerTradesDataTable.AsEnumerable().Where(r => r[(int)ExcelManager.TradeColumns.Splits].ToString().Contains("Split")).FirstOrDefault();
            if (splitDR != null)
            {
                bool splitting = false;
                int ratio = 1;
                foreach (DataRow currentRow in TickerTradesDataTable.Rows)
                {
                    string sss = currentRow.ItemArray[(int)ExcelManager.TradeColumns.Splits].ToString();
                    if (sss.Contains("Split"))
                    {
                        splitting = true;
                        ratio = Convert.ToInt32(sss._Between("Split", "-"));
                    }

                    if (splitting)
                    {
                        int quantity = Convert.ToInt32(currentRow.ItemArray[(int)ExcelManager.TradeColumns.QuantityTraded]) / ratio;
                        decimal price = Convert.ToDecimal(currentRow.ItemArray[(int)ExcelManager.TradeColumns.TradePrice]) * ratio;
                        
                        int totShares = 0;
                        if (currentRow.ItemArray[(int)ExcelManager.TradeColumns.QuantityHeld] != System.DBNull.Value)
                           totShares = Convert.ToInt32(currentRow.ItemArray[(int)ExcelManager.TradeColumns.QuantityHeld]) / ratio;
                        
                        currentRow[(int)ExcelManager.TradeColumns.QuantityTraded] = quantity;
                        currentRow[(int)ExcelManager.TradeColumns.TradePrice] = price;
                        currentRow[(int)ExcelManager.TradeColumns.QuantityHeld] = totShares;
                    }
                }
            }
        }

        private void ResetFormControls()
        {
            btnGetOne.Enabled = true;
            btnAnalyze.Enabled = true;
            picSpinner.Visible = false;
            Cursor.Current = Cursors.Default;
            panel1.Visible = panel2.Visible = panel3.Visible = pnlNoFinancials.Visible = false;
        }

        private void SetHistoricPriceTrendImages()
        {
            if (_stockDownloads.stockHistory.ThreeYearTrend != StockHistory.TrendEnum.Unknown)
            {
                pic3YearTrend.Visible = true;
                pic3YearTrend.Image = _stockDownloads.stockHistory.ThreeYearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockDownloads.stockHistory.ThreeYearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }
            else
                pic3YearTrend.Visible = false;

            if (_stockDownloads.stockHistory.YearTrend != StockHistory.TrendEnum.Unknown)
            {
                picYearTrend.Visible = true;
                picYearTrend.Image = _stockDownloads.stockHistory.YearTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockDownloads.stockHistory.YearTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }
            else
                picYearTrend.Visible = false;


            if (_stockDownloads.stockHistory.MonthTrend != StockHistory.TrendEnum.Unknown)
            {
                picMonthTrend.Visible = true;
                picMonthTrend.Image = _stockDownloads.stockHistory.MonthTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockDownloads.stockHistory.MonthTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }
            else
                picMonthTrend.Visible = false;

            if (_stockDownloads.stockHistory.WeekTrend != StockHistory.TrendEnum.Unknown)
            {
                picWeekTrend.Visible = true;
                picWeekTrend.Image = _stockDownloads.stockHistory.WeekTrend == StockHistory.TrendEnum.Up ? picUpTrend.Image : _stockDownloads.stockHistory.WeekTrend == StockHistory.TrendEnum.Down ? picDownTrend.Image : picSidewaysTrend.Image;
            }
            else
                picWeekTrend.Visible = false;
        }

        private void BindListToHistoricPriceGrid(List<StockHistory.HistoricPriceData> historicDisplayList)
        {
            var bindingList = new BindingList<StockHistory.HistoricPriceData>(historicDisplayList);
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


        private void PreSummaryWebCall()
        {
            _stockDownloads.stockSummary = new StockSummary(); // set values to zero
            btnGetAllHistory.Visible = true;
            txtAnalysisOutput.Text = "";
            lblValuation.Text = "";
            pic3YearTrend.Image = picSidewaysTrend.Image;
            picYearTrend.Image = picSidewaysTrend.Image;
            picMonthTrend.Image = picSidewaysTrend.Image;
            picWeekTrend.Image = picSidewaysTrend.Image;

            btnGetOne.Enabled = false;
            btnAnalyze.Enabled = false;
            panel1.Visible = panel2.Visible = panel3.Visible = false;
            picYearTrend.Visible = false;
            picMonthTrend.Visible = false;
            picWeekTrend.Visible = false;
            picSpinner.Visible = true;
            panelFinancials.Visible = false;
            pnlNoFinancials.Visible = false;

            lblShortInterest.Text = "...";

            Cursor.Current = Cursors.WaitCursor;
        }

        private void PostSummaryWebCall()
        {
            string errorPlace = "";
            ResetFormControls();
            try
            {
                lblCompanyNameAndTicker.Text = _stockDownloads.stockSummary.CompanyName;
                if (_tickerFound)
                {
                    errorPlace = "Setting labels #1";
                    lblPrice.Text = _stockDownloads.stockSummary.PriceString.NumericValue.ToString("####.00");
                    lblVolatility.Text = _stockDownloads.stockSummary.VolatilityString.StringValue;
                    lblEPS.Text = _stockDownloads.stockSummary.EarningsPerShareString.NumericValue.ToString("####.00");
                    lblEPS.ForeColor = _stockDownloads.stockSummary.EPSColor;
                    lblPriceBook.Text = _stockDownloads.stockSummary.PriceBookString.NumericValue.ToString();
                    lblPriceBook.ForeColor = _stockDownloads.stockSummary.PriceBookColor;
                    lblDividend.Text = _stockDownloads.stockSummary.DividendString.NumericValue.ToString() + "%";
                    lblDividend.ForeColor = _stockDownloads.stockSummary.DividendColor;
                    lblProfitMargin.Text = _stockDownloads.stockSummary.ProfitMarginString.StringValue.ToString() + "%";
                    lblProfitMargin.ForeColor = _stockDownloads.stockSummary.ProfitMarginColor;
                    lblOneYearTarget.Text = _stockDownloads.stockSummary.OneYearTargetPriceString.StringValue;
                    lblOneYearTarget.ForeColor = _stockDownloads.stockSummary.OneYearTargetColor;
                    lbl52WeekLow.Text = _stockDownloads.stockSummary.YearsRangeLow.StringValue;
                    lbl52WeekHigh.Text = _stockDownloads.stockSummary.YearsRangeHigh.StringValue;
                    lblForwardPE.Text = _stockDownloads.stockSummary.ForwardPEString.NumericValue.ToString();
                    lblForwardPE.ForeColor = _stockDownloads.stockSummary.ForwardPEColor;
                    lblEarningsDate.Text = _stockDownloads.stockSummary.EarningsDateString.DateTimeValue.ToString();
                    lblEarningsDate.ForeColor = _stockDownloads.stockSummary.EarningsDateColor;
                    lblSector.Text = _stockDownloads.stockSummary.Sector;
                    lblAvgSectorPE.Text = _stockDownloads.stockSummary.AverageSectorPE.ToString();
                    lblBuyQuantity.Text = "0";
                    lblBuyPrice.Text = "0.00";
                    lblSellQuantity.Text = "0";
                    lblSellPrice.Text = "0.00";
                    errorPlace = "Setting labels #2";
                    /////////  Market Data
                    DisplayMarketData();

                    // Calculated PE can only be figured after both summary and finacial data is combined
                    _stockDownloads.stockSummary.SetCalculatedPE(_stockDownloads);
                    lblCalculatedPE.Text = _stockDownloads.stockSummary.CalculatedPEString.StringValue;

                    if (_stockDownloads.stockSummary.Valuation == StockSummary.ValuationEnum.OverValued)
                    {
                        lblValuation.ForeColor = Color.Red;
                        lblValuation.Text = "Overvalued";
                    }
                    //if (_stockDownloads.stockSummary.CalculatedPEString.NumericValue > 0 && _stockDownloads.stockSummary.CalculatedPEString.NumericValue < (decimal)_stockDownloads.stockSummary.AverageSectorPE * .8M) // Under valued
                    if (_stockDownloads.stockSummary.Valuation == StockSummary.ValuationEnum.UnderValued)
                    {
                        lblValuation.ForeColor = Color.Lime;
                        lblValuation.Text = "Undervalued";
                    }

                    panelMarkets.Visible = true;
                    panelFinancials.Visible = true;

                    ///////////  52 week range
                    if (_stockDownloads.stockSummary.YearsRangeHigh.NumericValue > 0)
                    {
                        errorPlace = "52 week range";
                        lbl52WeekArrow.Visible = true;
                        int w52 = lbl52WeekHighArrow.Left - lbl52WeekLowArrow.Left; // distance between controls 
                        decimal range52 = _stockDownloads.stockSummary.YearsRangeHigh.NumericValue - _stockDownloads.stockSummary.YearsRangeLow.NumericValue; // total price range
                        decimal perc52 = (_stockDownloads.stockSummary.PriceString.NumericValue - _stockDownloads.stockSummary.YearsRangeLow.NumericValue) / range52; // percent above low
                        lbl52WeekArrow.Left = lbl52WeekLowArrow.Left + (int)(perc52 * w52) - 4; // set left of arrow current price
                    }
                    else
                    {
                        lbl52WeekArrow.Visible = false;
                    }

                    // Historic Price Trends
                    SetHistoricPriceTrendImages();

                    //////////  Ticker Trades
                    if (TickerTradesDataTable != null && TickerTradesDataTable.Rows.Count > 0)
                    {
                        errorPlace = "Ticker trades";
                        DataRow latestRow = TickerTradesDataTable.Rows[0];
                        txtSharesOwned.Text = latestRow.ItemArray[6].ToString();      // Total Shares
                        if (txtSharesOwned.Text.Trim() == "")
                            txtSharesOwned.Text = "1";
                        if (txtSharesTraded.Text.Trim() == "")
                            txtSharesTraded.Text = "1";
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
                        txtSharesTradePrice.Text = _stockDownloads.stockSummary.PriceString.NumericValue.ToString("0.00");
                    }

                    if (_stockDownloads.stockSummary.sqlTicker.IsFund == false && _stockDownloads.stockSummary.sqlTicker.IsFund == false)
                    {
                        //////////////////////////////////////////////////////////////////////////////
                        ///                            Income Statement
                        // fix EPS for stock that miss it in summary  
                        if (_stockDownloads.stockSummary.EarningsPerShareString.StringValue == "--")
                            _stockDownloads.stockSummary.EarningsPerShareString.StringValue = _stockDownloads.stockIncomeStatement.BasicEpsTtmString.StringValue;

                        // Revenue
                        lblFinRevTTM.Text = $"{_stockDownloads.stockIncomeStatement.RevenueTtmString:n0}";
                        lblFinRevTTM.ForeColor = _stockDownloads.stockIncomeStatement.RevenueTtmColor;
                        lblFinRev2YearsAgo.Text = _stockDownloads.stockIncomeStatement.Revenue2String.StringValue;
                        lblFinRev2YearsAgo.ForeColor = _stockDownloads.stockIncomeStatement.Revenue2Color;
                        lblFinRev4YearsAgo.Text = _stockDownloads.stockIncomeStatement.Revenue4String.StringValue;

                        // Cost of Revenue
                        lblFinCostRevTTM.Text = _stockDownloads.stockIncomeStatement.CostOfRevenueTtmString.StringValue;
                        lblFinCostRev2YearsAgo.Text = _stockDownloads.stockIncomeStatement.CostOfRevenue2String.StringValue;
                        lblFinCostRev4YearsAgo.Text = _stockDownloads.stockIncomeStatement.CostOfRevenue4String.StringValue;

                        // Operating Expense
                        lblOperExpTTM.Text = _stockDownloads.stockIncomeStatement.OperatingExpenseTtmString.StringValue;
                        lblOperExp2YearsAgo.Text = _stockDownloads.stockIncomeStatement.OperatingExpense2String.StringValue;
                        lblOperExp4YearsAgo.Text = _stockDownloads.stockIncomeStatement.OperatingExpense4String.StringValue;

                        // Operating Profit / Loss
                        lblOperProfitTTM.Text = _stockDownloads.stockIncomeStatement.ProfitTtmString.NumericValue.ToString("N0");
                        lblOperProfitTTM.ForeColor = _stockDownloads.stockIncomeStatement.ProfitTtmColor;
                        lblOperProfit2YearsAgo.Text = _stockDownloads.stockIncomeStatement.Profit2String.NumericValue.ToString("N0");
                        lblOperProfit2YearsAgo.ForeColor = _stockDownloads.stockIncomeStatement.Profit2YearsAgoColor;
                        lblOperProfit4YearsAgo.Text = _stockDownloads.stockIncomeStatement.Profit4String.NumericValue.ToString("N0");
                        lblOperProfit4YearsAgo.ForeColor = _stockDownloads.stockIncomeStatement.Profit4YearsAgoColor;

                        // Operating Cash Flow
                        lblOperCashFlowTTM.Text = _stockDownloads.stockCashFlow.OperatingCashFlowTtmString.NumericValue.ToString("N0");
                        lblOperCashFlowTTM.ForeColor = _stockDownloads.stockCashFlow.OperatingCashFlowTtmColor;
                        lblOperCashFlow2YearsAgo.Text = _stockDownloads.stockCashFlow.OperatingCashFlow2String.NumericValue.ToString("N0");
                        lblOperCashFlow2YearsAgo.ForeColor = _stockDownloads.stockCashFlow.OperatingCashFlow2Color;
                        lblOperCashFlow4YearsAgo.Text = _stockDownloads.stockCashFlow.OperatingCashFlow4String.NumericValue.ToString("N0");
                        lblOperCashFlow4YearsAgo.ForeColor = _stockDownloads.stockCashFlow.OperatingCashFlow4Color;

                        // Free Cash Flow
                        lblFreeCashFlowTTM.Text = _stockDownloads.stockCashFlow.FreeCashFlowTtmString.NumericValue.ToString("N0");
                        lblFreeCashFlowTTM.ForeColor = _stockDownloads.stockCashFlow.FreeCashFlowTtmColor;
                        lblFreeCashFlow2YearsAgo.Text = _stockDownloads.stockCashFlow.FreeCashFlow2String.NumericValue.ToString("N0");
                        lblFreeCashFlow2YearsAgo.ForeColor = _stockDownloads.stockCashFlow.FreeCashFlow2Color;
                        lblFreeCashFlow4YearsAgo.Text = _stockDownloads.stockCashFlow.FreeCashFlow4String.NumericValue.ToString("N0");
                        lblFreeCashFlow4YearsAgo.ForeColor = _stockDownloads.stockCashFlow.FreeCashFlow4Color;

                        // End Cash Postion
                        lblEndCashPositionTTM.Text = _stockDownloads.stockCashFlow.EndCashPositionTtmString.NumericValue.ToString("N0");
                        lblEndCashPositionTTM.ForeColor = _stockDownloads.stockCashFlow.EndCashPositionTtmColor;
                        lblEndCashPosition2YearsAgo.Text = _stockDownloads.stockCashFlow.EndCashPosition2String.NumericValue.ToString("N0");
                        lblEndCashPosition2YearsAgo.ForeColor = _stockDownloads.stockCashFlow.EndCashPosition2Color;
                        lblEndCashPosition4YearsAgo.Text = _stockDownloads.stockCashFlow.EndCashPosition4String.NumericValue.ToString("N0");
                        lblEndCashPosition4YearsAgo.ForeColor = _stockDownloads.stockCashFlow.EndCashPosition4Color;

                        // Basic EPS
                        lblBasicEpsTTM.Text = _stockDownloads.stockIncomeStatement.BasicEpsTtmString.NumericValue.ToString("0.00");
                        lblBasicEpsTTM.ForeColor = _stockDownloads.stockIncomeStatement.BasicEpsTtmColor;
                        lblBasicEps2YearsAgo.Text = _stockDownloads.stockIncomeStatement.BasicEps2String.NumericValue.ToString("0.00");
                        lblBasicEps2YearsAgo.ForeColor = _stockDownloads.stockIncomeStatement.BasicEps2Color;
                        lblBasicEps4YearsAgo.Text = _stockDownloads.stockIncomeStatement.BasicEps4String.NumericValue.ToString("0.00");
                        lblBasicEps4YearsAgo.ForeColor = _stockDownloads.stockIncomeStatement.BasicEps4Color;

                        //////////////////////////////////////////////////////////////////////////////////////
                        ///                           Statistics values
                        // Total Cash
                        lblFinTotalCash.Text = _stockDownloads.stockStatistics.TotalCashString;
                        // Total Debt
                        lblFinTotalDebt.Text = _stockDownloads.stockStatistics.TotalDebtString;
                        lblFinTotalDebt.ForeColor = _stockDownloads.stockStatistics.TotalDebtColor;
                        // Debt Equity Ratio
                        lblFinDebtEquity.Text = _stockDownloads.stockStatistics.DebtEquityString.StringValue;
                        lblFinDebtEquity.ForeColor = _stockDownloads.stockStatistics.DebtEquityColor;

                        // Short Interest
                        lblShortInterest.Text = _stockDownloads.stockStatistics.ShortInterestString.StringValue + "%";
                        lblShortInterest.ForeColor = _stockDownloads.stockStatistics.ShortInterestColor;
                        pnlNoFinancials.Visible = false;
                    }
                    else
                    {
                        pnlNoFinancials.Visible = true;
                    }
                    panel1.Visible = panel2.Visible = panel3.Visible = true;

                } // Ticker found.
                else // Ticker not found.
                {
                    ResetFormControls();
                    MessageBox.Show("Stock ticker was not found.");
                }
            }
            catch (Exception ex)
            {
                Program.logger.Error($"{ex.Message}  {ex.StackTrace}", ex);
                ResetFormControls();
                MessageBox.Show($"PostSummaryWebCall()\n {errorPlace}\n Error: {ex.Message} {ex.InnerException} {ex.StackTrace}");
            }
        }

        private void DisplayMarketData()
        {
            lblSandP500.Text = _markets.SAndP.CurrentLevel.NumericValue.ToString("N0");
            lblSandP500Change.Text = _markets.SAndP.Change.ToString();
            lblSandP500PercChange.Text = _markets.SAndP.PercentageChange.ToString("0.0") + "%";
            lblSandP500Change.ForeColor = _markets.SAndP.MarketColor;
            lblSandP500PercChange.ForeColor = _markets.SAndP.MarketColor;

            lblDOW30.Text = _markets.Dow.CurrentLevel.NumericValue.ToString("N0");
            lblDOW30Change.Text = _markets.Dow.Change.ToString();
            lblDOW30PercChange.Text = _markets.Dow.PercentageChange.ToString("0.0") + "%";
            lblDOW30Change.ForeColor = _markets.Dow.MarketColor;
            lblDOW30PercChange.ForeColor = _markets.Dow.MarketColor;

            lblNasdaq.Text = _markets.Nasdaq.CurrentLevel.NumericValue.ToString("N0");
            lblNasdaqChange.Text = _markets.Nasdaq.Change.ToString();
            lblNasdaqPercChange.Text = _markets.Nasdaq.PercentageChange.ToString("0.0") + "%";
            lblNasdaqChange.ForeColor = _markets.Nasdaq.MarketColor;
            lblNasdaqPercChange.ForeColor = _markets.Nasdaq.MarketColor;
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            Analyze.AnalyzeInputs analyzeInputs = new Analyze.AnalyzeInputs();
            SetUpAnalyzeInputs(analyzeInputs);
            _analyze.AnalyzeStockData(_stockDownloads, analyzeInputs, false);

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
        }

         private async void btnGetAllHistory_Click(object sender, EventArgs e)
        {
            UseWaitCursor = true;
            Application.DoEvents();
            btnGetAllHistory.Visible = false;
            await _stockDownloads.stockHistory.GetPriceHistoryForTodayWeekMonthYear(txtStockTicker.Text, _stockDownloads.stockSummary);
            BindListToHistoricPriceGrid(_stockDownloads.stockHistory.HistoricDisplayList);
            // Trends
            SetHistoricPriceTrendImages();
            UseWaitCursor = false;
        }

        private void lnkCompanyOverview_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(_stockDownloads.stockSummary.CompanyOverview, "Company Overview", MessageBoxButtons.OK);
        }

        //////////////////////////////
        //      Metrics Timer
        //////////////////////////////
        private void SetupDailyTimer()
        {
            MetricsTimer.Tick += MetricsTimer_Tick;
            SetNextDailyExecutionTime();
            MetricsTimer.Start();
        }
        private void SetNextDailyExecutionTime()
        {
            DateTime now = DateTime.Now;
            DateTime targetTime = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0); // Example: 7:00 AM
            //DateTime targetTime = DateTime.Now.AddSeconds(15); // Example: 8:00 AM

            if (now > targetTime)
            {
                targetTime = targetTime.AddDays(1); // If target time has passed today, set for tomorrow
            }

            TimeSpan timeUntilExecution = targetTime - now;
            MetricsTimer.Interval = (int)timeUntilExecution.TotalMilliseconds;
        }

        private async void MetricsTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer to prevent immediate re-triggering
            MetricsTimer.Stop();

            // Execute your daily function here
            Metrics metrics = new Metrics();
            int x = await metrics.DailyGetMetrics(PositionsDataTable, null);

            // Get news, earnings
            GetNewsEarnings();

            // Reset the timer for the next daily execution
            SetNextDailyExecutionTime();
            MetricsTimer.Start();
        }

        //////////////////////////////
        //         News / Earnings
        //////////////////////////////
        private void GetNewsEarnings()
        {
            SqlCrudOperations _finacialStatement = new SqlCrudOperations();
            List<SqlSummary> entities = _finacialStatement.GetAllSummaryList().Where(x => x.EarningsDate != null && x.EarningsDate > DateTime.Now.AddDays(-2) && x.EarningsDate < DateTime.Now.AddDays(1)).OrderBy(x => x.EarningsDate).ToList();
            entities = entities.Take(24).ToList(); // Only take first 24
            //entities.ForEach(x => x.EarningsDate = (x.EarningsDate ?? DateTime.MinValue));

            StringBuilder sb = new StringBuilder();
            foreach (SqlSummary x in entities)
            {
                if((x.EarningsDate ?? DateTime.MinValue).Date == DateTime.Now.Date.AddDays(-1))
                    sb.Append($"{x.Ticker} reported yesterday.\r\n");
                else if ((x.EarningsDate ?? DateTime.MinValue).Date == DateTime.Now.Date)
                    sb.Append($"{x.Ticker} reports today.\r\n");
            }
            _news = txtMessages.Text = sb.ToString();
        }


        //////////////////////////////
        //         Charting
        //////////////////////////////
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

        /////////////////////////////////
        //         Menu Items
        /////////////////////////////////
        ///////// Main 
        private async void runDailyMetricsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveText = runDailyMetricsToolStripMenuItem.Text;

            txtMessages.Text = "Running metrics for today...\r\n";
            runDailyMetricsToolStripMenuItem.Text = "Running...";
            runDailyMetricsToolStripMenuItem.Enabled = false;
            runDailyMetricsToolStripMenuItem.ForeColor = Color.LightBlue;
            // Execute your daily function here
            Metrics metrics = new Metrics();
            int x = await metrics.DailyGetMetrics(PositionsDataTable, txtMessages);
            runDailyMetricsToolStripMenuItem.Text = saveText;
            runDailyMetricsToolStripMenuItem.Enabled = true;
            runDailyMetricsToolStripMenuItem.ForeColor = Color.White;
            txtMessages.Text = _news;
        }
        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "logs\\app.log");

            try
            {
                Process.Start("notepad.exe", filePath);
            }
            catch (Exception ex)
            {
                Program.logger.Error($"Error opening Notepad: {ex.Message}  {ex.StackTrace}", ex);
            }
        }

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
                Program.logger.Error($"{ex.Message}  {ex.StackTrace}", ex);
                MessageBox.Show(ex.Message);
            }
        }

        private async void last20BuysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Performance performance = new Performance(_stockDownloads.stockSummary);
            if(_markets.Dow == null)
            {
                _markets.Dow = await _markets.GetMarketData("^DJI", true);
            }
            performance.GetLatestBuyPerformance(_markets.Dow, PositionsDataTable, TradesDataTable);
            performance.ShowPerformanceForm(this);  
        }
        private void latestSellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Performance performance = new Performance(_stockDownloads.stockSummary);
            List<PerformanceItem> performanceList = performance.GetLatestSellPerformance(PositionsDataTable, TradesDataTable);
            performance.ShowLiquidationPerformanceForm(this, performanceList, "Sell Performance", 0);
        }

        private async void liquidationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Performance performance = new Performance(_stockDownloads.stockSummary);
            List<PerformanceItem> performanceList =  await performance.GetLiquidationPerformance(TradesDataTable);
            performance.ShowLiquidationPerformanceForm(this, performanceList, "Liquidation Performance", 1);
        }

        private void offHighsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> tickers = OffHighs.GetHighMetricTickers(PositionsDataTable);

            OffHighs offHighs = new OffHighs(tickers, PositionsDataTable, TradesDataTable);
            offHighs.Owner = this;
            offHighs.Show();
        }

        private void watchOffHighsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> tickers = OffHighs.GetWatchListTickers(PositionsDataTable);

            OffHighs offHighs = new OffHighs(tickers, PositionsDataTable, TradesDataTable);
            offHighs.Owner = this;
            offHighs.Show();
        }

        private void summaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void incomeStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCrudOperations _finacialStatement = new SqlCrudOperations();
            _finacialStatement.DeleteIncomeStatement(txtStockTicker.Text);
            btnGetOne_click(null, null);
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCrudOperations _finacialStatement = new SqlCrudOperations();
            _finacialStatement.DeleteStatistics(txtStockTicker.Text);
            btnGetOne_click(null, null);
        }

        private void earningDatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlCrudOperations _finacialStatement = new SqlCrudOperations();
            List<SqlSummary> entities = _finacialStatement.GetAllSummaryList().Where(x => x.EarningsDate != null && x.EarningsDate > DateTime.Now).OrderBy(x => x.EarningsDate).ToList();

            StringBuilder sb = new StringBuilder();
            foreach (SqlSummary x in entities)
            {
                if (x.EarningsDate > DateTime.Now.AddDays(4))
                    break;

                sb.Append($"{x.Ticker,-12} {(x.EarningsDate ?? DateTime.Now).Date:MM/dd/yyyy}\n");
            }

            MessageBox.Show(sb.ToString(), "Coming Up Earnings");
        }

        private void stockMetricsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MetricsForm f = new MetricsForm(txtStockTicker.Text);
            f.Owner = this;
            f.Show();
        }
    }

    public class CustomColorTable : ProfessionalColorTable
    {
        //a bunch of other overrides...

        public override Color ToolStripBorder
        {
            get { return Color.FromArgb(0, 0, 0); }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.FromArgb(8, 32, 64); }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.FromArgb(8, 32, 64); }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(8, 32, 64); }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb(8, 32, 64); }
        }
        public override Color ToolStripDropDownBackground
        {
            get { return Color.FromArgb(8, 32, 64); }
        }
        public override Color ToolStripGradientBegin
        {
            get { return Color.FromArgb(8, 32, 64); }
        }
        public override Color ToolStripGradientEnd
        {
            get { return Color.FromArgb(8, 32, 64); }
        }
        public override Color ToolStripGradientMiddle
        {
            get { return Color.FromArgb(8, 32, 64); }
        }
    }
}
