using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockApi
{
    /// <summary>
    /// Daily export of metrics to a csv for import into a spreadsheet
    /// </summary>
    public class Metrics
    {
        private StockSummary _stockSummary = new StockSummary();
        private StockFinancials _stockFinancials = new StockFinancials();
        private StockHistory _stockHistory = new StockHistory();
        private Analyze _analyze = new Analyze();
        private ExcelManager _excelManager = new ExcelManager();

        public async Task<int> DailyGetMetrics(DataTable positionsDataTable)
        {
            // Get all tickers from position table
            List<string> stockList = new List<string>();
            Analyze.AnalyzeInputs analyzeInputs = new Analyze.AnalyzeInputs();
            analyzeInputs.SharesOwned = 1;
            analyzeInputs.QuantityTraded = 1;
            analyzeInputs.MarketHealth = 5;

            StringBuilder builder = new StringBuilder();

            bool networkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (networkUp == false)
            {
                MessageBox.Show("Your network connection is unavailable.");
                return 1;
            }

            stockList = _excelManager.GetStockListFromPositionsTable(positionsDataTable);



            // set up file for output           form1.txtTickerList.Text = "";
            //builder.Append($"Ticker, Volatility, EarningsPerShare, OneYearTargetPrice, PriceBook, ProfitMargin, Dividend, 3YearPrice, 3YearPriceChange{Environment.NewLine}");
            foreach (string ticker in stockList)
            {

                string stockMetricString = await GetStockMetric(ticker, analyzeInputs);

                // TODO  Write to file.

                Thread.Sleep(800);
            }

            MessageBox.Show("Daily function executed!");

            return 0;
        }

        public async Task<string> GetStockMetric(string ticker, Analyze.AnalyzeInputs analyzeInputs)
        {
            bool _tickerFound;

            List<StockHistory.HistoricPriceData> historicDataList = new List<StockHistory.HistoricPriceData>();

            _stockSummary.Ticker = ticker.Substring(0, (ticker + ",").IndexOf(",")).ToUpper();

            // Extract the individual data values from the html
            _tickerFound = await _stockSummary.GetSummaryData(_stockSummary.Ticker);

            if (_stockSummary.EarningsPerShareString.StringValue == "--")
            {
                Thread.Sleep(2000);
                _tickerFound = await _stockSummary.GetSummaryData(_stockSummary.Ticker);
                if (_stockSummary.EarningsPerShareString.StringValue == "--")
                {
                    Thread.Sleep(2000);
                    _tickerFound = await _stockSummary.GetSummaryData(_stockSummary.Ticker);
                    if (_stockSummary.EarningsPerShareString.StringValue == "--")
                    {
                        Thread.Sleep(2000);
                        _tickerFound = await _stockSummary.GetSummaryData(_stockSummary.Ticker);
                    }
                }
            }

            await _stockFinancials.GetFinancialData(_stockSummary.Ticker);

            // get 3 year ago price
            historicDataList = await _stockHistory.GetPriceHistoryForTodayWeekMonthYear(ticker, _stockSummary, true, false, false);

            if (historicDataList.Count > 0)
                _stockHistory.HistoricData3YearsAgo = historicDataList.Last();
            else
                _stockHistory.HistoricData3YearsAgo = new StockHistory.HistoricPriceData() { Ticker = _stockSummary.Ticker, Price = _stockSummary.PriceString.NumericValue };

            decimal percent_diff = _stockSummary.PriceString.NumericValue / _stockHistory.HistoricData3YearsAgo.Price - 1M;

            decimal totalMetric = _analyze.AnalyzeStockData(_stockSummary, _stockHistory, _stockFinancials, analyzeInputs, true);

            string stockMetricString = $"{_stockSummary.Ticker}, {_stockSummary.VolatilityString.NumericValue}, {_stockSummary.EarningsPerShareString.NumericValue}, {_stockSummary.OneYearTargetPriceString.NumericValue},"
                                     + $" {_stockSummary.PriceBookString.NumericValue}, {_stockSummary.ProfitMarginString.NumericValue}, {_stockSummary.DividendString.NumericValue}, {_stockFinancials.ShortInterestString.NumericValue}"
                                     + $", {_stockHistory.HistoricData3YearsAgo.Price}, {percent_diff.ToString("0.00")},{_stockSummary.YearsRangeLow.NumericValue},{_stockSummary.YearsRangeHigh.NumericValue},{totalMetric}{Environment.NewLine}";

            return stockMetricString;
        }
    }
}
