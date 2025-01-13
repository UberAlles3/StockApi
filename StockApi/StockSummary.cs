using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using Drake.Extensions;

namespace StockApi
{
    public class StockSummary : YahooFinance
    {
        //        private static readonly string _url = "https://finance.yahoo.com/quote/????p=???";
        private static readonly string _summaryUrl = "https://finance.yahoo.com/quote/???";
 
        public Color DividendColor = Color.LightSteelBlue;
        public Color EPSColor = Color.LightSteelBlue;
        public Color PriceBookColor = Color.LightSteelBlue;
        public Color ProfitMarginColor = Color.LightSteelBlue;
        public Color OneYearTargetColor = Color.LightSteelBlue;

        private string  companyName = "";
        private string  priceBookString = NotApplicable;
        private float   priceBook = 0;
        private string  oneYearTargetPriceString = NotApplicable;
        private float   oneYearTargetPrice = 0;
        private float   price = 0;
        private string  volatilityString = NotApplicable;
        private float   volatility = 0;
        public string  CompanyOverview= "";

        public string CompanyName { get => companyName; set => companyName = value; }

        public StringSafeNumeric<Decimal> DividendString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> EarningsPerShareString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> ProfitMarginString = new StringSafeNumeric<decimal>("--");

        public string PriceBookString
        {
            get => priceBookString;
            set
            {
                priceBookString = value;
                if (priceBookString == YahooFinance.NotApplicable || priceBookString == "" || priceBookString == "--" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0)
                    PriceBook = 0;
                else
                    try
                    {
                        PriceBook = Convert.ToSingle(priceBookString);
                    }
                    catch (Exception)
                    {
                        PriceBook = 0;
                    }
            }
        }

        public float PriceBook
        {
            get => priceBook;
            set
            {
                priceBook = value;
                if (priceBook > 5)
                    PriceBookColor = Color.Red;
                else if (priceBook < 1)
                    PriceBookColor = Color.Lime;
                else
                    PriceBookColor = Color.LightSteelBlue;
            }
        }

        public string OneYearTargetPriceString
        {
            get => oneYearTargetPriceString;
            set
            {
                oneYearTargetPriceString = value;
                if (oneYearTargetPriceString == YahooFinance.NotApplicable || oneYearTargetPriceString == "" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0)
                    OneYearTargetPrice = Price;
                else
                    try
                    {
                        OneYearTargetPrice = Convert.ToSingle(oneYearTargetPriceString);
                    }
                    catch (Exception)
                    {
                        OneYearTargetPrice = Price;
                    }
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
                if (volatilityString == YahooFinance.NotApplicable || volatilityString == "" || volatilityString == "--" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0)
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
 
        public async Task<bool> GetSummaryData(string ticker, bool verbose = true)
        {
            Ticker = ticker;

            string html = await GetHtmlForTicker(_summaryUrl, Ticker);

            //html = Regex.Replace(html, @"[^\u0020-\u007e]", "");

            CompanyName = GetDataByTagName(html, "title", Ticker);
            CompanyName = CompanyName.Substring(0, CompanyName.IndexOf(")") + 1);

            if (CompanyName == "")
            {
                CompanyName = "Not Found";
                return false;
            }

            // Price
            string searchTerm = SearchTerms.Find(x => x.Name == "Price").Term;
            string price = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 1);
            Price = Convert.ToSingle(price);

            if (verbose == false)
                return true;

            // EPS
            searchTerm = SearchTerms.Find(x => x.Name == "EPS").Term;
            EarningsPerShareString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
            //EarningsPerShareString = GetFloatValueFromHtml(html, "EPS_RATIO-value", YahooFinance.NotApplicable);

            // Volatility
            searchTerm = SearchTerms.Find(x => x.Name == "Volatility").Term;
            VolatilityString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 3);

            // Dividend
            searchTerm = SearchTerms.Find(x => x.Name == "Dividend").Term;
            string dividend = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 3);
            if (!dividend.Contains(YahooFinance.NotApplicable) && dividend.IndexOf("(") > 1)
            {
                dividend = dividend.Substring(dividend.IndexOf("(") + 1);
                dividend = dividend.Substring(0, dividend.IndexOf(")") - 1);
            }
            else
            {
                searchTerm = "Yield";
                dividend = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            }
            DividendString.StringValue = dividend;

            // One year target
            searchTerm = SearchTerms.Find(x => x.Name == "One Year Target").Term;
            OneYearTargetPriceString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4).Trim();

            // Price / Book
            searchTerm = SearchTerms.Find(x => x.Name == "Price/Book").Term;
            PriceBookString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);

            //Profit Margin %
            searchTerm = SearchTerms.Find(x => x.Name == "Profit Margin").Term;
            string profitMarginString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            if (profitMarginString != YahooFinance.NotApplicable && profitMarginString.IndexOf("%") > 0)
                ProfitMarginString.StringValue = profitMarginString.Substring(0, profitMarginString.IndexOf("%"));
            else
                ProfitMarginString.StringValue = YahooFinance.NotApplicable;

            // Company Overview
            searchTerm = SearchTerms.Find(x => x.Name == "Company Overview").Term;
            string htmlSnippet = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 4000);
            string[] parts = htmlSnippet.Split(">");
            string longest = parts.OrderByDescending(s => s.Length).First();
            CompanyOverview = longest._TrimSuffix("</");

            ////////////////////
            /// Set Colors
            ////////////////////
            DividendColor = Color.LightSteelBlue;
            if (DividendString.NumericValue > 1)
                DividendColor = Color.Lime;
            EPSColor = Color.LightSteelBlue;
            if (EarningsPerShareString.NumericValue < -1)
                EPSColor = Color.Red;
            else if (EarningsPerShareString.NumericValue > 1)
                EPSColor = Color.Lime;
            if (ProfitMarginString.NumericValue < -2)
                ProfitMarginColor = Color.Red;
            else if (ProfitMarginString.NumericValue > 2)
                ProfitMarginColor = Color.Lime;
            else
                ProfitMarginColor = Color.LightSteelBlue;

            return true;
        }




    }
}
