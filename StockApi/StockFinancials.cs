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
        public float ProfitTTM = 0;
        public float Profit2YearsAgo = 0;
        public float Profit4YearsAgo = 0;

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
                {
                    RevenueTtm = Convert.ToSingle(RevenueTtmString);
                    if (costOfRevenueTtm > 0)
                        ProfitTTM = RevenueTtm - CostOfRevenueTtm;
                }
            }
        }
        public float RevenueTtm
        {
            get => revenueTtm;
            set
            {
                revenueTtm = value;
                if (costOfRevenueTtm > 0)
                    ProfitTTM = revenueTtm - costOfRevenueTtm;
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
                {
                    Revenue2 = Convert.ToSingle(Revenue2String);
                    if (costOfRevenue2 > 0)
                        Profit2YearsAgo = Revenue2 - CostOfRevenue2;
                }
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
                {
                    Revenue4 = Convert.ToSingle(Revenue4String);
                    if (costOfRevenue4 > 0)
                        Profit4YearsAgo = Revenue4 - CostOfRevenue4;
                }
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
                {
                    CostOfRevenueTtm = Convert.ToSingle(CostOfRevenueTtmString);
                    if (revenueTtm > 0)
                        ProfitTTM = RevenueTtm - CostOfRevenueTtm;
                }
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
                {
                    CostOfRevenue2 = Convert.ToSingle(CostOfRevenue2String);
                    if (revenue2 > 0)
                        Profit2YearsAgo = Revenue2 - CostOfRevenue2;
                }
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
                {
                    CostOfRevenue4 = Convert.ToSingle(CostOfRevenue4String);
                    if (revenue4 > 0)
                        Profit4YearsAgo = Revenue4 - CostOfRevenue4;
                }
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

        /////////////////// TotalCash
        public Color TotalCashColor = Color.LightSteelBlue;
        private string totalCashString = NotApplicable;
        private float totalCash = 0;
        public string TotalCashString
        {
            get => totalCashString;
            set
            {
                string temp;
                
                totalCashString = value;
                if (NotNumber(value))
                    TotalCash = 0;
                else
                {
                    if (TotalCashString.IndexOf("B") > 0)
                    {
                        temp = TotalCashString.Replace("B", "");
                        TotalCash = Convert.ToSingle(temp) * 1000000000;
                    }
                    else if (TotalCashString.IndexOf("M") > 0)
                    {
                        temp = TotalCashString.Replace("M", "");
                        TotalCash = Convert.ToSingle(temp) * 1000000;
                    }
                    else if (TotalCashString.IndexOf("k") > 0)
                    {
                        temp = TotalCashString.Replace("k", "");
                        TotalCash = Convert.ToSingle(temp) * 1000;
                    }
                    else
                        TotalCash = Convert.ToSingle(value);
                }
            }
        }
        public float TotalCash
        {
            get => totalCash;
            set
            {
                totalCash = value;
            }
        }

        /////////////////// TotalDebt
        public Color TotalDebtColor = Color.LightSteelBlue;
        private string totalDebtString = NotApplicable;
        private float totalDebt = 0;
        public string TotalDebtString
        {
            get => totalDebtString;
            set
            {
                string temp;

                totalDebtString = value;
                if (NotNumber(value))
                    TotalDebt = 0;
                else
                {
                    if (TotalDebtString.IndexOf("B") > 0)
                    {
                        temp = TotalDebtString.Replace("B", "");
                        TotalDebt = Convert.ToSingle(temp) * 1000000000;
                    }
                    else if (TotalDebtString.IndexOf("M") > 0)
                    {
                        temp = TotalDebtString.Replace("M", "");
                        TotalDebt = Convert.ToSingle(temp) * 1000000;
                    }
                    else if (TotalDebtString.IndexOf("k") > 0)
                    {
                        temp = TotalDebtString.Replace("k", "");
                        TotalDebt = Convert.ToSingle(temp) * 1000;
                    }
                    else
                        TotalDebt = Convert.ToSingle(value);
                }
            }
        }
        public float TotalDebt
        {
            get => totalDebt;
            set
            {
                totalDebt = value;
            }
        }

        /////////////////// DebtEquity
        public Color DebtEquityColor = Color.LightSteelBlue;
        private string debtEquityString = NotApplicable;
        private float debtEquity = 0;
        public string DebtEquityString
        {
            get => debtEquityString;
            set
            {
                string temp = value.Replace("%", "");

                debtEquityString = value;
                if (NotNumber(temp))
                    DebtEquity = 0;
                else
                {
                    DebtEquity = Convert.ToSingle(temp);
                }
            }
        }
        public float DebtEquity
        {
            get => debtEquity;
            set
            {
                debtEquity = value;
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
            if (partial != "")
            {
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
            }
            else
                CostOfRevenueTtmString = CostOfRevenue2String = CostOfRevenue4String = "--";

            if (_revenueInMillions && CostOfRevenueTtm != 0 && CostOfRevenueTtmString != "--")
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

            // Set Colors of Cost of Revenue (if profit decreasing by 10% every 2 years, a problem
            if(CostOfRevenueTtm > 0 && CostOfRevenue4 > 0)
            {
                if ((RevenueTtm - CostOfRevenueTtm) < ((Revenue2 - CostOfRevenue2) * .9))
                    CostOfRevenueTtmColor = Color.Red;
                else if ((RevenueTtm - CostOfRevenueTtm) > ((Revenue2 - CostOfRevenue2) * 1.11))
                    CostOfRevenueTtmColor = Color.Lime;
                else
                    CostOfRevenueTtmColor = Color.LightSteelBlue;

                if ((Revenue2 - CostOfRevenue2) < ((Revenue4 - CostOfRevenue4) * .9))
                    CostOfRevenue2Color = Color.Red;
                else if ((Revenue2 - CostOfRevenue2) > ((Revenue4 - CostOfRevenue4) * 1.11))
                    CostOfRevenue2Color = Color.Lime;
                else
                    CostOfRevenue2Color = Color.LightSteelBlue;
            }

            // Set Colors of Total Debt
            if (TotalDebt > TotalCash * 1.6)
                TotalDebtColor = Color.Red;
            else if (TotalDebt < TotalCash * .6)
                TotalDebtColor = Color.Lime;
            else
                TotalDebtColor = Color.LightSteelBlue;

            // Set Colors of Debt Equity
            if (DebtEquity > 60)
                DebtEquityColor = Color.Red;
            else if (DebtEquity < 35)
                DebtEquityColor = Color.Lime;
            else
                DebtEquityColor = Color.LightSteelBlue;

            return true;
        }
        private bool NotNumber(string value)
        {
            return value == YahooFinance.NotApplicable || value == "" || value == "--" || "-0123456789.,".IndexOf(value.Substring(0, 1)) < 0;
        }
    }
}
