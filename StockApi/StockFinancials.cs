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

        public bool   _revenueInMillions = false;
        public string CostOfRevenueTTM = "";
        public string CostOfRevenue2 = "";
        public string CostOfRevenue4 = "";
        public string TotalCash = "";
        public string TotalDebt = "";
        public string DebtEquity = "";

        ////////////////////////////////////////////
        ///                Properties
        ////////////////////////////////////////////
        /////////////////// RevenueTTM
        public Color RevenueTtmColor = Color.LightSteelBlue;
        private string revenueTtmString = NotApplicable;
        private float revenueTtm = 0;
        public string RevenueTtmString
        {
            get => revenueTtmString;
            set
            {
                revenueTtmString = value;
                if (IsNumber(value))
                    RevenueTtm = 0;
                else
                    RevenueTtm = Convert.ToSingle(RevenueTtmString);
            }
        }
        public float RevenueTtm
        {
            get => revenueTtm;
            set
            {
                revenueTtm = value;
                //if (revenueTtm > 8)
                //    RevenueTtmColor = Color.Red;
                //else if (RevenueTtm < 2)
                //    RevenueTtmColor = Color.Lime;
                //else
                //    RevenueTtmColor = Color.LightSteelBlue;
            }
        }

        /////////////////// Revenue2
        public Color Revenue2Color = Color.LightSteelBlue;
        private string revenue2String = NotApplicable;
        private float revenue2 = 0;
        public string Revenue2String
        {
            get => revenue2String;
            set
            {
                revenue2String = value;
                if (IsNumber(value))
                    Revenue2 = 0;
                else
                    Revenue2 = Convert.ToSingle(Revenue2String);
            }
        }
        public float Revenue2
        {
            get => revenue2;
            set
            {
                revenue2 = value;
                //if (revenue2 > 8)
                //    Revenue2Color = Color.Red;
                //else if (Revenue2 < 2)
                //    Revenue2Color = Color.Lime;
                //else
                //    Revenue2Color = Color.LightSteelBlue;
            }
        }

        /////////////////// Revenue4
        public Color Revenue4Color = Color.LightSteelBlue;
        private string revenue4String = NotApplicable;
        private float revenue4 = 0;
        public string Revenue4String
        {
            get => revenue4String;
            set
            {
                revenue4String = value;
                if (IsNumber(value))
                    Revenue4 = 0;
                else
                    Revenue4 = Convert.ToSingle(Revenue4String);
            }
        }
        public float Revenue4
        {
            get => revenue4;
            set
            {
                revenue4 = value;
                //if (revenue4 > 8)
                //    Revenue4Color = Color.Red;
                //else if (Revenue4 < 2)
                //    Revenue4Color = Color.Lime;
                //else
                //    Revenue4Color = Color.LightSteelBlue;
            }
        }



        public Color ShortInterestColor = Color.LightSteelBlue;
        private string shortInterestString = NotApplicable;
        private float shortInterest = 0;
        public string ShortInterestString
        {
            get => shortInterestString;
            set
            {
                shortInterestString = value;
                if (IsNumber(value))
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
                RevenueTtmString = numbers[0];
            if (numbers.Count > 2)
                Revenue2String = numbers[2];
            if (numbers.Count > 4)
                Revenue4String = numbers[4];
            else if (numbers.Count > 3)
                Revenue4String = numbers[3];

            _revenueInMillions = false; // reset
            if (RevenueTtmString.Length > 7)
            {
                _revenueInMillions = true;
                RevenueTtmString = RevenueTtmString.Substring(0, RevenueTtmString.Length - 4);
                Revenue2String = Revenue2String.Substring(0, Revenue2String.Length - 4);
                Revenue4String = Revenue4String.Substring(0, Revenue4String.Length - 4);
            }

            // Cost of Revenue History
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Cost of Revenue").Term;
            partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 300);
            numbers = GetNumbersFromHtml(partial);
            numbers = numbers.Select(x => x._TrimSuffix(".")).ToList();

            if (numbers.Count > 0)
                CostOfRevenueTTM = numbers[0];
            if (numbers.Count > 2)
                CostOfRevenue2 = numbers[2];
            if (numbers.Count > 4)
                CostOfRevenue4 = numbers[4];
            else if (numbers.Count > 3)
                CostOfRevenue4 = numbers[3];

            if (_revenueInMillions)
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
            // Debt/Equity Ratio
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Debt/Equity").Term;
            DebtEquity = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);


            // Short Interest
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Short Interest").Term;
            shortInterestString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
            if (shortInterestString != YahooFinance.NotApplicable && shortInterestString.IndexOf("%") > 0)
                ShortInterestString = shortInterestString.Substring(0, shortInterestString.IndexOf("%"));
            else
                ShortInterestString = YahooFinance.NotApplicable;


            return true;
        }
        private bool IsNumber(string value)
        {
            return ShortInterestString == YahooFinance.NotApplicable || ShortInterestString == "" || ShortInterestString == "--" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0;
        }
    }
}
