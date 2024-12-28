using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            Up
        }

        public HistoricData HistoricDataToday;
        public HistoricData HistoricDataWeekAgo;
        public HistoricData HistoricDataMonthAgo;
        public HistoricData HistoricDataYearAgo;
        public HistoricData HistoricData3YearsAgo;
        public TrendEnum WeekTrend = TrendEnum.Sideways;
        public TrendEnum MonthTrend = TrendEnum.Sideways;
        public TrendEnum YearTrend = TrendEnum.Sideways;
        public TrendEnum ThreeYearTrend = TrendEnum.Sideways;

        public async Task<List<StockHistory.HistoricData>> GetPriceHistoryForTodayWeekMonthYear(string ticker, StockSummary summary, bool get3Year, bool get1Year, bool getMonth, bool getWeek)
        {
            /////// Get price history, today, week ago, month ago to determine short trend
            List<StockHistory.HistoricData> historicDataList = await GetHistoricalDataForDateRange(ticker, DateTime.Now.AddMonths(-1).AddDays(-1), DateTime.Now.AddDays(1));

            // Today will be the last in the list
            HistoricDataToday = historicDataList.Last();
            HistoricDataToday.Price = summary.Price; // Use latest price for todays data.

            // Last Week
            DateTime findDate = GetMondayIfWeekend(DateTime.Now.AddDays(-7).Date);
            HistoricDataWeekAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.AddDays(1));

            // Last Month (really 31 days ago)
            findDate = GetMondayIfWeekend(DateTime.Now.AddDays(-31).Date);
            HistoricDataMonthAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.Date.AddDays(1) || x.PriceDate.Date == findDate.Date.AddDays(2) || x.PriceDate.Date == findDate.Date.AddDays(3));

            if (HistoricDataMonthAgo == null || historicDataList.Count == 0)
                HistoricDataMonthAgo = HistoricDataWeekAgo;

            /////// Get price history for a year ago to determine long trend
            historicDataList = await GetHistoricalDataForDateRange(ticker, DateTime.Now.AddYears(-1).AddDays(-1), DateTime.Now.AddYears(-1).AddDays(4));
            // Last Year
            findDate = GetMondayIfWeekend(DateTime.Now.AddYears(-1).Date);
            HistoricDataYearAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.Date.AddDays(1));

            if (HistoricDataYearAgo == null || historicDataList.Count == 0)
                HistoricDataYearAgo = HistoricDataMonthAgo;

            /////// Get price history for 3 years ago to determine long trend
            historicDataList = await GetHistoricalDataForDateRange(ticker, DateTime.Now.AddYears(-3).AddDays(-1), DateTime.Now.AddYears(-3).AddDays(4));
            if (historicDataList.Count > 0)
                HistoricData3YearsAgo = historicDataList.First();
            else
                HistoricData3YearsAgo = HistoricDataYearAgo;

            List<StockHistory.HistoricData> historicDisplayList = new List<StockHistory.HistoricData>();
            historicDisplayList.Add(HistoricDataToday);
            historicDisplayList.Add(HistoricDataWeekAgo);
            historicDisplayList.Add(HistoricDataMonthAgo);
            historicDisplayList.Add(HistoricDataYearAgo);
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

        public async Task<List<HistoricData>> GetHistoricalDataForDateRange(string ticker, DateTime beginDate, DateTime endDate)
        {
            string formattedDate = "";

            Ticker = ticker;

            List<HistoricData> historicDataList = new List<HistoricData>();

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

            string html = await GetHistoryHtmlForTicker(Ticker, beginDate, endDate);

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

                HistoricData historicData = new HistoricData();

                historicData.Ticker = Ticker;
                historicData.PriceDate = beginDate.AddDays(i).Date;
                historicData.Price = Convert.ToSingle(numbers[3]);
                historicData.Volume = numbers[5];

                historicDataList.Add(historicData);

                if (beginDate < DateTime.Today.AddYears(-1) && historicDataList.Count > 3)
                    break;
            }

            return historicDataList;
        }

        private async Task<string> GetHistoryHtmlForTicker(string ticker, DateTime beginDate, DateTime endDate)
        {
            // https://finance.yahoo.com/quote/?ticker?/history?period1=?period1?&period2=?period2?&interval=1d&filter=history&frequency=1d&includeAdjustedClose=true
            // 3 year https://finance.yahoo.com/quote/IMPP/history/?frequency=1wk&period1=1563818349&period2=1721671130

            string formattedUrl = _url.Replace("?ticker?", ticker);

            if(beginDate < DateTime.Today.AddYears(-2))
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
            if (HistoricDataToday.Price > HistoricData3YearsAgo.Price * 1.12F) // year
                ThreeYearTrend = TrendEnum.Up;
            else if (HistoricDataToday.Price < HistoricData3YearsAgo.Price * .88F) // year
                ThreeYearTrend = TrendEnum.Down;
            else
                ThreeYearTrend = TrendEnum.Sideways;

            if (HistoricDataToday.Price > HistoricDataYearAgo.Price * 1.08F) // year
                YearTrend = TrendEnum.Up;
            else if (HistoricDataToday.Price < HistoricDataYearAgo.Price * .92F) // year
                YearTrend = TrendEnum.Down;
            else
                YearTrend = TrendEnum.Sideways;

            if (HistoricDataToday.Price > HistoricDataMonthAgo.Price * 1.06F) // month
                MonthTrend = TrendEnum.Up;
            else if (HistoricDataToday.Price < HistoricDataMonthAgo.Price * .94F) // month
                MonthTrend = TrendEnum.Down;
            else
                MonthTrend = TrendEnum.Sideways;

            if (HistoricDataToday.Price > HistoricDataWeekAgo.Price * 1.03F) // week
                WeekTrend = TrendEnum.Up;
            else if (HistoricDataToday.Price < HistoricDataWeekAgo.Price * .97F) // week
                WeekTrend = TrendEnum.Down;
            else
                WeekTrend = TrendEnum.Sideways;
        }

        public class HistoricData
        {
            private string ticker = "";
            private DateTime priceDate;
            private float price = 0;
            private string volume = YahooFinance.NotApplicable;

            public string Ticker { get => ticker; set => ticker = value; }
            public DateTime PriceDate { get => priceDate; set => priceDate = value; }
            public float Price { get => price; set => price = value; }
            public string Volume { get => volume; set => volume = value; }

            public override string ToString()
            {
                return string.Format(
                    $"{Ticker}, {PriceDate.ToString("MMM dd, yyyy")}, {Price}"
                ); ;
            }
        }
    }
}
