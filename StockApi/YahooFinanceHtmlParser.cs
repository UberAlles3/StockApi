using System;
using System.Collections.Generic;
using System.Text;

namespace StockApi
{
    public class YahooFinanceHtmlParser
    {
        public static void ExtractDataFromHtml(StockData equitySummaryData, string web)
        {
            equitySummaryData.Beta = System.Convert.ToSingle(GetValueFromHtml(web, "BETA_5Y-value"));
            equitySummaryData.EarningsPerShare = System.Convert.ToSingle(GetValueFromHtml(web, "EPS_RATIO-value"));
        }

        private static string GetValueFromHtml(string web_data, string data_test_name)
        {
            int loc1 = 0;
            int loc2 = 0;

            loc1 = web_data.IndexOf("data-test=\"" + data_test_name + "\"");
            if (loc1 == -1)
            {
                throw new Exception("Unable to find data with data test name '" + web_data + "' inside web data.");
            }

            loc1 = web_data.IndexOf(">", loc1 + 1);
            loc2 = web_data.IndexOf("<", loc1 + 1);

            string middle = web_data.Substring(loc1 + 1, loc2 - loc1 - 1);
            return middle;
        }


    }
}
