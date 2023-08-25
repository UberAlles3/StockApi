﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace StockApi
{
    public class StockSummary : YahooFinance
    {
        private static readonly string _url = "https://finance.yahoo.com/quote/????p=???";

        private string companyName = "";
        private float price = 0;
        private string volatility = NotApplicable;
        private string earningsPerShare = NotApplicable;
        private string oneYearTargetPrice = NotApplicable;
        private FairValueEnum fairValue = FairValueEnum.FairValue;
        private string dividend = NotApplicable;
        private float estimatedReturn = 0;

        public string CompanyName { get => companyName; set => companyName = value; }
        public float Price { get => price; set => price = value; }
        public string Volatility { get => volatility; set => volatility = value; }
        public string EarningsPerShare
        {
            get => earningsPerShare;
            set
            {
                earningsPerShare = value;
                if (earningsPerShare == YahooFinance.NotApplicable || earningsPerShare == "")
                    YahooFinance.EPSColor = Color.LightSteelBlue;
                else
                {
                    if (Convert.ToSingle(earningsPerShare) < -1)
                        YahooFinance.EPSColor = Color.Red;
                    else if (Convert.ToSingle(earningsPerShare) > 1)
                        YahooFinance.EPSColor = Color.Lime;
                    else
                        YahooFinance.EPSColor = Color.LightSteelBlue;
                }
            }

        }
        public string OneYearTargetPrice
        {
            get => oneYearTargetPrice; set
            {
                oneYearTargetPrice = value;
                if (oneYearTargetPrice == YahooFinance.NotApplicable || oneYearTargetPrice == "")
                    YahooFinance.OneYearTargetColor = Color.LightSteelBlue;
                else
                {
                    if (Convert.ToSingle(oneYearTargetPrice) < price * .9)
                        YahooFinance.OneYearTargetColor = Color.Red;
                    else if (Convert.ToSingle(oneYearTargetPrice) > price * 1.1)
                        YahooFinance.OneYearTargetColor = Color.Lime;
                    else
                        YahooFinance.OneYearTargetColor = Color.LightSteelBlue;
                }
            }
        }

        public FairValueEnum FairValue
        {
            get => fairValue;
            set
            {
                fairValue = value;
                if (fairValue == FairValueEnum.Overvalued)
                    YahooFinance.FairValueColor = Color.Red;
                if (fairValue == FairValueEnum.FairValue)
                    YahooFinance.FairValueColor = Color.LightSteelBlue;
                if (fairValue == FairValueEnum.Undervalued)
                    YahooFinance.FairValueColor = Color.Lime;
            }
        }

        public string Dividend { get => dividend; set => dividend = value; }

        public float EstimatedReturn
        {
            get => estimatedReturn;
            set
            {
                estimatedReturn = value;
                if (estimatedReturn < -2)
                    YahooFinance.EstReturnColor = Color.Red;
                else if (estimatedReturn > 2)
                    YahooFinance.EstReturnColor = Color.Lime;
                else
                    YahooFinance.EstReturnColor = Color.LightSteelBlue;
            }
        }

        ////////////////////////////////////////////
        ///                Methods
        ////////////////////////////////////////////
        public async Task<string> GetHtmlForTicker(string ticker)
        {
            Ticker = ticker;
            string formattedUrl = _url.Replace("???", Ticker);
            return await YahooFinance.GetHtml(formattedUrl);
        }

        public async void GetSummaryData(string ticker)
        {
            Ticker = ticker;

            string html = await GetHtmlForTicker(Ticker);

            CompanyName = GetDataByTagName(html, "title", Ticker);
            CompanyName = CompanyName.Substring(0, CompanyName.IndexOf(")") + 1);
            Price = System.Convert.ToSingle(GetDataByClassName(html, "Fw(b) Fz(36px) Mb(-4px) D(ib)", "0.00"));
            Volatility = GetValueFromHtml(html, "BETA_5Y-value", YahooFinance.NotApplicable);
            string dividend = GetValueFromHtml(html, "DIVIDEND_AND_YIELD-value", YahooFinance.NotApplicable);
            if (!dividend.Contains(YahooFinance.NotApplicable) && dividend.IndexOf("(") > 1)
            {
                dividend = dividend.Substring(dividend.IndexOf("(") + 1);
                dividend = dividend.Substring(0, dividend.IndexOf(")") - 1);
            }
            else
                dividend = YahooFinance.NotApplicable;
            Dividend = dividend;

            EarningsPerShare = GetFloatValueFromHtml(html, "EPS_RATIO-value", YahooFinance.NotApplicable);
            OneYearTargetPrice = GetFloatValueFromHtml(html, "ONE_YEAR_TARGET_PRICE-value", Price.ToString());

            // Fair Value
            if (GetValueFromHtmlBySearchText(html, ">Overvalued<", "") != "")
            {
                FairValue = FairValueEnum.Overvalued;
            }
            else if (GetValueFromHtmlBySearchText(html, ">Near Fair Value<", "") != "")
            {
                FairValue = FairValueEnum.FairValue;
            }
            else if (GetValueFromHtmlBySearchText(html, ">Undervalued<", "") != "")
            {
                FairValue = FairValueEnum.Undervalued;
            }
            else
            {
                FairValue = FairValueEnum.FairValue;
            }

            //% Est. Return<
            string estReturn = GetValueFromHtmlBySearchText(html, "% Est. Return<", "0%");
            estReturn = estReturn.Substring(0, estReturn.IndexOf("%"));
            EstimatedReturn = System.Convert.ToSingle(estReturn);
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
    }
}
