﻿using Drake.Extensions;
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

//        public decimal ProfitTTM = 0;
        public Color ProfitTtmColor = Form1.TextForeColor;
        public decimal Profit2YearsAgo = 0;
        public Color Profit2YearsAgoColor = Form1.TextForeColor;
        public decimal Profit4YearsAgo = 0;
        public Color Profit4YearsAgoColor = Form1.TextForeColor;


        ////////////////////////////////////////////
        ///                Properties
        ////////////////////////////////////////////

        /// RevenueTTM
        public StringSafeType<Decimal> RevenueTtmString = new StringSafeType<decimal>("--");
        public Color RevenueTtmColor = Form1.TextForeColor;
        /// Revenue2
        public StringSafeType<Decimal> Revenue2String = new StringSafeType<decimal>("--");
        public Color Revenue2Color = Form1.TextForeColor;
        /// Revenue4
        public StringSafeType<Decimal> Revenue4String = new StringSafeType<decimal>("--");
        public Color Revenue4Color = Form1.TextForeColor;
        /// Cost of RevenueTTM
        public StringSafeType<Decimal> CostOfRevenueTtmString = new StringSafeType<decimal>("--");
        /// Cost of Revenue2
        public StringSafeType<Decimal> CostOfRevenue2String = new StringSafeType<decimal>("--");
        /// Cost of Revenue4
        public StringSafeType<Decimal> CostOfRevenue4String = new StringSafeType<decimal>("--");
        /// Operating Expense TTM
        public StringSafeType<Decimal> OperatingExpenseTtmString = new StringSafeType<decimal>("--");
        ///  Operation Expense 2 Year
        public StringSafeType<Decimal> OperatingExpense2String = new StringSafeType<decimal>("--");
        /// Profit TTM
        public StringSafeType<Decimal> ProfitTtmString = new StringSafeType<decimal>("--");

        ///  Operation Expense 4 Year
        public StringSafeType<Decimal> OperatingExpense4String = new StringSafeType<decimal>("--");
        /// Debt Equity
        public StringSafeType<Decimal> DebtEquityString = new StringSafeType<decimal>("--");
        public Color DebtEquityColor = Form1.TextForeColor;
        /// Short Interest
        public StringSafeType<Decimal> ShortInterestString = new StringSafeType<decimal>("--");
        public Color ShortInterestColor = Form1.TextForeColor;

        /////////////////// TotalCash
        public Color TotalCashColor = Form1.TextForeColor;
        private string totalCashString = NotApplicable;
        private decimal totalCash = 0;
        public string TotalCashString
        {
            get => totalCashString;
            set
            {
                totalCashString = value;
                if (NotNumber(value))
                    TotalCash = 0;
                else
                {
                    TotalCash = ShortInterestString.ConvertNumericSuffix(value);
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
        public Color TotalDebtColor = Form1.TextForeColor;
        private string totalDebtString = NotApplicable;
        private decimal totalDebt = 0;
        public string TotalDebtString
        {
            get => totalDebtString;
            set
            {
                totalDebtString = value;
                if (NotNumber(value))
                    TotalDebt = 0;
                else
                {
                    TotalDebt = ShortInterestString.ConvertNumericSuffix(value);
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


        private bool NotNumber(string value)
        {
            return value == YahooFinance.NotApplicable || value == "" || value == "--" || "-0123456789.,".IndexOf(value.Substring(0, 1)) < 0;
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

                if (numbers.Count > 0)
                {
                    RevenueTtmString.StringValue = numbers[0].Trim();

                }
                if (numbers.Count > 2)
                    Revenue2String.StringValue = numbers[2].Trim();
                if (numbers.Count > 4)
                    Revenue4String.StringValue = numbers[4].Trim();
                else if (numbers.Count > 3)
                    Revenue4String.StringValue = numbers[3].Trim();

                // Cost of Revenue History
                searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Cost of Revenue").Term;
                partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 300);
                if (partial != "")
                {
                    numbers = GetNumbersFromHtml(partial);
                    //numbers = numbers.Select(x => x._TrimSuffix(".")).ToList();

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

                // Operating Expenses
                searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Operating Expense").Term;
                partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 300);
                if (partial != "")
                {
                    numbers = GetNumbersFromHtml(partial);
                    //numbers = numbers.Select(x => x._TrimSuffix(".")).ToList();

                    if (numbers.Count > 0)
                        OperatingExpenseTtmString.StringValue = numbers[0].Trim();
                    if (numbers.Count > 2)
                        OperatingExpense2String.StringValue = numbers[2].Trim();
                    if (numbers.Count > 4)
                        OperatingExpense4String.StringValue = numbers[4].Trim();
                    else if (numbers.Count > 3)
                        OperatingExpense4String.StringValue = numbers[3].Trim();
                }
                else
                    OperatingExpenseTtmString.StringValue = OperatingExpense2String.StringValue = OperatingExpense4String.StringValue = "--";

                if (RevenueTtmString.NumericValue > 0)
                {
                    ProfitTtmString.NumericValue = RevenueTtmString.NumericValue - CostOfRevenueTtmString.NumericValue - OperatingExpenseTtmString.NumericValue;
                    ProfitTtmString.StringValue = ProfitTtmString.NumericValue.ToString("n0");
                }

                if (Revenue2String.NumericValue > 0)
                    Profit2YearsAgo = Revenue2String.NumericValue - CostOfRevenue2String.NumericValue - OperatingExpense2String.NumericValue;
                if (Revenue4String.NumericValue > 0)
                    Profit4YearsAgo = Revenue4String.NumericValue - CostOfRevenue4String.NumericValue - OperatingExpense4String.NumericValue;


                Thread.Sleep(1000);
                html = await GetHtmlForTicker(_statisticsUrl, Ticker);

                // Total Cash
                searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Cash").Term;
                TotalCashString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);

                if (TotalCashString == "--") // try again
                {
                    Thread.Sleep(2000);
                    html = await GetHtmlForTicker(_statisticsUrl, Ticker);
                    // Total Cash
                    searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Cash").Term;
                    TotalCashString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
                }

                // Total Debt
                searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Total Debt").Term;
                TotalDebtString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);
                // Debt/Equity Ratio
                searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Debt/Equity").Term;
                DebtEquityString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 2);


                // Short Interest
                searchTerm = YahooFinance.SearchTerms.Find(x => x.Name == "Short Interest").Term;
                ShortInterestString.StringValue = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
                if (ShortInterestString.StringValue != YahooFinance.NotApplicable && ShortInterestString.StringValue.IndexOf("%") > 0)
                    ShortInterestString.StringValue = ShortInterestString.StringValue.Substring(0, ShortInterestString.StringValue.IndexOf("%"));
                else
                    ShortInterestString.StringValue = "--";

                // Set Colors of Revenue (if revenue decreasing by 5% every 2 years, a problem
                if (RevenueTtmString.NumericValue < (Revenue2String.NumericValue * .95M))
                    RevenueTtmColor = Color.Red;
                else if (RevenueTtmString.NumericValue > (Revenue2String.NumericValue * 1.05M))
                    RevenueTtmColor = Color.Lime;
                else
                    RevenueTtmColor = Form1.TextForeColor;

                if (Revenue2String.NumericValue < (Revenue4String.NumericValue * .95M))
                    Revenue2Color = Color.Red;
                else if (Revenue2String.NumericValue > (Revenue4String.NumericValue * 1.05M))
                    Revenue2Color = Color.Lime;
                else
                    Revenue2Color = Form1.TextForeColor;

                // Set Colors for Profits labels (if profit decreasing by 10% every 2 years, a problem
                if (RevenueTtmString.NumericValue > 0 && CostOfRevenueTtmString.NumericValue > 0 && CostOfRevenue4String.NumericValue > 0)
                {
                    if (ProfitTtmString.NumericValue < Profit2YearsAgo * .9M)
                        ProfitTtmColor = Color.Red;
                    else if (ProfitTtmString.NumericValue > (Profit2YearsAgo * 1.11M))
                        ProfitTtmColor = Color.Lime;
                    else
                        ProfitTtmColor = Form1.TextForeColor;

                    if (Profit2YearsAgo < (Profit4YearsAgo * .9M))
                        Profit2YearsAgoColor = Color.Red;
                    else if (Profit2YearsAgo > (Profit4YearsAgo * 1.11M))
                        Profit2YearsAgoColor = Color.Lime;
                    else
                        Profit2YearsAgoColor = Form1.TextForeColor;
                }

                // Set Colors of Total Debt
                if (TotalDebt > TotalCash * 1.6M)
                    TotalDebtColor = Color.Red;
                else if (TotalDebt < TotalCash * .6M)
                    TotalDebtColor = Color.Lime;
                else
                    TotalDebtColor = Form1.TextForeColor;

                // Set Colors of Debt Equity
                if (DebtEquityString.NumericValue > 60)
                    DebtEquityColor = Color.Red;
                else if (DebtEquityString.NumericValue < 35)
                    DebtEquityColor = Color.Lime;
                else
                    DebtEquityColor = Form1.TextForeColor;

                // Set Colors of Debt Equity
                if (ShortInterestString.NumericValue > 8)
                    ShortInterestColor = Color.Red;
                else if (ShortInterestString.NumericValue < 2)
                    ShortInterestColor = Color.Lime;
                else
                    ShortInterestColor = Form1.TextForeColor;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Source + x.Message + "\n" + "GetFinancialData() " + " " + ticker + "\n" + html.Substring(0, html.Length / 10));
            }

            return true;
        }
    }
}
