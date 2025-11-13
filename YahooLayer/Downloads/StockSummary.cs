using SqlLayer;
using SqlLayer.SQL_Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace YahooLayer
{
    public class StockSummary : YahooFinanceBase
    {
        public enum ValuationEnum
        {
            UnderValued,
            FairValue,
            OverValued
        }

        private static readonly string _summaryUrl = "https://finance.yahoo.com/quote/???";

        public Color DividendColor = Color.White;
        public Color EPSColor = Color.White;
        public Color PriceBookColor = Color.White;
        public Color ProfitMarginColor = Color.White;
        public Color OneYearTargetColor = Color.White;
        public Color ForwardPEColor = Color.White;
        public Color EarningsDateColor = Color.White;

        public string _html = "";
        private string companyName = "";
        public string CompanyOverview = "";
        public string Sector = "";
        public int AverageSectorPE = 20;
        public ValuationEnum Valuation = ValuationEnum.FairValue;
        public Dictionary<string, int> _sectors = new Dictionary<string, int>() { { "Technology", 35 }, { "Energy", 15 }, { "Materials", 25 }, { "Industrials", 26 }, { "Utilities", 21 }, { "Healthcare", 20 }, { "Real Estate", 36 }, { "Financial Services", 16 }, { "Communication Services", 21 }, { "Consumer Defensive", 24 } };
        public Exception LastException = null;
        public string Error = "";

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

        public SqlTicker sqlTicker = new SqlTicker();

        public StockSummary()
        {
            DividendColor      = _normalColor;
            EPSColor           = _normalColor;
            PriceBookColor     = _normalColor;
            ProfitMarginColor  = _normalColor;
            OneYearTargetColor = _normalColor;
            ForwardPEColor     = _normalColor;
            EarningsDateColor  = _normalColor;
        }

        ////////////////////////////////////////////
        ///                Methods
        ////////////////////////////////////////////
        public override async Task<bool> GetStockData(string ticker)
        {
            Error = "";
            LastException = null;
            string searchTerm = "";

            Ticker = ticker;

            _html = await GetHtmlForTicker(_summaryUrl, Ticker);

            CompanyName = GetDataByTagName(_html, "title", Ticker);
            CompanyName = CompanyName.Substring(0, CompanyName.IndexOf(")") + 1);
            CompanyName = HttpUtility.HtmlDecode(CompanyName);

            if (CompanyName == "")
            {
                CompanyName = "Not Found";
                return false;
            }

            try
            {
                Debug.WriteLine("GetStockSummary()");

                // Price
                searchTerm = SearchTerms.Find(x => x.Name == "Price").Term;
                PriceString.StringValue = GetValueFromHtmlBySearchTerm(_html, searchTerm, YahooFinanceBase.NotApplicable, 1);

                // EPS
                searchTerm = SearchTerms.Find(x => x.Name == "EPS").Term;
                EarningsPerShareString.StringValue = GetValueFromHtmlBySearchTerm(_html, searchTerm, "--", 4);

                // Volatility
                searchTerm = SearchTerms.Find(x => x.Name == "Volatility").Term;
                VolatilityString.StringValue = GetValueFromHtmlBySearchTerm(_html, searchTerm, "1", 3);
                if (VolatilityString.NumericValue == 0)
                    VolatilityString.NumericValue = 1;

                // Dividend
                searchTerm = SearchTerms.Find(x => x.Name == "Dividend").Term;
                string dividend = GetValueFromHtmlBySearchTerm(_html, searchTerm, "0", 3);
                if (!dividend.Contains(YahooFinanceBase.NotApplicable) && dividend.IndexOf("(") > 1)
                {
                    dividend = dividend.Substring(dividend.IndexOf("(") + 1);
                    dividend = dividend.Substring(0, dividend.IndexOf(")") - 1);
                }
                else
                {
                    searchTerm = SearchTerms.Find(x => x.Name == "Dividend2").Term;
                    dividend = GetValueFromHtmlBySearchTerm(_html, searchTerm, "0", 3);
                }
                DividendString.StringValue = dividend;

                // One year target
                searchTerm = SearchTerms.Find(x => x.Name == "One Year Target").Term;
                OneYearTargetPriceString.StringValue = GetValueFromHtmlBySearchTerm(_html, searchTerm, YahooFinanceBase.NotApplicable, 4).Trim();
                if (OneYearTargetPriceString.NumericValue == 0)
                    OneYearTargetPriceString.StringValue = PriceString.StringValue;

                // Price / Book
                searchTerm = SearchTerms.Find(x => x.Name == "Price/Book").Term;
                PriceBookString.StringValue = GetValueFromHtmlBySearchTerm(_html, searchTerm, YahooFinanceBase.NotApplicable, 2);

                //Profit Margin %
                searchTerm = SearchTerms.Find(x => x.Name == "Profit Margin").Term;
                string profitMarginString = GetValueFromHtmlBySearchTerm(_html, searchTerm, YahooFinanceBase.NotApplicable, 2);
                if (profitMarginString != YahooFinanceBase.NotApplicable && profitMarginString.IndexOf("%") > 0)
                    ProfitMarginString.StringValue = profitMarginString.Substring(0, profitMarginString.IndexOf("%"));
                else
                    ProfitMarginString.StringValue = YahooFinanceBase.NotApplicable;

                // 52 Week Range
                searchTerm = SearchTerms.Find(x => x.Name == "52 Week Range").Term;
                string range = GetValueFromHtmlBySearchTerm(_html, searchTerm, YahooFinanceBase.NotApplicable, 4);
                int idx = range.IndexOf("-");
                if (idx > 0)
                {
                    YearsRangeLow.StringValue = range.Substring(0, idx).Trim();
                    YearsRangeHigh.StringValue = range.Substring(idx + 1).Trim();
                }
                else // Mutual funds don't have 52 week range value on yahoo!
                {

                    YearsRangeLow.StringValue = PriceString.StringValue;  // Fake it
                    YearsRangeHigh.StringValue = PriceString.StringValue;
                }

                // Forward P/E
                searchTerm = SearchTerms.Find(x => x.Name == "Forward P/E").Term;
                ForwardPEString.StringValue = GetValueFromHtmlBySearchTerm(_html, searchTerm, YahooFinanceBase.NotApplicable, 2);

                // Earnings Date
                searchTerm = SearchTerms.Find(x => x.Name == "Earnings Date").Term;
                EarningsDateString.StringValue = GetValueFromHtmlBySearchTerm(_html, searchTerm, YahooFinanceBase.NotApplicable, 3);

                Debug.WriteLine("GetStockSummary() Company Overview");
                // Company Overview
                searchTerm = SearchTerms.Find(x => x.Name == "Company Overview").Term;
                string htmlSnippet = GetPartialHtmlFromHtmlBySearchTerm(_html, searchTerm, 6000);
                htmlSnippet = HttpUtility.HtmlDecode(htmlSnippet);
                string[] parts = htmlSnippet.Split(">");
                string longest = parts.OrderByDescending(s => s.Length).First();

                try
                {
                    AverageSectorPE = 20;

                    int secFound = parts.Where(x => x.Contains("Sector<")).Count();

                    if(secFound > 0)
                    {
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
                    }
                }
                catch (Exception ex)
                {
                    logger.Error($"{ex.Message}  {ex.StackTrace}");
                    Debug.WriteLine($"GetStockData() {ex.Message}");
                }

                CompanyOverview = longest._TrimSuffix("</");

                ///////////////////////////////////
                ///      Save to SQL Server
                SqlCrudOperations _finacialStatement = new SqlCrudOperations();
                _finacialStatement.SaveSummary(MapFrom(this));
            }
            catch (Exception ex)
            {
                logger.Error($"******ERROR**********\r\n   {ex.StackTrace}\r\n   {ex.StackTrace}\r\n");
                LastException = ex;
                Error = "GetSummaryData() " + ticker + " " + searchTerm;
                Debug.WriteLine("Error!");
            }

            //*******************
            //    Set Colors
            //*******************
            DividendColor = _normalColor;
            if (DividendString.NumericValue > 1.5M)
                DividendColor = Color.Lime;
            EPSColor = _normalColor;
            if (EarningsPerShareString.NumericValue < -1)
                EPSColor = Color.Red;
            else if (EarningsPerShareString.NumericValue > 1)
                EPSColor = Color.Lime;
            if (ProfitMarginString.NumericValue < -2)
                ProfitMarginColor = Color.Red;
            else if (ProfitMarginString.NumericValue > 2)
                ProfitMarginColor = Color.Lime;
            else
                ProfitMarginColor = _normalColor;
            
            if (PriceBookString.NumericValue > 5)
                PriceBookColor = Color.Red;
            else if (PriceBookString.NumericValue < 1)
                PriceBookColor = Color.Lime;
            else
                PriceBookColor = _normalColor;
            OneYearTargetColor = _normalColor;
            if (OneYearTargetPriceString.NumericValue < PriceString.NumericValue * .9M)
                OneYearTargetColor = Color.Red;
            else if (OneYearTargetPriceString.NumericValue > PriceString.NumericValue * 1.1M)
                OneYearTargetColor = Color.Lime;

            if (ForwardPEString.NumericValue > 50)
                ForwardPEColor = Color.Red;
            else if (ForwardPEString.NumericValue < 15)
                ForwardPEColor = Color.Lime;
            else
                ForwardPEColor = _normalColor;

            ForwardPEColor = _normalColor;
            if (EarningsDateString.IsDateTime)
            {
                DateTime dt = (EarningsDateString.DateTimeValue ?? DateTime.Now).Date;
                if (dt  == DateTime.Now.Date)
                    EarningsDateColor = Color.Lime;
                else if (dt == DateTime.Now.AddDays(1).Date)
                    EarningsDateColor = Color.LightGreen;
            }

            return true;
        }

        public void SetCalculatedPE(StockDownloads stockDownloads)
        {
            // Combine profit growth and margin into a number
            decimal marginFactor = 1 + (stockDownloads.stockSummary.ProfitMarginString.NumericValue / 100M);
            stockDownloads.stockSummary.CalculatedPEString.StringValue = (stockDownloads.stockSummary.ForwardPEString.NumericValue / (marginFactor * stockDownloads.stockIncomeStatement.ProfitGrowth)).ToString("0.00");
            stockDownloads.stockSummary.Valuation = StockSummary.ValuationEnum.FairValue;

            //if (_stockSummary.CalculatedPEString.NumericValue > 0 && _stockSummary.CalculatedPEString.NumericValue > (decimal)_stockSummary.AverageSectorPE * 1.3M) // Over valued
            if (stockDownloads.stockSummary.CalculatedPEString.NumericValue > 0 && stockDownloads.stockSummary.CalculatedPEString.NumericValue > (decimal)stockDownloads.stockSummary.AverageSectorPE * 1.35M) // Over valued
                stockDownloads.stockSummary.Valuation = StockSummary.ValuationEnum.OverValued;
            else if (stockDownloads.stockSummary.CalculatedPEString.NumericValue > 0 && stockDownloads.stockSummary.CalculatedPEString.NumericValue < (decimal)stockDownloads.stockSummary.AverageSectorPE * .70M) // Under valued
                stockDownloads.stockSummary.Valuation = StockSummary.ValuationEnum.UnderValued;
        }

        /// /////////////////////////////////////////////////////////////////////////////////////////
        ///                                    SQL Support
        /// /////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckSqlForRecentData()
        {
            SqlCrudOperations sqlFinancialStatement = new SqlCrudOperations();
            List<SqlSummary> entities = sqlFinancialStatement.GetSummaryList(Ticker);
            Random random = new Random();

            if (entities.Count > 0)
            {
                DateTime staleDate = DateTime.Now.Date.AddDays(-12 + random.Next(1, 4));

                if (entities[0].UpdateDate > staleDate) // We have recent data in the database, use it.
                {
                    MapFill(entities[0]);
                    return true;
                }

                return false; // need to go out and download new data
            }

            return false;
        }

        public static SqlSummary MapFrom(StockSummary source)
        {
            SqlSummary sqlSummary = new SqlSummary();

            sqlSummary.Ticker = source.Ticker;
            sqlSummary.Valuation = (SqlLayer.SQL_Models.ValuationEnum)(int)source.Valuation;
            sqlSummary.Dividend = (double)source.DividendString.NumericValue;
            sqlSummary.EarningsPerShare = (double)source.EarningsPerShareString.NumericValue;
            sqlSummary.ProfitMargin = (double)source.ProfitMarginString.NumericValue;
            sqlSummary.PriceBook = (double)source.PriceBookString.NumericValue;
            sqlSummary.OneYearTargetPrice = (double)source.OneYearTargetPriceString.NumericValue;
            sqlSummary.Price = (double)source.PriceString.NumericValue;
            sqlSummary.Volatility = (double)source.VolatilityString.NumericValue;
            sqlSummary.YearsRangeLow = (double)source.YearsRangeLow.NumericValue;
            sqlSummary.YearsRangeHigh = (double)source.YearsRangeHigh.NumericValue;
            sqlSummary.ForwardPE = (double)source.ForwardPEString.NumericValue;
            sqlSummary.EarningsDate = source.EarningsDateString.DateTimeValue;
            sqlSummary.UpdateDate = DateTime.Now.Date;

            return sqlSummary;
        }

        public void MapFill(SqlSummary source)
        {
            Ticker = source.Ticker;
            Valuation = (ValuationEnum)(int)source.Valuation;
            DividendString.NumericValue = (decimal)source.Dividend;
            EarningsPerShareString.NumericValue = (decimal)source.EarningsPerShare;
            ProfitMarginString.NumericValue = (decimal)source.ProfitMargin;
            PriceBookString.NumericValue = (decimal)source.PriceBook;
            OneYearTargetPriceString.NumericValue = (decimal)source.OneYearTargetPrice;
            PriceString.NumericValue = (decimal)source.Price;
            VolatilityString.NumericValue = (decimal)source.Volatility;
            YearsRangeLow.NumericValue = (decimal)source.YearsRangeLow;
            YearsRangeHigh.NumericValue = (decimal)source.YearsRangeHigh;
            ForwardPEString.NumericValue = (decimal)source.ForwardPE;
            EarningsDateString.DateTimeValue = source.EarningsDate;

            return;
        }
    }
}