using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockApi
{
    public class StockStatistics : YahooFinance
    {
        private static readonly string _statisticsUrl = "https://finance.yahoo.com/quote/???/key-statistics/";

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

        /// Short Interest
        public StringSafeType<Decimal> ShortInterestString = new StringSafeType<decimal>("--");
        public Color ShortInterestColor = Form1.TextForeColor;
        /// Debt Equity
        public StringSafeType<Decimal> DebtEquityString = new StringSafeType<decimal>("--");
        public Color DebtEquityColor = Form1.TextForeColor;


        ////////////////////////////////////////////
        ///                Methods
        ////////////////////////////////////////////
        public override async Task<bool> GetStockData(string ticker)
        {
            Ticker = ticker;
            string html = "";
            string searchTerm;

            try
            {

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
                MessageBox.Show(x.Source + x.Message + "\n" + "GetStatisticsData() " + " " + ticker + "\n" + html.Substring(0, html.Length / 10));
            }

            return true;
        }
    }
}
