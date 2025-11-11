using SqlLayer;
using SqlLayer.SQL_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YahooLayer;

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
        public List<HistoricPriceData> HistoricDisplayList = new List<StockHistory.HistoricPriceData>();


        public StockHistory()
        {
            _yahooFinanceAPI = new YahooFinanceAPI();
        }

        public async Task<decimal> GetTodaysPrice(string ticker)
        {
            /////// Get price history, today, week ago, month ago to determine short trend
            List<StockQuote> quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddDays(-3), 4, "1d");

            if (quoteList.Count > 0)
            {
                // Today will be the last in the list
                StockQuote stockQuote = quoteList.Last();
                if(stockQuote.Close == 0)
                {
                    stockQuote.Close = stockQuote.Price; // for mutual funds or ETF
                }
                
                return stockQuote.Close;
            }
            else
            {
                return 0;
            }
        }

        public async Task<List<StockHistory.HistoricPriceData>> GetPriceHistoryFor3Year(string ticker, StockSummary summary)
        {
            Ticker = ticker;
            
            /////// Get price history from 3 years ago
            List<StockQuote> quoteList = new List<StockQuote>();

            bool hasSqlData = CheckSqlForRecentData();
            if (!hasSqlData)
            {
                try
                {
                    // Some stocks didn't exist 3 years ago
                    quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddYears(-3).AddDays(-4), 4, "1d");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Data doesn't exist for startDate"))
                    {
                        // try a year ago
                        quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddYears(-1).AddDays(-4), 4, "1d");
                    }
                    else
                    {
                        Program.logger.Error($"{ex.Message}  {ex.StackTrace}");
                        MessageBox.Show(ex.Message);
                    }
                }

                if (quoteList.Count > 0)
                {
                    StockQuote stockQuote = quoteList.Last();
                    HistoricData3YearsAgo = HistoricPriceData.MapFromApiStockQuote(stockQuote, "3Y");
                }

                if (summary.YearsRangeLow.StringValue == summary.YearsRangeHigh.StringValue) // Mutual fund range was faked
                {
                    if (HistoricData3YearsAgo.Price < summary.PriceString.NumericValue) // going up
                        summary.YearsRangeLow.NumericValue = (HistoricData3YearsAgo.Price + summary.YearsRangeLow.NumericValue) / 2;
                    else
                    {
                        summary.YearsRangeLow.NumericValue = summary.PriceString.NumericValue * .95M;
                        summary.YearsRangeHigh.NumericValue = (HistoricData3YearsAgo.Price + summary.YearsRangeLow.NumericValue) / 2;
                    }
                }
                HistoricData3YearsAgo.Ticker = Ticker;

                ///////////////////////////////////
                ///      Save to SQL Server
                List<SqlPriceHistory> rows = MapFrom(this);
                SqlCrudOperations _finacialStatement = new SqlCrudOperations();
                _finacialStatement.SavePriceHistories(rows);
            }

            // todaay's price not saved to sql server.
            HistoricDisplayList = new List<HistoricPriceData>();
            HistoricDataToday = new HistoricPriceData() { PeriodType = "D", Price = summary.PriceString.NumericValue, PriceDate = DateTime.Now.Date, Ticker = ticker, Volume = 0M };
            HistoricDisplayList.Add(HistoricDataToday);
            HistoricDisplayList.Add(HistoricData3YearsAgo);

            // Set historic price trends
            SetTrends();

            return HistoricDisplayList;
        }

        public async Task<List<StockHistory.HistoricPriceData>> GetPriceHistoryForTodayWeekMonthYear(string ticker, StockSummary summary)
        {
            List<StockQuote> quoteList = new List<StockQuote>();
            /////// Get price history, today, week ago, month ago to determine short trend
            DateTime findDate;

            Ticker = ticker;

            HistoricDataWeekAgo = null;
            HistoricDataMonthAgo = null;
            HistoricDataYearAgo = null;
            quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddMonths(-1).AddDays(-1), 34, "1d");

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

                /////// Get price history for a year ago to determine long trend
                bool hasSqlData = CheckSqlForRecentData();

                if (hasSqlData && HistoricDataYearAgo != null && HistoricDataYearAgo.Price > 0)
                    hasSqlData = true;
                else
                    hasSqlData = false; // only has 3 years ago price and not last years price

                if(hasSqlData == false)
                {
                    quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddYears(-1).AddDays(-4), 4, "1d");

                    if (quoteList.Count > 0)
                    {
                        stockQuote = quoteList.Last();
                        HistoricDataYearAgo = HistoricPriceData.MapFromApiStockQuote(stockQuote, "Y");
                    }

                    ///////////////////////////////////
                    ///      Save to SQL Server
                    List<SqlPriceHistory> rows = MapFrom(this);
                    SqlCrudOperations _finacialStatement = new SqlCrudOperations();
                    _finacialStatement.SavePriceHistories(rows);
                }
            }

            HistoricDisplayList = new List<HistoricPriceData>();

            if (HistoricDataToday != null)
            {
                HistoricDataToday.Ticker = Ticker;
                HistoricDisplayList.Add(HistoricDataToday);
            }
            if (HistoricDataWeekAgo != null)
            {
                HistoricDataWeekAgo.Ticker = Ticker;
                HistoricDisplayList.Add(HistoricDataWeekAgo);
            }
            if (HistoricDataMonthAgo != null)
            {
                HistoricDataMonthAgo.Ticker = Ticker;
                HistoricDisplayList.Add(HistoricDataMonthAgo);
            }
            if (HistoricDataYearAgo != null)
            {
                HistoricDataYearAgo.Ticker = Ticker;
                HistoricDisplayList.Add(HistoricDataYearAgo);
            }
            if (HistoricData3YearsAgo != null)
            {
                HistoricData3YearsAgo.Ticker = Ticker;
                HistoricDisplayList.Add(HistoricData3YearsAgo);
            }

            // Set historic price trends
            SetTrends();

            return HistoricDisplayList;
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

        /// /////////////////////////////////////////////////////////////////////////////////////////
        ///                                    SQL Support
        /// /////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckSqlForRecentData()
        {
            SqlCrudOperations sqlFinancialStatement = new SqlCrudOperations();
            List<SqlPriceHistory> entities = sqlFinancialStatement.GetPriceHistoryList(Ticker);
            Random random = new Random();

            if (entities.Count > 0)
            {
                DateTime staleDate = DateTime.Now.Date.AddDays(-14 + random.Next(1, 4));

                if (entities[0].UpdateDate > staleDate) // We have recent data in the database, use it.
                {
                    MapFill(entities);
                    return true;
                }

                return false; // need to go out and download new data
            }

            return false;
        }

        public static List<SqlPriceHistory> MapFrom(StockHistory source)
        {
            SqlPriceHistory sqlPriceHistory = null;
            List<SqlPriceHistory> sqlPriceHistories = new List<SqlPriceHistory>();

            // 3 Year
            sqlPriceHistory = MapPeriodPrice(source, sqlPriceHistories, source.HistoricData3YearsAgo);
            sqlPriceHistories.Add(sqlPriceHistory);

            // 1 Year
            sqlPriceHistory = MapPeriodPrice(source, sqlPriceHistories, source.HistoricDataYearAgo);
            sqlPriceHistories.Add(sqlPriceHistory);

            // 1 Month
            sqlPriceHistory = MapPeriodPrice(source, sqlPriceHistories, source.HistoricDataMonthAgo);
            sqlPriceHistories.Add(sqlPriceHistory);

            // 1 Week
            sqlPriceHistory = MapPeriodPrice(source, sqlPriceHistories, source.HistoricDataWeekAgo);
            sqlPriceHistories.Add(sqlPriceHistory);

            return sqlPriceHistories;
        }

        private static SqlPriceHistory MapPeriodPrice(StockHistory source, List<SqlPriceHistory> sqlPriceHistories, HistoricPriceData priceData)
        {
            SqlPriceHistory sqlPriceHistory = null;

            if (priceData != null && priceData.Price > 0)
            {
                sqlPriceHistory = new SqlPriceHistory()
                {
                    Ticker = source.Ticker,
                    PeriodType = priceData.PeriodType,
                    PriceDate = priceData.PriceDate,
                    Price = (double)priceData.Price,
                    Volume = (double)priceData.Volume,
                    UpdateDate = DateTime.Now
                };

            }

            return sqlPriceHistory;
        }

        public void MapFill(List<SqlPriceHistory> sourceList)
        {
            //sourceList = sourceList.OrderBy(x => x.PriceDate).ToList();

            foreach (SqlPriceHistory row in sourceList)
            {
                if (row.PeriodType == "3Y")
                {
                    HistoricData3YearsAgo = new HistoricPriceData();
                    HistoricData3YearsAgo.Ticker = row.Ticker;
                    HistoricData3YearsAgo.PeriodType = row.PeriodType;
                    HistoricData3YearsAgo.PriceDate = row.PriceDate;
                    HistoricData3YearsAgo.Price = (decimal)row.Price;
                    HistoricData3YearsAgo.Volume = (decimal)row.Volume;
                }

                if (row.PeriodType == "Y")
                {
                    HistoricDataYearAgo = new HistoricPriceData();
                    HistoricDataYearAgo.Ticker = row.Ticker;
                    HistoricDataYearAgo.PeriodType = row.PeriodType;
                    HistoricDataYearAgo.PriceDate = row.PriceDate;
                    HistoricDataYearAgo.Price = (decimal)row.Price;
                    HistoricDataYearAgo.Volume = (decimal)row.Volume;
                }

                if (row.PeriodType == "M")
                {
                    HistoricDataMonthAgo = new HistoricPriceData();
                    HistoricDataMonthAgo.Ticker = row.Ticker;
                    HistoricDataMonthAgo.PeriodType = row.PeriodType;
                    HistoricDataMonthAgo.PriceDate = row.PriceDate;
                    HistoricDataMonthAgo.Price = (decimal)row.Price;
                    HistoricDataMonthAgo.Volume = (decimal)row.Volume;
                }
                if (row.PeriodType == "W")
                {
                    HistoricDataWeekAgo = new HistoricPriceData();
                    HistoricDataWeekAgo.Ticker = row.Ticker;
                    HistoricDataWeekAgo.PeriodType = row.PeriodType;
                    HistoricDataWeekAgo.PriceDate = row.PriceDate;
                    HistoricDataWeekAgo.Price = (decimal)row.Price;
                    HistoricDataWeekAgo.Volume = (decimal)row.Volume;
                }
            }

            return;
        }

        public class HistoricPriceData
        {
            public string Ticker { get; set; }
            public string PeriodType { get; set; }
            public DateTime PriceDate { get; set; }
            public decimal Price { get; set; }
            public decimal Volume { get; set; }

            public static HistoricPriceData MapFromApiStockQuote(StockQuote stockQuote, string periodType)
            {
                HistoricPriceData historicPriceData = new HistoricPriceData();

                historicPriceData.Ticker = stockQuote.Ticker;
                historicPriceData.PeriodType = periodType;
                historicPriceData.Price = Math.Round(stockQuote.Close, 2);
                historicPriceData.PriceDate = stockQuote.QuoteDate;
                historicPriceData.Volume = stockQuote.Volume;

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
