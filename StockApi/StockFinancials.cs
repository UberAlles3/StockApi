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
        public string TotalCashString = "";
        public string TotalDebtString = "";
        public string DebtEquityString = "";

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
                if (NotNumber(value))
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
                if (NotNumber(value))
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
                if (NotNumber(value))
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
            }
        }

        /////////////////// Cost of RevenueTTM
        public Color CostOfRevenueTtmColor = Color.LightSteelBlue;
        private string costOfRevenueTtmString = NotApplicable;
        private float costOfRevenueTtm = 0;
        public string CostOfRevenueTtmString
        {
            get => costOfRevenueTtmString;
            set
            {
                costOfRevenueTtmString = value;
                if (NotNumber(value))
                    CostOfRevenueTtm = 0;
                else
                    CostOfRevenueTtm = Convert.ToSingle(CostOfRevenueTtmString);
            }
        }
        public float CostOfRevenueTtm
        {
            get => costOfRevenueTtm;
            set
            {
                costOfRevenueTtm = value;
            }
        }

        /////////////////// Cost of Revenue2
        public Color CostOfRevenue2Color = Color.LightSteelBlue;
        private string costOfRevenue2String = NotApplicable;
        private float costOfRevenue2 = 0;
        public string CostOfRevenue2String
        {
            get => costOfRevenue2String;
            set
            {
                costOfRevenue2String = value;
                if (NotNumber(value))
                    CostOfRevenue2 = 0;
                else
                    CostOfRevenue2 = Convert.ToSingle(CostOfRevenue2String);
            }
        }
        public float CostOfRevenue2
        {
            get => costOfRevenue2;
            set
            {
                costOfRevenue2 = value;
            }
        }

        /////////////////// Cost of Revenue4
        public Color CostOfRevenue4Color = Color.LightSteelBlue;
        private string costOfRevenue4String = NotApplicable;
        private float costOfRevenue4 = 0;
        public string CostOfRevenue4String
        {
            get => costOfRevenue4String;
            set
            {
                costOfRevenue4String = value;
                if (NotNumber(value))
                    CostOfRevenue4 = 0;
                else
                    CostOfRevenue4 = Convert.ToSingle(CostOfRevenue4String);
            }
        }
        public float CostOfRevenue4
        {
            get => costOfRevenue4;
            set
            {
                costOfRevenue4 = value;
            }
        }


        /////////////////// Short Interest
        public Color ShortInterestColor = Color.LightSteelBlue;
        private string shortInterestString = NotApplicable;
        private float shortInterest = 0;
        public string ShortInterestString
        {
            get => shortInterestString;
            set
            {
                shortInterestString = value;
                if (NotNumber(value))
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
                CostOfRevenueTtmString = numbers[0];
            if (numbers.Count > 2)
                CostOfRevenue2String = numbers[2];
            if (numbers.Count > 4)
                CostOfRevenue4String = numbers[4];
            else if (numbers.Count > 3)
                CostOfRevenue4String = numbers[3];

            if (_revenueInMillions)
            {
                CostOfRevenueTtmString = CostOfRevenueTtmString.Substring(0, CostOfRevenueTtmString.Length - 4);
                CostOfRevenue2String = CostOfRevenue2String.Substring(0, CostOfRevenue2String.Length - 4);
                CostOfRevenue4String = CostOfRevenue4String.Substring(0, CostOfRevenue4String.Length - 4);
            }

            html = await GetHtmlForTicker(_statisticsUrl, Ticker);

            // Total Cash
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Cash").Term;
            TotalCashString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            // Total Debt
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Debt").Term;
            TotalDebtString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
            // Debt/Equity Ratio
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Debt/Equity").Term;
            DebtEquityString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);


            // Short Interest
            searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Short Interest").Term;
            shortInterestString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
            if (shortInterestString != YahooFinance.NotApplicable && shortInterestString.IndexOf("%") > 0)
                ShortInterestString = shortInterestString.Substring(0, shortInterestString.IndexOf("%"));
            else
                ShortInterestString = YahooFinance.NotApplicable;

            // Set Colors of Revenue (if revenue decreasing by 5% every 2 years, a problem
            if (RevenueTtm < (Revenue2 * .95))
                RevenueTtmColor = Color.Red;
            else if (RevenueTtm > (Revenue2 * 1.05))
                RevenueTtmColor = Color.Lime;
            else
                RevenueTtmColor = Color.LightSteelBlue;
            
            if (Revenue2 < (Revenue4 * .95))
                Revenue2Color = Color.Red;
            else if (Revenue2 > (Revenue4 * 1.05))
                Revenue2Color = Color.Lime;
            else
                Revenue2Color = Color.LightSteelBlue;

            // Set Colors of Cost of Revenue (if profit decreasing by 5% every 2 years, a problem
            if ((RevenueTtm - CostOfRevenueTtm) < ((Revenue2 - CostOfRevenue2) * .95))
                CostOfRevenueTtmColor = Color.Red;
            else if ((RevenueTtm - CostOfRevenueTtm) > ((Revenue2 - CostOfRevenue2) * 1.05))
                CostOfRevenueTtmColor = Color.Lime;
            else
                CostOfRevenueTtmColor = Color.LightSteelBlue;

            if ((Revenue2 - CostOfRevenue2) < ((Revenue4 - CostOfRevenue4) * .95))
                CostOfRevenue2Color = Color.Red;
            else if ((Revenue2 - CostOfRevenue2) > ((Revenue4 - CostOfRevenue4) * 1.05))
                CostOfRevenue2Color = Color.Lime;
            else
                CostOfRevenue2Color = Color.LightSteelBlue;

            return true;
        }
        private bool NotNumber(string value)
        {
            return value == YahooFinance.NotApplicable || value == "" || value == "--" || "-0123456789.,".IndexOf(value.Substring(0, 1)) < 0;
        }
    }
}
