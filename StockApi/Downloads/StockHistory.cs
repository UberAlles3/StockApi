using SqlLayer.SQL_Models;
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
            /////// Get price history from 3 years ago
            List<StockQuote> quoteList = new List<StockQuote>();
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
                    MessageBox.Show(ex.Message);
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
                quoteList = await _yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddYears(-1).AddDays(-4), 4, "1d");

                if (quoteList.Count > 0)
                {
                    stockQuote = quoteList.Last();
                    HistoricDataYearAgo = HistoricPriceData.MapFromApiStockQuote(stockQuote, "Y");
                }
            }

            HistoricDisplayList = new List<HistoricPriceData>();

            if (HistoricDataToday != null)
                HistoricDisplayList.Add(HistoricDataToday);
            if (HistoricDataWeekAgo != null)
                HistoricDisplayList.Add(HistoricDataWeekAgo);
            if (HistoricDataMonthAgo != null)
                HistoricDisplayList.Add(HistoricDataMonthAgo);
            if (HistoricDataYearAgo != null)
                HistoricDisplayList.Add(HistoricDataYearAgo);
            if (HistoricData3YearsAgo != null)
                HistoricDisplayList.Add(HistoricData3YearsAgo);

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

        public static List<SqlPriceHistory> MapFrom(StockHistory source)
        {
            List<SqlPriceHistory> sqlPriceHistories = new List<SqlPriceHistory>();

            // 3 Year
            sqlPriceHistories.Add(new SqlPriceHistory()
            {
                Ticker = source.Ticker,
                PeriodType = source.HistoricData3YearsAgo.PeriodType,
                PriceDate  = source.HistoricData3YearsAgo.PriceDate,
                Price      = (double)source.HistoricData3YearsAgo.Price,
                Volume     = (double)source.HistoricData3YearsAgo.Volume,
                UpdateDate = DateTime.Now.Date
            });

            // 1 Year
            sqlPriceHistories.Add(new SqlPriceHistory()
            {
                Ticker = source.Ticker,
                PeriodType = source.HistoricDataYearAgo.PeriodType,
                PriceDate = source.HistoricDataYearAgo.PriceDate,
                Price = (double)source.HistoricDataYearAgo.Price,
                Volume = (double)source.HistoricDataYearAgo.Volume,
                UpdateDate = DateTime.Now.Date
            });


            //// 4 years ago
            //sqlPriceHistorys.Add(new SqlPriceHistory()
            //{
            //    Ticker = source.Ticker,
            //    Year = DateTime.Now.AddYears(-4).Year,
            //    Revenue = (double)source.Revenue4String.NumericValue,
            //    CostOfRevenue = (double)source.CostOfRevenue4String.NumericValue,
            //    OperatingExpense = (double)source.OperatingExpense4String.NumericValue,
            //    NetIncome = (double)source.NetIncome4String.NumericValue,
            //    BasicEPS = (double)source.BasicEps4String.NumericValue,
            //    UpdateDate = DateTime.Now.Date
            //});

            return sqlPriceHistories;
        }

        public void MapFill(List<SqlIncomeStatement> sourceList)
        {
            sourceList = sourceList.OrderBy(x => x.Year).ToList();

            //// 4 years ago
            //Revenue4String.NumericValue = (decimal)sourceList[0].Revenue;
            //CostOfRevenue4String.NumericValue = (decimal)sourceList[0].CostOfRevenue;
            //OperatingExpense4String.NumericValue = (decimal)sourceList[0].OperatingExpense;
            //NetIncome4String.NumericValue = (decimal)sourceList[0].NetIncome;
            //BasicEps4String.NumericValue = (decimal)sourceList[0].BasicEPS;

            //// 2 years ago
            //Revenue2String.NumericValue = (decimal)sourceList[1].Revenue;
            //CostOfRevenue2String.NumericValue = (decimal)sourceList[1].CostOfRevenue;
            //OperatingExpense2String.NumericValue = (decimal)sourceList[1].OperatingExpense;
            //NetIncome2String.NumericValue = (decimal)sourceList[1].NetIncome;
            //BasicEps2String.NumericValue = (decimal)sourceList[1].BasicEPS;

            //// TTM
            //RevenueTtmString.NumericValue = (decimal)sourceList[2].Revenue;
            //CostOfRevenueTtmString.NumericValue = (decimal)sourceList[2].CostOfRevenue;
            //OperatingExpenseTtmString.NumericValue = (decimal)sourceList[2].OperatingExpense;
            //NetIncomeTtmString.NumericValue = (decimal)sourceList[2].NetIncome;
            //BasicEpsTtmString.NumericValue = (decimal)sourceList[2].BasicEPS;

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
