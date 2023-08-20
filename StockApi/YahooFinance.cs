using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockApi
{
    public class YahooFinance
    {
        private static string _url = "https://finance.yahoo.com/quote/";
        public static string Html { get; set;}

        private static string _ticker;
        public static string Ticker
        {
            get { return _ticker; }
            set { _ticker = value.ToUpper(); }
        }


        public static async Task<string> GetHtmlForTicker(string ticker)
        {
            Ticker = ticker;

            HttpClient cl = new HttpClient();
            HttpResponseMessage hrm = await cl.GetAsync(_url + ticker);
            string Html = await hrm.Content.ReadAsStringAsync();
            return Html;
        }

        public class HtmlParser
        {
            public static void ExtractDataFromHtml(StockData stockData, string html)
            {
                stockData.Ticker = Ticker;
                stockData.Price = System.Convert.ToSingle(GetDataByClassName(html, "Fw(b) Fz(36px) Mb(-4px) D(ib)"));
                stockData.Beta = GetFloatValueFromHtml(html, "BETA_5Y-value", 1.0F);
                stockData.EarningsPerShare = GetFloatValueFromHtml(html, "EPS_RATIO-value", 0.0F);
                stockData.OneYearTargetPrice = GetFloatValueFromHtml(html, "ONE_YEAR_TARGET_PRICE-value", stockData.Price);
                stockData.FairValue = GetValueFromHtmlBySearchText(html, ">Overvalued<", "");
                if (stockData.FairValue == "")
                {
                    stockData.FairValue = GetValueFromHtmlBySearchText(html, ">Near Fair Value<", "");
                    if (stockData.FairValue == "")
                    {
                        stockData.FairValue = GetValueFromHtmlBySearchText(html, ">Undervalued<", "FV");
                    }
                    else
                        stockData.FairValue = "FV";
                }
                else
                    stockData.FairValue = "OV";

                //% Est. Return<
                string estReturn = GetValueFromHtmlBySearchText(html, "% Est. Return<", "0%");
                estReturn = estReturn.Substring(0, estReturn.IndexOf("%"));
                stockData.EstimatedReturn = System.Convert.ToSingle(estReturn);
            }

            private static float GetFloatValueFromHtml(string html, string data_test_name, float defaultValue)
            {
                string temp = GetValueFromHtml(html, data_test_name, defaultValue);
                if (temp != "N/A")
                    return System.Convert.ToSingle(temp);
                else
                    return defaultValue;
            }

            private static string GetDataByClassName(string html, string class_name)
            {
                int loc1 = html.IndexOf("class=\"" + class_name + "\"");
                if (loc1 == -1)
                {
                    throw new Exception("Unable to find class with name '" + class_name + "' in the web data.");
                }
                loc1 = html.IndexOf(">", loc1 + 1);
                int loc2 = html.IndexOf("<", loc1 + 1);
                string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
                return middle;
            }

            private static string GetValueFromHtml(string html, string data_test_name, float defaultValue)
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
        }
        public class StockData
        {
            public string Ticker = "";
            public float Price = 0;
            public float Beta = 1;
            public float EarningsPerShare = 0;
            public float OneYearTargetPrice = 0;
            public string FairValue = "";
            public float EstimatedReturn = 0;
        }
    }
}
