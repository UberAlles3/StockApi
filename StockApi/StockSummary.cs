﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StockApi
{
    public class StockSummary : YahooFinance
    {
        private static readonly string _url = "https://finance.yahoo.com/quote/????p=???";

        public enum FairValueEnum
        {
            Overvalued,
            FairValue,
            Undervalued
        }

        public Color DividendColor = Color.LightSteelBlue;
        public Color EPSColor = Color.LightSteelBlue;
        public Color FairValueColor = Color.LightSteelBlue;
        public Color ProfitMarginColor = Color.LightSteelBlue;
        public Color OneYearTargetColor = Color.LightSteelBlue;

        private string  companyName = "";
        private string  dividendString = NotApplicable;
        private float   dividend = 0;
        private string  earningsPerShareString = NotApplicable;
        private float   earningsPerShare = 0;
        private string  profitMarginString = NotApplicable;
        private float   profitMargin = 0;
        private FairValueEnum fairValue = FairValueEnum.FairValue;
        private string  oneYearTargetPriceString = NotApplicable;
        private float   oneYearTargetPrice = 0;
        private float   price = 0;
        private string  volatilityString = NotApplicable;
        private float   volatility = 0;

        private List<string> SearchTerms = new List<string>();

        public string CompanyName { get => companyName; set => companyName = value; }

        public string DividendString
        {
            get => dividendString;
            set
            {
                dividendString = value;
                if (dividendString == YahooFinance.NotApplicable || dividendString == "")
                    Dividend = 0;
                else
                    Dividend = Convert.ToSingle(dividendString);
            }
        }

        public float Dividend
        {
            get => dividend;
            set
            {
                dividend = value;
                DividendColor = Color.LightSteelBlue;
                if (dividend > 1)
                    DividendColor = Color.Lime;
            }
        }

        public string EarningsPerShareString
        {
            get => earningsPerShareString;
            set
            {
                earningsPerShareString = value;
                if (earningsPerShareString == YahooFinance.NotApplicable || earningsPerShareString == "")
                    EarningsPerShare = 0;
                else
                    EarningsPerShare = Convert.ToSingle(earningsPerShareString);
            }
        }

        public float EarningsPerShare
        {
            get => earningsPerShare;
            set
            {
                earningsPerShare = value;
                EPSColor = Color.LightSteelBlue;
                if (earningsPerShare < -1)
                    EPSColor = Color.Red;
                else if (earningsPerShare > 1)
                    EPSColor = Color.Lime;
            }
        }

        public string ProfitMarginString
        {
            get => profitMarginString;
            set
            {
                profitMarginString = value;
                if (ProfitMarginString == YahooFinance.NotApplicable || ProfitMarginString == "")
                    ProfitMargin = 0;
                else
                    ProfitMargin = Convert.ToSingle(ProfitMarginString);
            }
        }

        public float ProfitMargin
        {
            get => profitMargin;
            set
            {
                profitMargin = value;
                if (profitMargin < -2)
                    ProfitMarginColor = Color.Red;
                else if (ProfitMargin > 2)
                    ProfitMarginColor = Color.Lime;
                else
                    ProfitMarginColor = Color.LightSteelBlue;
            }
        }

        public FairValueEnum FairValue
        {
            get => fairValue;
            set
            {
                fairValue = value;
                if (fairValue == FairValueEnum.Overvalued)
                    FairValueColor = Color.Red;
                if (fairValue == FairValueEnum.FairValue)
                    FairValueColor = Color.LightSteelBlue;
                if (fairValue == FairValueEnum.Undervalued)
                    FairValueColor = Color.Lime;
            }
        }

        public string OneYearTargetPriceString
        {
            get => oneYearTargetPriceString;
            set
            {
                oneYearTargetPriceString = value;
                if (oneYearTargetPriceString == YahooFinance.NotApplicable || oneYearTargetPriceString == "")
                    OneYearTargetPrice = Price;
                else
                    OneYearTargetPrice = Convert.ToSingle(oneYearTargetPriceString);
            }
        }

        public float OneYearTargetPrice
        {
            get => oneYearTargetPrice;
            set
            {
                oneYearTargetPrice = value;
                OneYearTargetColor = Color.LightSteelBlue;
                if (oneYearTargetPrice < price * .9)
                    OneYearTargetColor = Color.Red;
                else if (oneYearTargetPrice > price * 1.1)
                    OneYearTargetColor = Color.Lime;
            }
        }

        public float Price { get => price; set => price = value; }

        public string VolatilityString
        {
            get => volatilityString;
            set
            {
                volatilityString = value;
                if (volatilityString == YahooFinance.NotApplicable || volatilityString == "")
                    Volatility = 1;
                else
                    Volatility = Convert.ToSingle(volatilityString);
            }
        }

        public float Volatility
        {
            get => volatility;
            set { volatility = value; }
        }


        ////////////////////////////////////////////
        ///                Methods
        ////////////////////////////////////////////
        public async Task<string> GetHtmlForTicker(string ticker)
        {
            Ticker = ticker;
            string formattedUrl = _url.Replace("???", Ticker);
            return await GetHtml(formattedUrl);
        }

        public async Task<bool> GetSummaryData(string ticker)
        {
            Ticker = ticker;
            List<SearchTerm> searchTerms = ConfigurationManager.GetSection("SearchTokens") as List<SearchTerm>;

            string html = await GetHtmlForTicker(Ticker);

            //html = Regex.Replace(html, @"[^\u0020-\u007e]", "");

            CompanyName = GetDataByTagName(html, "title", Ticker);
            CompanyName = CompanyName.Substring(0, CompanyName.IndexOf(")") + 1);

            // Price
            string searchTerm = searchTerms.Find(x => x.Name == "Price").Term;
            string price = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            Price = Convert.ToSingle(price);

            // EPS
            searchTerm = searchTerms.Find(x => x.Name == "EPS").Term;
            EarningsPerShareString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 3);
            //EarningsPerShareString = GetFloatValueFromHtml(html, "EPS_RATIO-value", YahooFinance.NotApplicable);

            // Volatility
            searchTerm = searchTerms.Find(x => x.Name == "Volatility").Term;
            VolatilityString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);

            // Dividend
            searchTerm = searchTerms.Find(x => x.Name == "Dividend").Term;
            string dividend = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            if (!dividend.Contains(YahooFinance.NotApplicable) && dividend.IndexOf("(") > 1)
            {
                dividend = dividend.Substring(dividend.IndexOf("(") + 1);
                dividend = dividend.Substring(0, dividend.IndexOf(")") - 1);
            }
            else
                dividend = YahooFinance.NotApplicable;
            DividendString = dividend;

            // One year target
            searchTerm = searchTerms.Find(x => x.Name == "One Year Target").Term;
            OneYearTargetPriceString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 3);

            // Fair Value
            FairValue = FairValueEnum.FairValue;
            if (GetValueFromHtmlBySearchText(html, ">Overvalued<", "") != "")
            {
                FairValue = FairValueEnum.Overvalued;
            }
            else if (GetValueFromHtmlBySearchText(html, ">Undervalued<", "") != "")
            {
                FairValue = FairValueEnum.Undervalued;
            }

            //Profit Margin %
            searchTerm = searchTerms.Find(x => x.Name == "Profit Margin").Term;
            string profitMarginString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            ProfitMarginString = profitMarginString.Substring(0, profitMarginString.IndexOf("%"));
            //string estReturn = GetValueFromHtmlBySearchText(html, "% Est. Return<", "0%");
            //estReturn = estReturn.Substring(0, estReturn.IndexOf("%"));
            //ProfitMarginString = estReturn;

            return true;
        }


        ////////////////////////////
        ///    Parsing methods
        ////////////////////////////
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
        private static string GetValueFromHtmlBySearchTerm(string html, string searchText, string defaultValue, int tagPosition)
        {
            int loc1 = 0;
            int loc2 = 0;

            loc1 = html.IndexOf(searchText);
            if (loc1 == -1)
            {
                return defaultValue;
            }

            string htmlSnippet = html.Substring(loc1 + 1, 200);
            string[] parts = htmlSnippet.Split(">");

            loc2 = parts[tagPosition].IndexOf("<");

            string middle = parts[tagPosition].Substring(0, loc2);
            return middle;
        }

    }
}
