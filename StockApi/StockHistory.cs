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
        private static readonly string _url = "https://finance.yahoo.com/quote/?ticker?/history?period1=?period1?&period2=?period2?&filter=history&frequency=1d&includeAdjustedClose=true";

        public enum TrendEnum
        {
            Down,
            Sideways,
            Up,
            Unknown
        }

        public HistoricPriceData HistoricDataToday;
        public HistoricPriceData HistoricDataWeekAgo;
        public HistoricPriceData HistoricDataMonthAgo;
        public HistoricPriceData HistoricDataYearAgo;
        public HistoricPriceData HistoricData3YearsAgo;
        public TrendEnum WeekTrend = TrendEnum.Sideways;
        public TrendEnum MonthTrend = TrendEnum.Sideways;
        public TrendEnum YearTrend = TrendEnum.Sideways;
        public TrendEnum ThreeYearTrend = TrendEnum.Sideways;

        public async Task<List<StockHistory.HistoricPriceData>> GetPriceHistoryForTodayWeekMonthYear(string ticker, StockSummary summary, bool get3Year, bool get1Year, bool getMonthAndWeek)
        {
            /////// Get price history, today, week ago, month ago to determine short trend
            List<StockHistory.HistoricPriceData> historicDataList;
            DateTime findDate;

            HistoricDataWeekAgo = null;
            HistoricDataMonthAgo = null;
            HistoricDataYearAgo = null;
            if (getMonthAndWeek)
            {
                historicDataList = await GetHistoricalDataForDateRange(ticker, DateTime.Now.AddMonths(-1).AddDays(-1), DateTime.Now.AddDays(1));

                // Today will be the last in the list
                HistoricDataToday = historicDataList.Last();
                HistoricDataToday.Price = summary.PriceString.NumericValue; // Use latest price for todays data.
                HistoricDataToday.PeriodType = "D";

                // Last Week
                findDate = GetMondayIfWeekend(DateTime.Now.AddDays(-7).Date);
                HistoricDataWeekAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.AddDays(1));
                if (HistoricDataWeekAgo != null)
                    HistoricDataWeekAgo.PeriodType = "W";

                // Last Month (really 31 days ago)
                findDate = GetMondayIfWeekend(DateTime.Now.AddDays(-31).Date);
                HistoricDataMonthAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.Date.AddDays(1) || x.PriceDate.Date == findDate.Date.AddDays(2) || x.PriceDate.Date == findDate.Date.AddDays(3));
            }
            else
            {
                HistoricDataToday = new HistoricPriceData() { PeriodType = "D", Price = summary.PriceString.NumericValue, PriceDate = DateTime.Now.Date, Ticker = ticker, Volume = "N/A" };
            }

            /////// Get price history for a year ago to determine long trend
            if (get1Year)
            {
                historicDataList = await GetHistoricalDataForDateRange(ticker, DateTime.Now.AddYears(-1).AddDays(-1), DateTime.Now.AddYears(-1).AddDays(4));
                // Last Year
                findDate = GetMondayIfWeekend(DateTime.Now.AddYears(-1).Date);
                HistoricDataYearAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.Date.AddDays(1));
                if (HistoricDataYearAgo != null)
                    HistoricDataYearAgo.PeriodType = "Y";
            }


            /////// Get price history for 3 years ago to determine long trend
            if (get3Year)
            {
                historicDataList = await GetHistoricalDataForDateRange(ticker, DateTime.Now.AddYears(-3).AddDays(-1), DateTime.Now.AddYears(-3).AddDays(4));
                if (historicDataList.Count > 0)
                {
                    HistoricData3YearsAgo = historicDataList.First();
                    HistoricData3YearsAgo.PeriodType = "3Y";
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

        public async Task<List<HistoricPriceData>> GetHistoricalDataForDateRange(string ticker, DateTime beginDate, DateTime endDate)
        {
            string formattedDate = "";
            string html = "";

            Ticker = ticker;

            List<HistoricPriceData> historicDataList = new List<HistoricPriceData>();

            try
            {
                if (beginDate.DayOfWeek == DayOfWeek.Saturday)
                    beginDate = beginDate.AddDays(-1);
                if (beginDate.DayOfWeek == DayOfWeek.Sunday)
                    beginDate = beginDate.AddDays(-2);

                double totalDays = endDate.Subtract(beginDate).TotalDays;

                if (beginDate < DateTime.Today.AddYears(-2))
                {
                    totalDays = 200;
                    endDate = beginDate.AddDays(200);
                }

                html = await GetHistoryHtmlForTicker(Ticker, beginDate, endDate);
                if (html.Length < 4000) // try again
                {
                    Thread.Sleep(2000);
                    html = await GetHistoryHtmlForTicker(Ticker, beginDate, endDate);
                }

                for (int i = 0; i < totalDays; i++)
                {
                    formattedDate = beginDate.AddDays(i).ToString("MMM dd, yyyy");
                    int index = html.IndexOf(formattedDate);

                    if (index < 0)
                    {
                        formattedDate = beginDate.AddDays(i).ToString("MMM d, yyyy");
                        index = html.IndexOf(formattedDate);
                    }

                    if (index < 0)
                        continue;

                    string htmlForDate = html.Substring(index, 600);

                    var numbers = GetNumbersFromHtml(htmlForDate);

                    if (numbers.Count < 6)
                    {
                        numbers.Add("0");
                    }

                    if (numbers[5].IndexOf(",") < 0)
                        numbers[5] = "0";

                    HistoricPriceData historicData = new HistoricPriceData();

                    historicData.Ticker = Ticker;
                    historicData.PriceDate = beginDate.AddDays(i).Date;
                    historicData.Price = Convert.ToDecimal(numbers[3]);
                    historicData.Volume = numbers[5];

                    historicDataList.Add(historicData);

                    if (beginDate < DateTime.Today.AddYears(-1) && historicDataList.Count > 3)
                        break;
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Source + x.Message + "\n" + "GetHistoricalDataForDateRange() " + " " + ticker + " " + beginDate + " " + endDate + "\n" + html.Substring(0, html.Length / 10));
            }

            return historicDataList;
        }

        private async Task<string> GetHistoryHtmlForTicker(string ticker, DateTime beginDate, DateTime endDate)
        {
            // https://finance.yahoo.com/quote/?ticker?/history?period1=?period1?&period2=?period2?&interval=1d&filter=history&frequency=1d&includeAdjustedClose=true
            // 3 year https://finance.yahoo.com/quote/IMPP/history/?frequency=1wk&period1=1563818349&period2=1721671130

            string formattedUrl = _url.Replace("?ticker?", ticker);

            if (beginDate < DateTime.Today.AddYears(-2))
            {
                formattedUrl = formattedUrl.Replace("frequency=1d", "frequency=1wk");
            }

            Ticker = ticker;

            // Get seconds pasesed since 1/1/1970
            double beginEpoch = beginDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            double endEpoch = endDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            formattedUrl = formattedUrl.Replace("?period1?", Convert.ToInt32(beginEpoch).ToString());
            formattedUrl = formattedUrl.Replace("?period2?", Convert.ToInt32(endEpoch).ToString());

            string html = await GetHtml(formattedUrl);

            int historyLocation = html.IndexOf("data-testid=\"history-table");
            historyLocation = html.IndexOf("<table");



            html = html.Substring(historyLocation);

            return html;
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
            private string ticker = "";
            private string periodType = "";
            private DateTime priceDate;
            private decimal price = 0;
            private string volume = YahooFinance.NotApplicable;

            public string Ticker { get => ticker; set => ticker = value; }
            public string PeriodType { get => periodType; set => periodType = value; }
            public DateTime PriceDate { get => priceDate; set => priceDate = value; }
            public decimal Price { get => price; set => price = value; }
            public string Volume { get => volume; set => volume = value; }

            public override string ToString()
            {
                return string.Format(
                    $"{Ticker}, {PeriodType}, {PriceDate.ToString("MMM dd, yyyy")}, {Price}"
                ); ;
            }
        }
    }
}
