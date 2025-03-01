using Drake.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockApi
{
    public class StockFinancials : YahooFinance
    {
        private static readonly string _statisticsUrl = "https://finance.yahoo.com/quote/???/key-statistics/";
        private static readonly string _financialsUrl = "https://finance.yahoo.com/quote/???/financials/";

        public bool _revenueInMillions = false;
        public decimal ProfitTTM = 0;
        public Color ProfitTtmColor = Color.LightSteelBlue;
        public decimal Profit2YearsAgo = 0;
        public Color Profit2YearsAgoColor = Color.LightSteelBlue;
        public decimal Profit4YearsAgo = 0;
        public Color Profit4YearsAgoColor = Color.LightSteelBlue;


        ////////////////////////////////////////////
        ///                Properties
        ////////////////////////////////////////////
        ///
        /// RevenueTTM
        public StringSafeNumeric<Decimal> RevenueTtmString = new StringSafeNumeric<decimal>("--");
        public Color RevenueTtmColor = Color.LightSteelBlue;
        /// Revenue2
        public StringSafeNumeric<Decimal> Revenue2String = new StringSafeNumeric<decimal>("--");
        public Color Revenue2Color = Color.LightSteelBlue;
        /// Revenue4
        public StringSafeNumeric<Decimal> Revenue4String = new StringSafeNumeric<decimal>("--");
        public Color Revenue4Color = Color.LightSteelBlue;
        /// Cost of RevenueTTM
        public StringSafeNumeric<Decimal> CostOfRevenueTtmString = new StringSafeNumeric<decimal>("--");
        /// Cost of Revenue2
        public StringSafeNumeric<Decimal> CostOfRevenue2String = new StringSafeNumeric<decimal>("--");
        /////////////////// Cost of Revenue4
        public StringSafeNumeric<Decimal> CostOfRevenue4String = new StringSafeNumeric<decimal>("--");


        //private string costOfRevenue4String = NotApplicable;
        //private decimal costOfRevenue4 = 0;
        //public string CostOfRevenue4String
        //{
        //    get => costOfRevenue4String;
        //    set
        //    {
        //        costOfRevenue4String = value;
        //        if (NotNumber(value))
        //            CostOfRevenue4 = 0;
        //        else
        //        {
        //            CostOfRevenue4 = Convert.ToDecimal(CostOfRevenue4String);
        //        }
        //    }
        //}
        //public decimal CostOfRevenue4
        //{
        //    get => costOfRevenue4;
        //    set
        //    {
        //        costOfRevenue4 = value;
        //    }
        //}

        /////////////////// Operating Expense TTM
        private string operatingExpenseTtmString = NotApplicable;
        private decimal operatingExpenseTtm = 0;
        public string OperatingExpenseTtmString
        {
            get => operatingExpenseTtmString;
            set
            {
                operatingExpenseTtmString = value;
                if (NotNumber(value))
                    OperatingExpenseTtm = 0;
                else
                {
                    OperatingExpenseTtm = Convert.ToDecimal(OperatingExpenseTtmString);
                }
            }
        }
        public decimal OperatingExpenseTtm
        {
            get => operatingExpenseTtm;
            set
            {
                operatingExpenseTtm = value;
            }
        }

        ///////////////////  Operation Expense 2 Year
        private string operatingExpense2String = NotApplicable;
        private decimal operatingExpense2 = 0;
        public string OperatingExpense2String
        {
            get => operatingExpense2String;
            set
            {
                operatingExpense2String = value;
                if (NotNumber(value))
                    OperatingExpense2 = 0;
                else
                {
                    OperatingExpense2 = Convert.ToDecimal(OperatingExpense2String);
                }
            }
        }
        public decimal OperatingExpense2
        {
            get => operatingExpense2;
            set
            {
                operatingExpense2 = value;
            }
        }

        ///////////////////  Operation Expense 4 Year
        private string operatingExpense4String = NotApplicable;
        private decimal operatingExpense4 = 0;
        public string OperatingExpense4String
        {
            get => operatingExpense4String;
            set
            {
                operatingExpense4String = value;
                if (NotNumber(value))
                    OperatingExpense4 = 0;
                else
                {
                    OperatingExpense4 = Convert.ToDecimal(OperatingExpense4String);
                }
            }
        }
        public decimal OperatingExpense4
        {
            get => operatingExpense4;
            set
            {
                operatingExpense4 = value;
            }
        }

        /////////////////// TotalCash
        public Color TotalCashColor = Color.LightSteelBlue;
        private string totalCashString = NotApplicable;
        private decimal totalCash = 0;
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
                    if (TotalCashString.IndexOf("T") > 0)
                    {
                        temp = TotalCashString.Replace("T", "");
                        TotalCash = Convert.ToDecimal(temp) * 1000000000000;
                    }
                    else if (TotalCashString.IndexOf("B") > 0)
                    {
                        temp = TotalCashString.Replace("B", "");
                        TotalCash = Convert.ToDecimal(temp) * 1000000000;
                    }
                    else if (TotalCashString.IndexOf("M") > 0)
                    {
                        temp = TotalCashString.Replace("M", "");
                        TotalCash = Convert.ToDecimal(temp) * 1000000;
                    }
                    else if (TotalCashString.IndexOf("k") > 0)
                    {
                        temp = TotalCashString.Replace("k", "");
                        TotalCash = Convert.ToDecimal(temp) * 1000;
                    }
                    else
                        TotalCash = Convert.ToDecimal(value);
                }
            }
        }
        public decimal TotalCash
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
        private decimal totalDebt = 0;
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
                    try
                    {
                        if (TotalDebtString.IndexOf("B") > 0 || TotalDebtString.IndexOf("T") > 0)
                        {
                            temp = TotalDebtString.Replace("B", "").Replace("T", "");
                            TotalDebt = Convert.ToDecimal(temp) * 1000000000;
                        }
                        else if (TotalDebtString.IndexOf("M") > 0)
                        {
                            temp = TotalDebtString.Replace("M", "");
                            TotalDebt = Convert.ToDecimal(temp) * 1000000;
                        }
                        else if (TotalDebtString.IndexOf("k") > 0)
                        {
                            temp = TotalDebtString.Replace("k", "");
                            TotalDebt = Convert.ToDecimal(temp) * 1000;
                        }
                        else
                            TotalDebt = Convert.ToDecimal(value);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.Message + "\n" + Ticker + "\n" + value);
                    }
                }
            }
        }
        public decimal TotalDebt
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
        private decimal debtEquity = 0;
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
                    DebtEquity = Convert.ToDecimal(temp);
                }
            }
        }
        public decimal DebtEquity
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
        private decimal shortInterest = 0;
        public string ShortInterestString
        {
            get => shortInterestString;
            set
            {
                shortInterestString = value;
                if (NotNumber(value))
                    ShortInterest = 0;
                else
                    ShortInterest = Convert.ToDecimal(ShortInterestString);
            }
        }
        public decimal ShortInterest
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
            if (html.Length < 4000) // try again
            {
                Thread.Sleep(2000);
                html = await GetHtmlForTicker(_financialsUrl, Ticker);
            }

            try
            {
                // Revenue History
                string searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Revenue").Term;
                string partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 300);

                if (partial.Length < 100) // Some stocks like Vangaurd don't have financials, exit
                    return false; //=====>>>>>>>

                List<string> numbers = GetNumbersFromHtml(partial);
                numbers = numbers.Select(x => x._TrimSuffix(".")).ToList();

                if (numbers.Count > 0)
                    RevenueTtmString.StringValue = numbers[0].Trim();
                if (numbers.Count > 2)
                    Revenue2String.StringValue = numbers[2].Trim();
                if (numbers.Count > 4)
                    Revenue4String.StringValue = numbers[4].Trim();
                else if (numbers.Count > 3)
                    Revenue4String.StringValue = numbers[3].Trim();

                _revenueInMillions = false; // reset
                if (RevenueTtmString.StringValue.Length > 7 && Revenue4String.StringValue.Length > 7)
                {
                    _revenueInMillions = true;
                    RevenueTtmString.StringValue = RevenueTtmString.StringValue.Substring(0, RevenueTtmString.StringValue.Length - 4);
                    if (Revenue2String.StringValue.Length > 7)
                        Revenue2String.StringValue = Revenue2String.StringValue.Substring(0, Revenue2String.StringValue.Length - 4);
                    if (Revenue4String.StringValue.Length > 7)
                        Revenue4String.StringValue = Revenue4String.StringValue.Substring(0, Revenue4String.StringValue.Length - 4);
                }

                // Cost of Revenue History
                searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Cost of Revenue").Term;
                partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 300);
                if (partial != "")
                {
                    numbers = GetNumbersFromHtml(partial);
                    numbers = numbers.Select(x => x._TrimSuffix(".")).ToList();

                    if (numbers.Count > 0)
                        CostOfRevenueTtmString.StringValue = numbers[0].Trim();
                    if (numbers.Count > 2)
                        CostOfRevenue2String.StringValue = numbers[2].Trim();
                    if (numbers.Count > 4)
                        CostOfRevenue4String.StringValue = numbers[4].Trim();
                    else if (numbers.Count > 3)
                        CostOfRevenue4String.StringValue = numbers[3].Trim();
                }
                else
                    CostOfRevenueTtmString.StringValue = CostOfRevenue2String.StringValue = CostOfRevenue4String.StringValue = "--";

                if (_revenueInMillions && CostOfRevenueTtmString.NumericValue != 0 && CostOfRevenueTtmString.StringValue != "--")
                {
                    CostOfRevenueTtmString.StringValue = CostOfRevenueTtmString.StringValue.Substring(0, CostOfRevenueTtmString.StringValue.Length - 4);
                    CostOfRevenue2String.StringValue = CostOfRevenue2String.StringValue.Substring(0, CostOfRevenue2String.StringValue.Length - 4);
                    CostOfRevenue4String.StringValue = CostOfRevenue4String.StringValue.Substring(0, CostOfRevenue4String.StringValue.Length - 4);
                }

                // Operating Expenses
                searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Operating Expense").Term;
                partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 300);
                if (partial != "")
                {
                    numbers = GetNumbersFromHtml(partial);
                    numbers = numbers.Select(x => x._TrimSuffix(".")).ToList();

                    if (numbers.Count > 0)
                        OperatingExpenseTtmString = numbers[0].Trim();
                    if (numbers.Count > 2)
                        OperatingExpense2String = numbers[2].Trim();
                    if (numbers.Count > 4)
                        OperatingExpense4String = numbers[4].Trim();
                    else if (numbers.Count > 3)
                        OperatingExpense4String = numbers[3].Trim();
                }
                else
                    OperatingExpenseTtmString = OperatingExpense2String = OperatingExpense4String = "--";

                if (_revenueInMillions && OperatingExpenseTtm != 0 && OperatingExpenseTtmString != "--")
                {
                    OperatingExpenseTtmString = OperatingExpenseTtmString.Substring(0, OperatingExpenseTtmString.Length - 4);
                    OperatingExpense2String = OperatingExpense2String.Substring(0, OperatingExpense2String.Length - 4);
                    OperatingExpense4String = OperatingExpense4String.Substring(0, OperatingExpense4String.Length - 4);
                }

                if (RevenueTtmString.NumericValue > 0)
                    ProfitTTM = RevenueTtmString.NumericValue - CostOfRevenueTtmString.NumericValue - OperatingExpenseTtm;
                if (Revenue2String.NumericValue > 0)
                    Profit2YearsAgo = Revenue2String.NumericValue - CostOfRevenue2String.NumericValue - OperatingExpense2;
                if (Revenue4String.NumericValue > 0)
                    Profit4YearsAgo = Revenue4String.NumericValue - CostOfRevenue4String.NumericValue - OperatingExpense4;


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
                if (RevenueTtmString.NumericValue < (Revenue2String.NumericValue * .95M))
                    RevenueTtmColor = Color.Red;
                else if (RevenueTtmString.NumericValue > (Revenue2String.NumericValue * 1.05M))
                    RevenueTtmColor = Color.Lime;
                else
                    RevenueTtmColor = Color.LightSteelBlue;

                if (Revenue2String.NumericValue < (Revenue4String.NumericValue * .95M))
                    Revenue2Color = Color.Red;
                else if (Revenue2String.NumericValue > (Revenue4String.NumericValue * 1.05M))
                    Revenue2Color = Color.Lime;
                else
                    Revenue2Color = Color.LightSteelBlue;

                // Set Colors for Profits labels (if profit decreasing by 10% every 2 years, a problem
                if (RevenueTtmString.NumericValue > 0 && CostOfRevenueTtmString.NumericValue > 0 && CostOfRevenue4String.NumericValue > 0)
                {
                    if (ProfitTTM < Profit2YearsAgo * .9M)
                        ProfitTtmColor = Color.Red;
                    else if (ProfitTTM > (Profit2YearsAgo * 1.11M))
                        ProfitTtmColor = Color.Lime;
                    else
                        ProfitTtmColor = Color.LightSteelBlue;

                    if (Profit2YearsAgo < (Profit4YearsAgo * .9M))
                        Profit2YearsAgoColor = Color.Red;
                    else if (Profit2YearsAgo > (Profit4YearsAgo * 1.11M))
                        Profit2YearsAgoColor = Color.Lime;
                    else
                        Profit2YearsAgoColor = Color.LightSteelBlue;
                }

                // Set Colors of Total Debt
                if (TotalDebt > TotalCash * 1.6M)
                    TotalDebtColor = Color.Red;
                else if (TotalDebt < TotalCash * .6M)
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
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Source + x.Message + "\n" + "GetFinancialData() " + " " + ticker  + "\n" + html.Substring(0, html.Length / 10));
            }

            return true;
        }
        private bool NotNumber(string value)
        {
            return value == YahooFinance.NotApplicable || value == "" || value == "--" || "-0123456789.,".IndexOf(value.Substring(0, 1)) < 0;
        }
    }
}
