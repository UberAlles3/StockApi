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

        public static async Task<string> GetHtmlForTicker(string ticker)
        {
            HttpClient cl = new HttpClient();
            HttpResponseMessage hrm = await cl.GetAsync(_url + ticker);
            string Html = await hrm.Content.ReadAsStringAsync();
            return Html;
        }

        public class HtmlParser
        {
            public static void ExtractDataFromHtml(StockData equitySummaryData, string html)
            {
                equitySummaryData.Beta = System.Convert.ToSingle(GetValueFromHtml(html, "BETA_5Y-value"));
                equitySummaryData.EarningsPerShare = System.Convert.ToSingle(GetValueFromHtml(html, "EPS_RATIO-value"));
                equitySummaryData.OneYearTargetPrice = System.Convert.ToSingle(GetValueFromHtml(html, "ONE_YEAR_TARGET_PRICE-value"));
                equitySummaryData.FairValue = GetValueFromHtmlBySearchText(html, ">Overvalued<");
                if (equitySummaryData.FairValue == "")
                {
                    equitySummaryData.FairValue = GetValueFromHtmlBySearchText(html, ">Near Fair Value<");
                    if (equitySummaryData.FairValue == "")
                    {
                        equitySummaryData.FairValue = GetValueFromHtmlBySearchText(html, ">Undervalued<");
                        if (equitySummaryData.FairValue != "")
                            equitySummaryData.FairValue = "UV";
                    }
                    else
                        equitySummaryData.FairValue = "FV";
                }
                else
                    equitySummaryData.FairValue = "OV";

                //% Est. Return<
                string estReturn = GetValueFromHtmlBySearchText(html, "% Est. Return<");
                if (estReturn != "")
                {
                    estReturn = estReturn.Substring(0, estReturn.IndexOf("%"));
                    equitySummaryData.EstimatedReturn = System.Convert.ToSingle(estReturn);
                }
            }

            private static string GetValueFromHtml(string html, string data_test_name)
            {
                int loc1 = 0;
                int loc2 = 0;

                loc1 = html.IndexOf("data-test=\"" + data_test_name + "\"");
                if (loc1 == -1)
                {
                    throw new Exception("Unable to find data with data test name '" + html + "' inside web data.");
                }

                loc1 = html.IndexOf(">", loc1 + 1);
                loc2 = html.IndexOf("<", loc1 + 1);

                string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
                return middle;
            }

            private static string GetValueFromHtmlBySearchText(string html, string searchText)
            {
                int loc1 = 0;
                int loc2 = 0;

                loc1 = html.IndexOf(searchText);
                if (loc1 == -1)
                {
                    return "";
                }

                loc1 = html.IndexOf(">", loc1 - 4);
                loc2 = html.IndexOf("<", loc1 + 1);

                string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
                return middle;
            }
        }
        public class StockData
        {
            public float Beta = 0;
            public float EarningsPerShare = 0;
            public float OneYearTargetPrice = 0;
            public string FairValue = "";
            public float EstimatedReturn = 0;
        }
    }
}
