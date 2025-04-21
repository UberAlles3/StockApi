using Drake.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace StockApi
{
    public class StockSummary : YahooFinance
    {
        public enum ValuationEnum
        {
            UnderValued,
            FairValue,
            OverValued
        }

        private static readonly string _summaryUrl = "https://finance.yahoo.com/quote/???";

        public Color DividendColor = Color.LightSteelBlue;
        public Color EPSColor = Color.LightSteelBlue;
        public Color PriceBookColor = Color.LightSteelBlue;
        public Color ProfitMarginColor = Color.LightSteelBlue;
        public Color OneYearTargetColor = Color.LightSteelBlue;
        public Color ForwardPEColor = Color.LightSteelBlue;
        public Color EarningsDateColor = Color.LightSteelBlue;

        private string companyName = "";
        public string CompanyOverview = "";
        public string Sector = "";
        public int AverageSectorPE = 20;
        public ValuationEnum Valuation = ValuationEnum.FairValue;
        public Dictionary<string, int> _sectors = new Dictionary<string, int>() { { "Technology", 35 }, { "Energy", 15 }, { "Materials", 25 }, { "Industrials", 26 }, { "Utilities", 21 }, { "Healthcare", 20 }, { "Real Estate", 36 }, { "Financial Services", 16 }, { "Communication Services", 21 }, { "Consumer Defensive", 24 } };

        public string CompanyName { get => companyName; set => companyName = value; }

        public StringSafeType<Decimal> DividendString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> EarningsPerShareString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> ProfitMarginString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> PriceBookString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> OneYearTargetPriceString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> PriceString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> VolatilityString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> YearsRangeLow = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> YearsRangeHigh = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> ForwardPEString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> CalculatedPEString = new StringSafeType<decimal>("--");
        public StringSafeType<DateTime> EarningsDateString = new StringSafeType<DateTime>("--");

        ////////////////////////////////////////////
        ///                Methods
        ////////////////////////////////////////////

        public async Task<bool> GetSummaryData(string ticker, bool verbose = true)
        {
            Ticker = ticker;

            string html = await GetHtmlForTicker(_summaryUrl, Ticker);

            if (html.Length < 3400) // try again
            {
                Thread.Sleep(1000);
                html = await GetHtmlForTicker(_summaryUrl, Ticker);
                Thread.Sleep(1000);
            }

            CompanyName = GetDataByTagName(html, "title", Ticker);
            CompanyName = CompanyName.Substring(0, CompanyName.IndexOf(")") + 1);
            CompanyName = HttpUtility.HtmlDecode(CompanyName);

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
                string range = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
                int idx = range.IndexOf("-");
                if (idx > 0)
                {
                    YearsRangeLow.StringValue = range.Substring(0, idx).Trim();
                    YearsRangeHigh.StringValue = range.Substring(idx + 1).Trim();
                }

                // Forward P/E
                searchTerm = SearchTerms.Find(x => x.Name == "Forward P/E").Term;
                ForwardPEString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);

                // Earnings Date
                searchTerm = SearchTerms.Find(x => x.Name == "Earnings Date").Term;
                EarningsDateString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 3);

                // Company Overview
                searchTerm = SearchTerms.Find(x => x.Name == "Company Overview").Term;
                string htmlSnippet = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 4500);
                htmlSnippet = HttpUtility.HtmlDecode(htmlSnippet);
                string[] parts = htmlSnippet.Split(">");
                string longest = parts.OrderByDescending(s => s.Length).First();

                int sectorIndex = parts.Select((s, i) => new { i, s }).Where(x => x.s.Contains("Sector<")).Select(t => t.i).First();
                sectorIndex -= 3;

                string[] words = (parts[sectorIndex] + " |").Split(" ");
                if (words.Length > 2)
                    this.Sector = words[0] + " " + words[1];
                else
                    this.Sector = (parts[sectorIndex] + " |").Split(" ")[0];

                // find average PE for Sector
                if (_sectors.ContainsKey(Sector))
                    AverageSectorPE = _sectors.First(x => x.Key == Sector).Value;
                else
                    AverageSectorPE = 20;

                CompanyOverview = longest._TrimSuffix("</");

            }
            catch (Exception x)
            {
                MessageBox.Show("GetSummaryData() " + " " + ticker + "\n" + x.Source + x.Message + "\n" + html.Substring(0, html.Length / 10));
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

            if (ForwardPEString.NumericValue > 50)
                ForwardPEColor = Color.Red;
            else if (ForwardPEString.NumericValue < 15)
                ForwardPEColor = Color.Lime;
            else
                ForwardPEColor = Color.LightSteelBlue;

            ForwardPEColor = Color.LightSteelBlue;
            if (EarningsDateString.IsDateTime)
            {
                TimeSpan? diff = EarningsDateString.DateTimeValue - DateTime.Now;
                if (diff.Value.TotalDays < 1)
                    EarningsDateColor = Color.Lime;
                else if (diff.Value.TotalDays < 2)
                    EarningsDateColor = Color.LightGreen;
            }

            return true;
        }
    }
}