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
                        else if (Convert.ToSingle(earningsPerShare) < 1)
                            YahooFinance.EPSColor = Color.White;
                        else if (Convert.ToSingle(earningsPerShare) > 1)
                            YahooFinance.EPSColor = Color.Red;
                        else
                            YahooFinance.EPSColor = Color.White;
                    }
                }

            }
            public string OneYearTargetPrice { get => oneYearTargetPrice; set => oneYearTargetPrice = value; }
            public FairValue FairValue { get => fairValue; 
                set 
                { 
                    fairValue = value;
                    if (fairValue == FairValue.Overvalued)
                        YahooFinance.FairValueColor = Color.Red;
                    if (fairValue == FairValue.FairValue)
                        YahooFinance.FairValueColor = Color.White;
                    if (fairValue == FairValue.Undervalued)
                        YahooFinance.FairValueColor = Color.Green;
                }
            }
            public float EstimatedReturn { get => estimatedReturn;
                set
                {
                    estimatedReturn = value;
                    if (estimatedReturn < -2)
                        YahooFinance.EstReturnColor = Color.Red;
                    else if (estimatedReturn > 2)
                        YahooFinance.EstReturnColor = Color.Green;
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

        public static HistoricData HistoricDataToday;
        public static HistoricData HistoricDataWeekAgo;
        public static HistoricData HistoricDataMonthAgo;
        public static HistoricData HistoricDataYearAgo;
        public static bool WeekTrend = false;
        public static bool MonthTrend = false;
        public static bool YearTrend = false;

        private static string _ticker;
        public static string Ticker
        {
            get { return _ticker; }
            set { _ticker = value.ToUpper(); }
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

        public static void SetTrends()
        {
            if (HistoricDataToday.Price < HistoricDataYearAgo.Price) // year
                YearTrend = false;
            else
                YearTrend = true;

            if (HistoricDataToday.Price < HistoricDataMonthAgo.Price) // month
                MonthTrend = false;
            else
                MonthTrend = true;

            if (HistoricDataToday.Price < HistoricDataWeekAgo.Price) // week
                WeekTrend = false;
            else
                WeekTrend = true;
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

