using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using StockApi.Downloads;

namespace StockApi
{
    /// <summary>
    /// Daily export of metrics to a csv for import into a spreadsheet
    /// </summary>
    public class Metrics
    {
        //private StockSummary _stockSummary = new StockSummary();
        //private StockIncomeStatement _stockFinancials = new StockIncomeStatement();
        //private StockStatistics _stockStatistics = new StockStatistics();
        //private StockHistory _stockHistory = new StockHistory();
        private Analyze _analyze = new Analyze();
        private ExcelManager _excelManager = new ExcelManager();

        public async Task<int> DailyGetMetrics(DataTable positionsDataTable)
        {
            // Get all tickers from position table
            List<string> stockList = new List<string>();
            StringBuilder builder = new StringBuilder();
            Analyze.AnalyzeInputs analyzeInputs = new Analyze.AnalyzeInputs();
            analyzeInputs.SharesOwned = 1;
            analyzeInputs.QuantityTraded = 1;

            bool networkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            if (networkUp == false)
            {
                try
                {
                    //await new MarketData().GetMarketData("^GSPC", false);
                    YahooFinance.RenewIPAddress();
                    Thread.Sleep(2000); // wait 2 seconds
                }
                catch
                {
                    networkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

                    if (networkUp == false)
                    {
                        MessageBox.Show("Your network connection is unavailable.");
                        return 1;
                    }
                }
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            stockList = _excelManager.GetStockListFromPositionsTable(positionsDataTable);
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                //stockList = stockList.Skip(0).Take(4).ToList();
                stockList = stockList.Skip(0).Take(30).ToList();
                desktopPath = Path.Combine(desktopPath, "StockMetricsMonday.txt");
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
            {
                stockList = stockList.Skip(30).Take(30).ToList();
                desktopPath = Path.Combine(desktopPath, "StockMetricsTuesday.txt");
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
            {
                stockList = stockList.Skip(60).Take(30).ToList();
                desktopPath = Path.Combine(desktopPath, "StockMetricsWednesday.txt");
            }

            //if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
            //{
            //    stockList = stockList.Skip(4).Take(1).ToList();
            //    desktopPath = Path.Combine(desktopPath, "Test.txt");
            //}

            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                stockList = stockList.Skip(90).Take(30).ToList();
                desktopPath = Path.Combine(desktopPath, "StockMetricsFriday_T-V.txt");
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                stockList = stockList.Skip(120).Take(30).ToList();
                desktopPath = Path.Combine(desktopPath, "StockMetricsSaturday_V-Z.txt");
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return 0;

            string stockMetricString = "";
            foreach (string ticker in stockList)
            {
                stockMetricString = await GetStockMetric(ticker, analyzeInputs);
                builder.Append(stockMetricString);
                Debug.Print(stockMetricString);
                Thread.Sleep(800);
            }

            if (Directory.Exists(desktopPath))
                Directory.Delete(desktopPath);
            File.WriteAllText(desktopPath, builder.ToString());


            MessageBox.Show("Daily function executed!");

            return 0;
        }

        public async Task<string> GetStockMetric(string ticker, Analyze.AnalyzeInputs analyzeInputs)
        {
            bool _tickerFound;
            string stockMetricString;
            StockDownloads stockDownloads = new StockDownloads(ticker);

            _tickerFound = await stockDownloads.GetAllStockData();

            if (stockDownloads.stockSummary.LastException != null)
            {
                return $"{ticker} Error {stockDownloads.stockSummary.Error}";
            }


            // Calculated PE can only be figured after both summary and finacial data is combined
            stockDownloads.stockSummary.SetCalculatedPE(stockDownloads);

            if (stockDownloads.stockHistory.HistoricDisplayList.Count > 0)
                stockDownloads.stockHistory.HistoricData3YearsAgo = stockDownloads.stockHistory.HistoricDisplayList.Last();
            else
                stockDownloads.stockHistory.HistoricData3YearsAgo = new StockHistory.HistoricPriceData() { Ticker = stockDownloads.stockSummary.Ticker, Price = stockDownloads.stockSummary.PriceString.NumericValue };

            decimal percent_diff = stockDownloads.stockSummary.PriceString.NumericValue / stockDownloads.stockHistory.HistoricData3YearsAgo.Price - 1M;

            decimal totalMetric = _analyze.AnalyzeStockData(stockDownloads, analyzeInputs, true);
            if(ticker == "ABR" || ticker == "KIM" || ticker == "ACHR" || ticker == "AMZN")
            {
                Debug.WriteLine(_analyze.AnalysisMetricsOutputText);
            }

            stockMetricString = $"{stockDownloads.stockSummary.Ticker}, {stockDownloads.stockSummary.VolatilityString.NumericValue}, {stockDownloads.stockSummary.EarningsPerShareString.NumericValue}, {stockDownloads.stockSummary.OneYearTargetPriceString.NumericValue},"
                                     + $" {stockDownloads.stockSummary.PriceBookString.NumericValue}, {stockDownloads.stockSummary.ProfitMarginString.NumericValue}, {stockDownloads.stockSummary.DividendString.NumericValue}, {stockDownloads.stockStatistics.ShortInterestString.NumericValue}"
                                     + $", {stockDownloads.stockHistory.HistoricData3YearsAgo.Price}, {percent_diff.ToString("0.00")},{stockDownloads.stockSummary.YearsRangeLow.NumericValue},{stockDownloads.stockSummary.YearsRangeHigh.NumericValue},{totalMetric}{Environment.NewLine}";

            return stockMetricString;
        }
    }
}
