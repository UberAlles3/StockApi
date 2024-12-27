using System;
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
        //        private static readonly string _url = "https://finance.yahoo.com/quote/????p=???";
        private static readonly string _summaryUrl = "https://finance.yahoo.com/quote/???";
 
        public Color DividendColor = Color.LightSteelBlue;
        public Color EPSColor = Color.LightSteelBlue;
        public Color PriceBookColor = Color.LightSteelBlue;
        public Color ProfitMarginColor = Color.LightSteelBlue;
        public Color OneYearTargetColor = Color.LightSteelBlue;

        private string  companyName = "";
        private string  dividendString = NotApplicable;
        private float   dividend = 0;
        private string  earningsPerShareString = NotApplicable;
        private float   earningsPerShare = 0;
        private string  profitMarginString = NotApplicable;
        private float   profitMargin = 0;
        private string  priceBookString = NotApplicable;
        private float   priceBook = 0;
        private string  oneYearTargetPriceString = NotApplicable;
        private float   oneYearTargetPrice = 0;
        private float   price = 0;
        private string  volatilityString = NotApplicable;
        private float   volatility = 0;

        private List<SearchTerm> _searchTerms = new List<SearchTerm>();

        public string CompanyName { get => companyName; set => companyName = value; }

        public string DividendString
        {
            get => dividendString;
            set
            {
                dividendString = value.Replace("%", "");
                if (dividendString == YahooFinance.NotApplicable || dividendString == "" || dividendString == "--" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0)
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
                if (earningsPerShareString == YahooFinance.NotApplicable || earningsPerShareString == "" || earningsPerShareString == "--" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0)
                    EarningsPerShare = 0;
                else
                    try
                    {
                        EarningsPerShare = Convert.ToSingle(earningsPerShareString);
                    }
                    catch (Exception)
                    {
                        EarningsPerShare = 0;
                    }
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
                if (ProfitMarginString == YahooFinance.NotApplicable || ProfitMarginString == "" || ProfitMarginString == "--" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0)
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
        public async Task<string> GetHtmlForTicker(string url, string ticker)
        {
            Ticker = ticker;
            string formattedUrl = url.Replace("???", Ticker);
            return await GetHtml(formattedUrl);
        }

        public async Task<bool> GetSummaryData(string ticker, bool verbose = true)
        {
            Ticker = ticker;
            _searchTerms = ConfigurationManager.GetSection("SearchTokens") as List<SearchTerm>;

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
            string searchTerm = _searchTerms.Find(x => x.Name == "Price").Term;
            string price = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            Price = Convert.ToSingle(price);

            if (verbose == false)
                return true;

            // EPS
            searchTerm = _searchTerms.Find(x => x.Name == "EPS").Term;
            EarningsPerShareString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
            //EarningsPerShareString = GetFloatValueFromHtml(html, "EPS_RATIO-value", YahooFinance.NotApplicable);

            // Volatility
            searchTerm = _searchTerms.Find(x => x.Name == "Volatility").Term;
            VolatilityString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 3);

            // Dividend
            searchTerm = _searchTerms.Find(x => x.Name == "Dividend").Term;
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
            DividendString = dividend;

            // One year target
            searchTerm = _searchTerms.Find(x => x.Name == "One Year Target").Term;
            OneYearTargetPriceString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4).Trim();

            // Price / Book
            searchTerm = _searchTerms.Find(x => x.Name == "Price/Book").Term;
            PriceBookString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);

            //Profit Margin %
            searchTerm = _searchTerms.Find(x => x.Name == "Profit Margin").Term;
            string profitMarginString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            if (profitMarginString != YahooFinance.NotApplicable && profitMarginString.IndexOf("%") > 0)
                ProfitMarginString = profitMarginString.Substring(0, profitMarginString.IndexOf("%"));
            else
                ProfitMarginString = YahooFinance.NotApplicable;
            return true;
        }




    }
}
