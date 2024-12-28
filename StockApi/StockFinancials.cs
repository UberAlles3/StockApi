using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Drake.Extensions;

namespace StockApi 
{
    public class StockFinancials : YahooFinance
    {
        private static readonly string _statisticsUrl = "https://finance.yahoo.com/quote/???/key-statistics/";
        private static readonly string _financialsUrl = "https://finance.yahoo.com/quote/???/financials/";
        private List<SearchTerm> _searchTerms = new List<SearchTerm>();

        public Color ShortInterestColor = Color.LightSteelBlue;

        private string shortInterestString = NotApplicable;
        private float shortInterest = 0;

        public bool   RevenueInMillions = false;
        public string RevenueTTM = "";
        public string Revenue2 = "";
        public string Revenue4 = "";
        public string CostOfRevenueTTM = "";
        public string CostOfRevenue2 = "";
        public string CostOfRevenue4 = "";
        public string TotalCash = "";
        public string TotalDebt = "";
        public string CashDebtRatio = "";

        public string ShortInterestString
        {
            get => shortInterestString;
            set
            {
                shortInterestString = value;
                if (ShortInterestString == YahooFinance.NotApplicable || ShortInterestString == "" || ShortInterestString == "--" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0)
                    ShortInterest = 0;
                else
                    ShortInterest = Convert.ToSingle(ShortInterestString);
            }
        }

        public float ShortInterest
        {
            get => shortInterest;
            set
            {
                shortInterest = value;
                if (shortInterest > 8)
                    ShortInterestColor = Color.Red;
                else if (ShortInterest < 2)
                    ShortInterestColor = Color.Lime;
                else
                    ShortInterestColor = Color.LightSteelBlue;
            }
        }

        ////////////////////////////////////////////
        ///                Methods
        ////////////////////////////////////////////

        public async Task<bool> GetFinancialData(string ticker)
        {
            Ticker = ticker;

            string html = await GetHtmlForTicker(_financialsUrl, Ticker);

            // Revenue History
            string searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Revenue").Term;
            string partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 300);
            List<string> numbers = GetNumbersFromHtml(partial);
            numbers = numbers.Select(x => x._TrimSuffix(".")).ToList();

            if (numbers.Count > 0)
                RevenueTTM = numbers[0];
            if (numbers.Count > 2)
                Revenue2 = numbers[2];
            if (numbers.Count > 4)
                Revenue4 = numbers[4];
            else if (numbers.Count > 3)
                Revenue4 = numbers[3];

            RevenueInMillions = false; // reset
            if (RevenueTTM.Length > 7)
            {
                RevenueInMillions = true;
                RevenueTTM = RevenueTTM.Substring(0, RevenueTTM.Length - 4);
                Revenue2 = Revenue2.Substring(0, Revenue2.Length - 4);
                Revenue4 = Revenue4.Substring(0, Revenue4.Length - 4);
            }

            // Cost of Revenue History
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Cost of Revenue").Term;
            partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 300);
            numbers = GetNumbersFromHtml(partial);
            numbers = numbers.Select(x => RemoveAfter(x)).ToList();

            if (numbers.Count > 0)
                CostOfRevenueTTM = numbers[0];
            if (numbers.Count > 2)
                CostOfRevenue2 = numbers[2];
            if (numbers.Count > 4)
                CostOfRevenue4 = numbers[4];
            else if (numbers.Count > 3)
                CostOfRevenue4 = numbers[3];

            if (RevenueInMillions)
            {
                CostOfRevenueTTM = CostOfRevenueTTM.Substring(0, CostOfRevenueTTM.Length - 4);
                CostOfRevenue2 = CostOfRevenue2.Substring(0, CostOfRevenue2.Length - 4);
                CostOfRevenue4 = CostOfRevenue4.Substring(0, CostOfRevenue4.Length - 4);
            }

            html = await GetHtmlForTicker(_statisticsUrl, Ticker);

            // Total Cash
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Cash").Term;
            TotalCash = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            // Total Debt
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Debt").Term;
            TotalDebt = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            // Cash Debt Ratio
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Cash Debt Ratio").Term;
            CashDebtRatio = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);


            // Short Interest
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Short Interest").Term;
            shortInterestString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
            if (shortInterestString != YahooFinance.NotApplicable && shortInterestString.IndexOf("%") > 0)
                ShortInterestString = shortInterestString.Substring(0, shortInterestString.IndexOf("%"));
            else
                ShortInterestString = YahooFinance.NotApplicable;


            return true;
        }
    }
}
