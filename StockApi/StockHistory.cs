using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockApi
{
    /// <summary>
    /// Goes out on the web to the Yahoo financial page and gets stock history data.
    /// It determines long and short term price trends for the stock.
    /// </summary>
    public class StockHistory : YahooFinance
    {
        public enum TrendEnum
        {
            Down,
            Sideways,
            Up,
            Unknown
        }

        YahooFinanceAPI _yahooFinanceAPI;

        public HistoricPriceData HistoricDataToday;
        public HistoricPriceData HistoricDataWeekAgo;
        public HistoricPriceData HistoricDataMonthAgo;
        public HistoricPriceData HistoricDataYearAgo;
        public HistoricPriceData HistoricData3YearsAgo;
        public TrendEnum WeekTrend = TrendEnum.Sideways;
        public TrendEnum MonthTrend = TrendEnum.Sideways;
        public TrendEnum YearTrend = TrendEnum.Sideways;
        public TrendEnum ThreeYearTrend = TrendEnum.Sideways;

        public StockHistory()
        {
            _yahooFinanceAPI = new YahooFinanceAPI();
        }

        public async Task<decimal> GetTodaysPrice(string ticker)
        {
            /////// Get price history, today, week ago, month ago to determine short trend
            List<StockQuote> quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddDays(-3), 4);

            if (quoteList.Count > 0)
            {
                // Today will be the last in the list
                StockQuote stockQuote = quoteList.Last();
                return stockQuote.Close;
            }
            else
            {
                return 0;
            }
        }

        public async Task<List<StockHistory.HistoricPriceData>> GetPriceHistoryForTodayWeekMonthYear(string ticker, StockSummary summary, bool get3Year, bool get1Year, bool getMonthAndWeek)
        {
            /////// Get price history, today, week ago, month ago to determine short trend
            DateTime findDate;

            HistoricDataWeekAgo = null;
            HistoricDataMonthAgo = null;
            HistoricDataYearAgo = null;
            if (getMonthAndWeek)
            {
                List<StockQuote> quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddMonths(-1).AddDays(-1), 34);

                if (quoteList.Count > 0)
                {
                    // Today will be the last in the list
                    StockQuote stockQuote = quoteList.Last();
                    HistoricDataToday = HistoricPriceData.MapFromApiStockQuote(stockQuote, "D");

                    // Last Week
                    findDate = GetMondayIfWeekend(DateTime.Now.AddDays(-7).Date);
                    stockQuote = quoteList.Find(x => x.QuoteDate.Date == findDate.Date || x.QuoteDate.Date == findDate.AddDays(1));
                    if (stockQuote != null)
                    {
                        HistoricDataWeekAgo = HistoricPriceData.MapFromApiStockQuote(stockQuote, "W");
                    }

                    //// Last Month (really 31 days ago)
                    findDate = GetMondayIfWeekend(DateTime.Now.AddMonths(-1).Date);
                    stockQuote = quoteList.Find(x => x.QuoteDate.Date == findDate.Date || x.QuoteDate.Date == findDate.AddDays(1) || x.QuoteDate.Date == findDate.AddDays(2));
                    if (stockQuote != null)
                    {
                        HistoricDataMonthAgo = HistoricPriceData.MapFromApiStockQuote(stockQuote, "M");
                    }
                }
            }
            else
            {
                HistoricDataToday = new HistoricPriceData() { PeriodType = "D", Price = summary.PriceString.NumericValue, PriceDate = DateTime.Now.Date, Ticker = ticker, Volume = "N/A" };
            }

            /////// Get price history for a year ago to determine long trend
            if (get1Year)
            {
                List<StockQuote> quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddYears(-1).AddDays(-4), 4);

                if (quoteList.Count > 0)
                {
                    StockQuote stockQuote = quoteList.Last();
                    HistoricDataYearAgo = HistoricPriceData.MapFromApiStockQuote(stockQuote, "Y");
                }
            }


            /////// Get price history for 3 years ago to determine long trend
            if (get3Year)
            {
                List<StockQuote> quoteList = new List<StockQuote>();
                try
                {
                    quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddYears(-3).AddDays(-4), 4);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }

                if (quoteList.Count > 0)
                {
                    StockQuote stockQuote = quoteList.Last();
                    HistoricData3YearsAgo = HistoricPriceData.MapFromApiStockQuote(stockQuote, "3Y");
                }

                if(summary.YearsRangeLow.StringValue == summary.YearsRangeHigh.StringValue) // Mutual fund range was faked
                {
                    if (HistoricData3YearsAgo.Price < summary.PriceString.NumericValue) // going up
                        summary.YearsRangeLow.NumericValue = (HistoricData3YearsAgo.Price + summary.YearsRangeLow.NumericValue) / 2;
                    else
                    {
                        summary.YearsRangeLow.NumericValue = summary.PriceString.NumericValue * .95M;
                        summary.YearsRangeHigh.NumericValue = (HistoricData3YearsAgo.Price + summary.YearsRangeLow.NumericValue) / 2;
                    }
                }
            }

            List<StockHistory.HistoricPriceData> historicDisplayList = new List<StockHistory.HistoricPriceData>();
            if (HistoricDataToday != null)
                historicDisplayList.Add(HistoricDataToday);
            if (HistoricDataWeekAgo != null)
                historicDisplayList.Add(HistoricDataWeekAgo);
            if (HistoricDataMonthAgo != null)
                historicDisplayList.Add(HistoricDataMonthAgo);
            if (HistoricDataYearAgo != null)
                historicDisplayList.Add(HistoricDataYearAgo);
            if (HistoricData3YearsAgo != null)
                historicDisplayList.Add(HistoricData3YearsAgo);

            return historicDisplayList;
        }

        private static DateTime GetMondayIfWeekend(DateTime theDate)
        {
            if (theDate.DayOfWeek == DayOfWeek.Saturday)
                theDate = theDate.AddDays(2); // return Monday

            if (theDate.DayOfWeek == DayOfWeek.Sunday)
            {
                theDate = theDate.AddDays(1); // return Monday
            }

            return theDate;
        }
         
        public void SetTrends()
        {
            if (HistoricData3YearsAgo != null)
            {
                if (HistoricDataToday.Price > HistoricData3YearsAgo.Price * 1.12M) // 3 year
                    ThreeYearTrend = TrendEnum.Up;
                else if (HistoricDataToday.Price < HistoricData3YearsAgo.Price * .88M) // 3 year
                    ThreeYearTrend = TrendEnum.Down;
                else
                    ThreeYearTrend = TrendEnum.Sideways;
            }
            else
                ThreeYearTrend = TrendEnum.Unknown;

            if (HistoricDataYearAgo != null)
            {
                if (HistoricDataToday.Price > HistoricDataYearAgo.Price * 1.08M) // year
                    YearTrend = TrendEnum.Up;
                else if (HistoricDataToday.Price < HistoricDataYearAgo.Price * .92M) // year
                    YearTrend = TrendEnum.Down;
                else
                    YearTrend = TrendEnum.Sideways;
            }
            else
                YearTrend = TrendEnum.Unknown;

            if (HistoricDataMonthAgo != null)
            {
                if (HistoricDataToday.Price > HistoricDataMonthAgo.Price * 1.06M) // month
                    MonthTrend = TrendEnum.Up;
                else if (HistoricDataToday.Price < HistoricDataMonthAgo.Price * .94M) // month
                    MonthTrend = TrendEnum.Down;
                else
                    MonthTrend = TrendEnum.Sideways;
            }
            else
                MonthTrend = TrendEnum.Unknown;

            if (HistoricDataWeekAgo != null)
            {
                if (HistoricDataToday.Price > HistoricDataWeekAgo.Price * 1.03M) // week
                    WeekTrend = TrendEnum.Up;
                else if (HistoricDataToday.Price < HistoricDataWeekAgo.Price * .97M) // week
                    WeekTrend = TrendEnum.Down;
                else
                    WeekTrend = TrendEnum.Sideways;
            }
            else
                WeekTrend = TrendEnum.Unknown;
        }

        public class HistoricPriceData
        {
            public string Ticker { get; set; }
            public string PeriodType { get; set; }
            public DateTime PriceDate { get; set; }
            public decimal Price { get; set; }
            public string Volume { get; set; }

            public static HistoricPriceData MapFromApiStockQuote(StockQuote stockQuote, string periodType)
            {
                HistoricPriceData historicPriceData = new HistoricPriceData();

                historicPriceData.Ticker = stockQuote.Ticker;
                historicPriceData.PeriodType = periodType;
                historicPriceData.Price = stockQuote.Close;
                historicPriceData.PriceDate = stockQuote.QuoteDate;
                historicPriceData.Volume = stockQuote.Volume.ToString("N0");

                return historicPriceData;
            }

            public override string ToString()
            {
                return string.Format(
                    $"{Ticker}, {PeriodType}, {PriceDate.ToString("MMM dd, yyyy")}, {Price}"
                ); ;
            }
        }
    }
}
