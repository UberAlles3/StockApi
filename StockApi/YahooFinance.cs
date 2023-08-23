using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Linq;

namespace StockApi
{
    public class YahooFinance // General purpose functions
    {
        public static string NotApplicable = "N/A";

        public enum FairValue
        {
            Overvalued,
            FairValue,
            Undervalued
        }

        public static Color FairValueColor = Color.White;
        public static Color EPSColor = Color.White;
        public static Color EstReturnColor = Color.White;
        public static Color OneYearTargetColor = Color.White;

        protected static async Task<string> GetHtml(string url)
        {
            HttpClient cl = new HttpClient();
            HttpResponseMessage hrm = await cl.GetAsync(url);
            string html = await hrm.Content.ReadAsStringAsync();
            return html;
        }

    }

    //********************************************************
    //                 STOCK SUMMARY CLASS
    //********************************************************
    public class StockSummary : YahooFinance
    {
        private static string _url = "https://finance.yahoo.com/quote/????p=???";

        private static string _ticker;
        public static string Ticker
        {
            get { return _ticker; }
            set { _ticker = value.ToUpper(); }
        }

        public static async Task<string> GetHtmlForTicker(string ticker)
        {
            Ticker = ticker;
            string formattedUrl = _url.Replace("???", Ticker);
            return await YahooFinance.GetHtml(formattedUrl);
        }

        public static void ExtractDataFromHtml(StockData stockData, string html)
        {
            stockData.Ticker = Ticker;
            stockData.CompanyName = GetDataByTagName(html, "title", Ticker);
            stockData.CompanyName = stockData.CompanyName.Substring(0, stockData.CompanyName.IndexOf(")") + 1);
            stockData.Price = System.Convert.ToSingle(GetDataByClassName(html, "Fw(b) Fz(36px) Mb(-4px) D(ib)", "0.00"));
            stockData.Volatility = GetValueFromHtml(html, "BETA_5Y-value", YahooFinance.NotApplicable);
            string dividend = GetValueFromHtml(html, "DIVIDEND_AND_YIELD-value", YahooFinance.NotApplicable);
            if (dividend != YahooFinance.NotApplicable && dividend.IndexOf("(") > 1)
            {
                dividend = dividend.Substring(dividend.IndexOf("(") + 1);
                dividend = dividend.Substring(0, dividend.IndexOf(")") -1);
            }
            stockData.Dividend = dividend;
            stockData.EarningsPerShare = GetFloatValueFromHtml(html, "EPS_RATIO-value", YahooFinance.NotApplicable);
            stockData.OneYearTargetPrice = GetFloatValueFromHtml(html, "ONE_YEAR_TARGET_PRICE-value", stockData.Price.ToString());

            // Fair Value
            if (GetValueFromHtmlBySearchText(html, ">Overvalued<", "") != "")
            {
                stockData.FairValue = FairValue.Overvalued;
            }
            else if (GetValueFromHtmlBySearchText(html, ">Near Fair Value<", "") != "")
            {
                stockData.FairValue = FairValue.FairValue;
            }
            else if (GetValueFromHtmlBySearchText(html, ">Undervalued<", "") != "")
            {
                stockData.FairValue = FairValue.Undervalued;
            }
            else
            {
                stockData.FairValue = FairValue.FairValue;
            }

            //% Est. Return<
            string estReturn = GetValueFromHtmlBySearchText(html, "% Est. Return<", "0%");
            estReturn = estReturn.Substring(0, estReturn.IndexOf("%"));
            stockData.EstimatedReturn = System.Convert.ToSingle(estReturn);
        }

        private static string GetFloatValueFromHtml(string html, string data_test_name, string defaultValue)
        {
            string temp = GetValueFromHtml(html, data_test_name, defaultValue);
            if (temp != YahooFinance.NotApplicable)
                return temp;
            else
                return defaultValue;
        }

        private static string GetDataByTagName(string html, string tagName, string defaultValue)
        {
            int loc1 = html.IndexOf("<" + tagName) + 1;
            int loc2 = html.IndexOf("</" + tagName, loc1 + 5); //</title
            if (loc1 == -1)
            {
                //throw new Exception("Unable to find class with name '" + class_name + "' in the web data.");
                return defaultValue;
            }
            string middle = html.Substring(loc1 + 1 + tagName.Length, loc2 - loc1 - 1);
            return middle;
        }

        private static string GetDataByClassName(string html, string class_name, string defaultValue)
        {
            int loc1 = html.IndexOf("class=\"" + class_name + "\"");
            if (loc1 == -1)
            {
                //throw new Exception("Unable to find class with name '" + class_name + "' in the web data.");
                return defaultValue;
            }
            loc1 = html.IndexOf(">", loc1 + 1);
            int loc2 = html.IndexOf("<", loc1 + 1);
            string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
            return middle;
        }

        private static string GetValueFromHtml(string html, string data_test_name, string defaultValue)
        {
            int loc1 = 0;
            int loc2 = 0;

            loc1 = html.IndexOf("data-test=\"" + data_test_name + "\"");
            if (loc1 == -1)
            {
                return defaultValue.ToString();
            }

            loc1 = html.IndexOf(">", loc1 + 1);
            loc2 = html.IndexOf("<", loc1 + 1);

            string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
            return middle;
        }

        private static string GetValueFromHtmlBySearchText(string html, string searchText, string defaultValue)
        {
            int loc1 = 0;
            int loc2 = 0;

            loc1 = html.IndexOf(searchText);
            if (loc1 == -1)
            {
                return defaultValue;
            }

            loc1 = html.IndexOf(">", loc1 - 4);
            loc2 = html.IndexOf("<", loc1 + 1);

            string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
            return middle;
        }
        public class StockData
        {
            private string ticker = "";
            private string companyName = "";
            private float price = 0;
            private string volatility = YahooFinance.NotApplicable;
            private string earningsPerShare = YahooFinance.NotApplicable;
            private string oneYearTargetPrice = YahooFinance.NotApplicable;
            private FairValue fairValue = YahooFinance.FairValue.FairValue;
            private string dividend = YahooFinance.NotApplicable;
            private float estimatedReturn = 0;

            public string Ticker { get => ticker; set => ticker = value; }
            public string CompanyName { get => companyName; set => companyName = value; }
            public float Price { get => price; set => price = value; }
            public string Volatility { get => volatility; set => volatility = value; } 
            public string EarningsPerShare { get => earningsPerShare; 
                set
                {
                    earningsPerShare = value;
                    if (earningsPerShare == YahooFinance.NotApplicable || earningsPerShare == "")
                        YahooFinance.EPSColor = Color.White;
                    else
                    {
                        if (Convert.ToSingle(earningsPerShare) < -1)
                            YahooFinance.EPSColor = Color.Red;
                        else if (Convert.ToSingle(earningsPerShare) > 1)
                            YahooFinance.EPSColor = Color.Lime;
                        else
                            YahooFinance.EPSColor = Color.White;
                    }
                }

            }
            public string OneYearTargetPrice { get => oneYearTargetPrice; set
                {
                    oneYearTargetPrice = value;
                    if (oneYearTargetPrice == YahooFinance.NotApplicable || oneYearTargetPrice == "")
                        YahooFinance.OneYearTargetColor = Color.White;
                    else
                    {
                        if (Convert.ToSingle(oneYearTargetPrice) < price * .9)
                            YahooFinance.OneYearTargetColor = Color.Red;
                        else if (Convert.ToSingle(oneYearTargetPrice) > price * 1.1)
                            YahooFinance.OneYearTargetColor = Color.Lime;
                        else
                            YahooFinance.OneYearTargetColor = Color.White;
                    }
                }
            }

            public FairValue FairValue { get => fairValue; 
                set 
                { 
                    fairValue = value;
                    if (fairValue == FairValue.Overvalued)
                        YahooFinance.FairValueColor = Color.Red;
                    if (fairValue == FairValue.FairValue)
                        YahooFinance.FairValueColor = Color.White;
                    if (fairValue == FairValue.Undervalued)
                        YahooFinance.FairValueColor = Color.Lime;
                }
            }
            public string Dividend { get => dividend; set => dividend = value; }

            public float EstimatedReturn { get => estimatedReturn;
                set
                {
                    estimatedReturn = value;
                    if (estimatedReturn < -2)
                        YahooFinance.EstReturnColor = Color.Red;
                    else if (estimatedReturn > 2)
                        YahooFinance.EstReturnColor = Color.Lime;
                    else
                        YahooFinance.EstReturnColor = Color.White;
                }
            }
        }
    }

    //********************************************************
    //                 STOCK HISTORY CLASS
    //********************************************************
    public class StockHistory : YahooFinance
    {
        private static readonly string _url = "https://finance.yahoo.com/quote/?ticker?/history?period1=?period1?&period2=?period2?&interval=1d&filter=history&frequency=1d&includeAdjustedClose=true";

        public enum TrendEnum
        {
            Up,
            Sideways,
            Down
        }

        public static HistoricData HistoricDataToday;
        public static HistoricData HistoricDataWeekAgo;
        public static HistoricData HistoricDataMonthAgo;
        public static HistoricData HistoricDataYearAgo;
        public static TrendEnum WeekTrend = TrendEnum.Sideways;
        public static TrendEnum MonthTrend = TrendEnum.Sideways;
        public static TrendEnum YearTrend = TrendEnum.Sideways;

        private static string _ticker;
        public static string Ticker
        {
            get { return _ticker; }
            set { _ticker = value.ToUpper(); }
        }

        public static async Task<List<StockHistory.HistoricData>> GetPriceHistoryForTodayWeekMonthYear(string ticker)
        {
            /////// Get price history, today, week ago, month ago to determine short trend
            List<StockHistory.HistoricData> historicDataList = await StockHistory.GetHistoricalDataForDateRange(ticker, DateTime.Now.AddMonths(-1), DateTime.Now);

            // Today will be the last in the list
            StockHistory.HistoricDataToday = historicDataList.Last();

            // Last Week
            DateTime findDate = GetMondayIfWeekend(DateTime.Now.AddDays(-7).Date);
            StockHistory.HistoricDataWeekAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.AddDays(1));

            // Last Month (really 31 days ago)
            findDate = GetMondayIfWeekend(DateTime.Now.AddDays(-31).Date);
            StockHistory.HistoricDataMonthAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.Date.AddDays(1));

            /////// Get price history for a year ago to determine long trend
            historicDataList = await StockHistory.GetHistoricalDataForDateRange(ticker, DateTime.Now.AddYears(-1), DateTime.Now.AddYears(-1).AddDays(2));
            // Last Year
            findDate = GetMondayIfWeekend(DateTime.Now.AddYears(-1).Date);
            StockHistory.HistoricDataYearAgo = historicDataList.Find(x => x.PriceDate.Date == findDate.Date || x.PriceDate.Date == findDate.Date.AddDays(1));

            List<StockHistory.HistoricData> historicDisplayList = new List<StockHistory.HistoricData>();
            historicDisplayList.Add(StockHistory.HistoricDataToday);
            historicDisplayList.Add(StockHistory.HistoricDataWeekAgo);
            historicDisplayList.Add(StockHistory.HistoricDataMonthAgo);
            historicDisplayList.Add(StockHistory.HistoricDataYearAgo);

            return historicDisplayList;
        }



        public static async Task<string> GetHistoryHtmlForTicker(string ticker, DateTime beginDate, DateTime endDate)
        {
            string formattedUrl = _url.Replace("?ticker?", ticker);

            Ticker = ticker;

            // Get seconds pasesed since 1/1/1970
            double beginEpoch = beginDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            double endEpoch = endDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            formattedUrl = formattedUrl.Replace("?period1?", Convert.ToInt32(beginEpoch).ToString());
            formattedUrl = formattedUrl.Replace("?period2?", Convert.ToInt32(endEpoch).ToString());

            string html = await YahooFinance.GetHtml(formattedUrl);

            int historyLocation = html.IndexOf("data-test=\"historical-prices\"");
            html = html.Substring(historyLocation);

            return html;
        }

        public static async Task<List<HistoricData>> GetHistoricalDataForDateRange(string ticker, DateTime beginDate, DateTime endDate)
        {
            string formattedDate = "";

            Ticker = ticker;

            List<HistoricData> historicDataList = new List<HistoricData>();

            if (beginDate.DayOfWeek == DayOfWeek.Saturday)
                beginDate = beginDate.AddDays(-1);
            if (beginDate.DayOfWeek == DayOfWeek.Sunday)
                beginDate = beginDate.AddDays(-2);

            string html = await StockHistory.GetHistoryHtmlForTicker(Ticker, beginDate, endDate);

            for (int i = 0; i < endDate.Subtract(beginDate).TotalDays; i++)
            {
                formattedDate = beginDate.AddDays(i).ToString("MMM dd, yyyy");
                int index = html.IndexOf(formattedDate);

                if (index < 0)
                    continue;

                string htmlForDate = html.Substring(index, 480);
                htmlForDate = htmlForDate.Substring(htmlForDate.IndexOf("<span>"));

                var items = new List<string>();
                foreach (Match match in Regex.Matches(htmlForDate, "span>(.*?)</span"))
                    items.Add(match.Groups[1].Value);

                if (items.Count < 5)
                    continue;

                HistoricData historicData = new HistoricData();

                historicData.Ticker = Ticker;
                historicData.PriceDate = beginDate.AddDays(i).Date;
                historicData.Price = Convert.ToSingle(items[3]);
                historicData.Volume = items[5];

                historicDataList.Add(historicData);
            }

            return historicDataList;
        }

        public static DateTime GetMondayIfWeekend(DateTime theDate)
        {
            if (theDate.DayOfWeek == DayOfWeek.Saturday)
                theDate = theDate.AddDays(2); // return Monday

            if (theDate.DayOfWeek == DayOfWeek.Sunday)
            {
                theDate = theDate.AddDays(1); // return Monday
            }

            return theDate;
        }

        public static void SetTrends()
        {
            if (HistoricDataToday.Price > HistoricDataYearAgo.Price * 1.1) // year
                YearTrend = TrendEnum.Up;
            if (HistoricDataToday.Price < HistoricDataYearAgo.Price * .9F) // year
                YearTrend = TrendEnum.Down;
            else
                YearTrend = TrendEnum.Sideways;

            if (HistoricDataToday.Price > HistoricDataMonthAgo.Price * 1.1) // year
                MonthTrend = TrendEnum.Up;
            if (HistoricDataToday.Price < HistoricDataMonthAgo.Price * .9F) // year
                MonthTrend = TrendEnum.Down;
            else
                MonthTrend = TrendEnum.Sideways;

            if (HistoricDataToday.Price > HistoricDataWeekAgo.Price * 1.1) // year
                WeekTrend = TrendEnum.Up;
            if (HistoricDataToday.Price < HistoricDataWeekAgo.Price * .9F) // year
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

