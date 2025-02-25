using Drake.Extensions;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private string companyName = "";
        public string CompanyOverview = "";

        public string CompanyName { get => companyName; set => companyName = value; }

        public StringSafeNumeric<Decimal> DividendString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> EarningsPerShareString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> ProfitMarginString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> PriceBookString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> OneYearTargetPriceString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> PriceString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> VolatilityString = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> YearsRangeLow = new StringSafeNumeric<decimal>("--");
        public StringSafeNumeric<Decimal> YearsRangeHigh = new StringSafeNumeric<decimal>("--");

        ////////////////////////////////////////////
        ///                Methods
        ////////////////////////////////////////////

        public async Task<bool> GetSummaryData(string ticker, bool verbose = true)
        {
            Ticker = ticker;

            string html = await GetHtmlForTicker(_summaryUrl, Ticker);

            if (html.Length < 4000) // try again
            {
                Thread.Sleep(2000);
                html = await GetHtmlForTicker(_summaryUrl, Ticker);
            }

            //html = Regex.Replace(html, @"[^\u0020-\u007e]", "");

            CompanyName = GetDataByTagName(html, "title", Ticker);
            CompanyName = CompanyName.Substring(0, CompanyName.IndexOf(")") + 1);

            if (CompanyName == "")
            {
                CompanyName = "Not Found";
                return false;
            }

            try
            {
                // Price
                string searchTerm = SearchTerms.Find(x => x.Name == "Price").Term;
                PriceString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 1);

                if (verbose == false)
                    return true;

                // EPS
                searchTerm = SearchTerms.Find(x => x.Name == "EPS").Term;
                EarningsPerShareString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);

                // Volatility
                searchTerm = SearchTerms.Find(x => x.Name == "Volatility").Term;
                VolatilityString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 3);

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
                OneYearTargetPriceString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4).Trim();

                // Price / Book
                searchTerm = SearchTerms.Find(x => x.Name == "Price/Book").Term;
                PriceBookString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);

                //Profit Margin %
                searchTerm = SearchTerms.Find(x => x.Name == "Profit Margin").Term;
                string profitMarginString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
                if (profitMarginString != YahooFinance.NotApplicable && profitMarginString.IndexOf("%") > 0)
                    ProfitMarginString.StringValue = profitMarginString.Substring(0, profitMarginString.IndexOf("%"));
                else
                    ProfitMarginString.StringValue = YahooFinance.NotApplicable;

                // 52 Week Range
                searchTerm = SearchTerms.Find(x => x.Name == "52 Week Range").Term;
                string range  = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
                int idx = range.IndexOf("-");
                if(idx > 0)
                {
                    YearsRangeLow.StringValue = range.Substring(0, idx).Trim();
                    YearsRangeHigh.StringValue = range.Substring(idx+1).Trim();
                }

                // Company Overview
                searchTerm = SearchTerms.Find(x => x.Name == "Company Overview").Term;
                string htmlSnippet = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 4000);
                string[] parts = htmlSnippet.Split(">");
                string longest = parts.OrderByDescending(s => s.Length).First();
                CompanyOverview = longest._TrimSuffix("</");

            }
            catch (Exception x)
            {
                MessageBox.Show(x.Source + x.Message + "\n" + "GetSummaryData() " + " " + ticker + "\n" + html.Substring(0, 1000));
            }

            //*******************
            //    Set Colors
            //*******************
            DividendColor = Color.LightSteelBlue;
            if (DividendString.NumericValue > 1.5M)
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
            if (PriceBookString.NumericValue > 5)
                PriceBookColor = Color.Red;
            else if (PriceBookString.NumericValue < 1)
                PriceBookColor = Color.Lime;
            else
                PriceBookColor = Color.LightSteelBlue;
            OneYearTargetColor = Color.LightSteelBlue;
            if (OneYearTargetPriceString.NumericValue < PriceString.NumericValue * .9M)
                OneYearTargetColor = Color.Red;
            else if (OneYearTargetPriceString.NumericValue > PriceString.NumericValue * 1.1M)
                OneYearTargetColor = Color.Lime;

            return true;
        }
    }
}
