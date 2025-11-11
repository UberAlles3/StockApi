using SqlLayer;
using SqlLayer.SQL_Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YahooLayer
{
    public class StockIncomeStatement : YahooFinance
    {
        private static readonly string _financialsUrl = "https://finance.yahoo.com/quote/???/financials/";
        public decimal ProfitGrowth = 1;

        ////////////////////////////////////////////
        ///                Properties
        ////////////////////////////////////////////
        public Color RevenueTtmColor       = Color.White;
        public Color Revenue2Color         = Color.White;
        public Color Revenue4Color         = Color.White;
        public Color CostOfRevenueTtmColor = Color.White;
        public Color CostOfRevenue2Color   = Color.White;
        public Color CostOfRevenue4Color   = Color.White;
        public Color BasicEpsTtmColor      = Color.White;
        public Color BasicEps2Color        = Color.White;
        public Color BasicEps4Color        = Color.White;
        public Color ProfitTtmColor        = Color.White;
        public Color Profit2YearsAgoColor  = Color.White;
        public Color Profit4YearsAgoColor  = Color.White;
                                          

        /// Revenue
        public StringSafeType<Decimal> RevenueTtmString = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> Revenue2String = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> Revenue4String = new StringSafeType<decimal>("--", "N0");

        /// Cost of Revenue
        public StringSafeType<Decimal> CostOfRevenueTtmString = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> CostOfRevenue2String = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> CostOfRevenue4String = new StringSafeType<decimal>("--", "N0");

        /// Operating Expense
        public StringSafeType<Decimal> OperatingExpenseTtmString = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> OperatingExpense2String = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> OperatingExpense4String = new StringSafeType<decimal>("--", "N0");
        
        /// Net Income
        public StringSafeType<Decimal> NetIncomeTtmString = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> NetIncome2String = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> NetIncome4String = new StringSafeType<decimal>("--", "N0");

        /// Basic EPS
        public StringSafeType<Decimal> BasicEpsTtmString = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> BasicEps2String = new StringSafeType<decimal>("--");
        public StringSafeType<Decimal> BasicEps4String = new StringSafeType<decimal>("--");

        /// Profit 
        public StringSafeType<Decimal> ProfitTtmString = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> Profit2String = new StringSafeType<decimal>("--", "N0");
        public StringSafeType<Decimal> Profit4String = new StringSafeType<decimal>("--", "N0");

        public StockIncomeStatement()
        {
            RevenueTtmColor       = _normalColor;
            Revenue2Color         = _normalColor;
            Revenue4Color         = _normalColor;
            CostOfRevenueTtmColor = _normalColor;
            CostOfRevenue2Color   = _normalColor;
            CostOfRevenue4Color   = _normalColor;
            BasicEpsTtmColor      = _normalColor;
            BasicEps2Color        = _normalColor;
            BasicEps4Color        = _normalColor;
            ProfitTtmColor        = _normalColor;
            Profit2YearsAgoColor  = _normalColor;
            Profit4YearsAgoColor  = _normalColor;
    }

    //////////////////////////
    ///                Methods
    ////////////////////////////////////////////
    public override async Task<bool> GetStockData(string ticker)
        {
            Ticker = ticker;
            string html;

            bool hasSqlData = CheckSqlForRecentData();
            if (!hasSqlData)
            {
                html = await GetHtmlForTicker(_financialsUrl, Ticker);
                if (html.Length < 4000) // try again
                {
                    Thread.Sleep(2000);
                    html = await GetHtmlForTicker(_financialsUrl, Ticker);
                }

                try
                {
                    Debug.WriteLine("GetStockIncomeStatement()");

                    //// Revenue History
                    if (ParseHtmlRowData(html, "Total Revenue", RevenueTtmString, Revenue2String, Revenue4String) == false)
                        return false; //=====>>>>>>> Exit

                    // Cost of Revenue History
                    if (ParseHtmlRowData(html, "Cost of Revenue", CostOfRevenueTtmString, CostOfRevenue2String, CostOfRevenue4String) == false)
                    {
                        // Use Total Expenses instead
                        if (ParseHtmlRowData(html, "Total Expenses", CostOfRevenueTtmString, CostOfRevenue2String, CostOfRevenue4String) == false)
                        {
                            CostOfRevenueTtmString.StringValue = CostOfRevenue2String.StringValue = CostOfRevenue4String.StringValue = "--";
                        }
                    }

                    //// Net Income
                    if (ParseHtmlRowData(html, "Net Income Common", NetIncomeTtmString, NetIncome2String, NetIncome4String) == false)
                        return false; //=====>>>>>>> Exit

                    //// Operating Expenses
                    if (ParseHtmlRowData(html, "Operating Expense", OperatingExpenseTtmString, OperatingExpense2String, OperatingExpense4String) == false)
                    {
                        // Not found, calculate from revenue and net income
                        CostOfRevenueTtmString.NumericValue = RevenueTtmString.NumericValue - NetIncomeTtmString.NumericValue;
                        CostOfRevenue2String.NumericValue = Revenue2String.NumericValue - NetIncome2String.NumericValue;
                        CostOfRevenue4String.NumericValue = Revenue4String.NumericValue - NetIncome4String.NumericValue;
                    }

                    //// Basic EPS
                    if (ParseHtmlRowData(html, "Basic EPS", BasicEpsTtmString, BasicEps2String, BasicEps4String) == false)
                    {
                        // Not found, set to default   
                        BasicEpsTtmString.StringValue = BasicEps2String.StringValue = BasicEps4String.StringValue = "--";
                    }

                    ///////////////////////////////////
                    ///      Save to SQL Server
                    List<SqlIncomeStatement> ListSis = MapFrom(this);
                    SqlCrudOperations _finacialStatement = new SqlCrudOperations();
                    _finacialStatement.SaveIncomeStatements(ListSis);
                }
                catch (Exception ex)
                {
                    logger.Error($"{ex.Message}\r\n  {ex.StackTrace}\r\n");
                    //MessageBox.Show(ex.Source + ex.Message + "\n" + "GetIncomeStatementData() " + " " + ticker + "\n" + html.Substring(0, html.Length / 10));
                }
            }
            
            //////////////////////////////////////////
            /// Determine Profit
            if (RevenueTtmString.NumericValue > 0)
            {
                ProfitTtmString.NumericValue = RevenueTtmString.NumericValue - CostOfRevenueTtmString.NumericValue - OperatingExpenseTtmString.NumericValue;
            }

            if (Revenue2String.NumericValue > 0)
                Profit2String.NumericValue = Revenue2String.NumericValue - CostOfRevenue2String.NumericValue - OperatingExpense2String.NumericValue;
            if (Revenue4String.NumericValue > 0)
                Profit4String.NumericValue = Revenue4String.NumericValue - CostOfRevenue4String.NumericValue - OperatingExpense4String.NumericValue;

            ///////////////////////////////////////////////////
            //                   Set Colors
            // Set Colors of Revenue (if revenue decreasing by 5% every 2 years, a problem
            if (RevenueTtmString.NumericValue < (Revenue2String.NumericValue * .95M))
                RevenueTtmColor = Color.Red;
            else if (RevenueTtmString.NumericValue > (Revenue2String.NumericValue * 1.05M))
                RevenueTtmColor = Color.Lime;
            else
                RevenueTtmColor = _normalColor;

            if (Revenue2String.NumericValue < (Revenue4String.NumericValue * .95M))
                Revenue2Color = Color.Red;
            else if (Revenue2String.NumericValue > (Revenue4String.NumericValue * 1.05M))
                Revenue2Color = Color.Lime;
            else
                Revenue2Color = _normalColor;

            // Set Colors for Profits labels (if profit decreasing by 10% every 2 years, a problem
            if (RevenueTtmString.NumericValue > 0) // && CostOfRevenueTtmString.NumericValue > 0 && CostOfRevenue4String.NumericValue > 0 <-- Why was this in there?
            {
                if (ProfitTtmString.NumericValue < Profit2String.NumericValue * .9M)
                    ProfitTtmColor = Color.Red;
                else if (ProfitTtmString.NumericValue > (Profit2String.NumericValue * 1.11M))
                    ProfitTtmColor = Color.Lime;
                else
                    ProfitTtmColor = _normalColor;

                if (Profit2String.NumericValue < (Profit4String.NumericValue * .9M))
                    Profit2YearsAgoColor = Color.Red;
                else if (Profit2String.NumericValue > (Profit4String.NumericValue * 1.11M))
                    Profit2YearsAgoColor = Color.Lime;
                else
                    Profit2YearsAgoColor = _normalColor;
            }
                
            // Set Colors for Basic EPS labels (if EPS decreasing by 10% every 2 years, a problem
            if (BasicEpsTtmString.NumericValue > 0) 
            {   // TTM
                if (BasicEpsTtmString.NumericValue < BasicEps2String.NumericValue * .9M)
                    BasicEpsTtmColor = Color.Red;
                else if (BasicEpsTtmString.NumericValue > (BasicEps2String.NumericValue * 1.11M))
                    BasicEpsTtmColor = Color.Lime;
                else
                    BasicEpsTtmColor = _normalColor;
            }
            else
            {
                if ((BasicEpsTtmString.NumericValue + 10) < (BasicEps2String.NumericValue + 10) * .96M)
                    BasicEpsTtmColor = Color.Red;
                else if ((BasicEpsTtmString.NumericValue + 10) > (BasicEps2String.NumericValue + 10) * 1.04M)
                    BasicEpsTtmColor = Color.Lime;
                else
                    BasicEpsTtmColor = _normalColor;
            }
            if (BasicEps2String.NumericValue > 0)
            {   
                // 2 years ago
                if (BasicEps2String.NumericValue < (BasicEps4String.NumericValue * .9M))
                    BasicEps2Color = Color.Red;
                else if (BasicEps2String.NumericValue > (BasicEps4String.NumericValue * 1.11M))
                    BasicEps2Color = Color.Lime;
                else
                    BasicEps2Color = _normalColor;
            }
            else
            {
                if ((BasicEps2String.NumericValue + 10) < (BasicEps4String.NumericValue + 10) * .96M)
                    BasicEps2Color = Color.Red;
                else if ((BasicEps2String.NumericValue + 10) > (BasicEps4String.NumericValue + 10) * 1.04M)
                    BasicEps2Color = Color.Lime;
                else
                    BasicEps2Color = _normalColor;
            }

            // Forward PE coloring is complex. It takes into account
            // 1. Average PE for the sector
            // 2. How large the profits are. We can use current profit margin. >15% is a high profit margin. -15% is a bad profit margin.
            // 3. How fast profits are growing/decreasing. (Current profit / Prior Profit)
            // TODO
            ProfitGrowth = 1; // SetYearOverYearTrend(Profit4String, Profit2String, ProfitTtmString, 0);

            return true;
        }

        /// /////////////////////////////////////////////////////////////////////////////////////////
        ///                                    SQL Support
        /// /////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckSqlForRecentData()
        {
            SqlCrudOperations sqlFinancialStatement = new SqlCrudOperations();
            List<SqlIncomeStatement> entities = sqlFinancialStatement.GetIncomeStatementList(Ticker);
            Random random = new Random();

            // Generate a random integer between 1 (inclusive) and 4 (exclusive).
            // This means the possible results are 1, 2, or 3.
            int randomNumber = random.Next(1, 4);

            if (entities.Count > 0)
            {
                DateTime staleDate = DateTime.Now.Date.AddDays(-12 + random.Next(1, 4));

                if (entities[0].UpdateDate > staleDate) // We have recent data in the database, use it.
                {
                    MapFill(entities);
                    return true;
                }

                return false; // need to go out and download new data
            }

            return false;
        }

        public static List<SqlIncomeStatement> MapFrom(StockIncomeStatement source)
        {
            List<SqlIncomeStatement> sqlIncomeStatements = new List<SqlIncomeStatement>();

            // TTM
            sqlIncomeStatements.Add(new SqlIncomeStatement()
            {
                Ticker = source.Ticker,
                Year = DateTime.Now.Year,
                Revenue = (double)source.RevenueTtmString.NumericValue,
                CostOfRevenue = (double)source.CostOfRevenueTtmString.NumericValue,
                OperatingExpense = (double)source.OperatingExpenseTtmString.NumericValue,
                NetIncome = (double)source.NetIncomeTtmString.NumericValue,
                BasicEPS = (double)source.BasicEpsTtmString.NumericValue,
                UpdateDate = DateTime.Now
            });

            // 2 years ago
            sqlIncomeStatements.Add(new SqlIncomeStatement()
            {
                Ticker = source.Ticker,
                Year = DateTime.Now.AddYears(-2).Year,
                Revenue = (double)source.Revenue2String.NumericValue,
                CostOfRevenue = (double)source.CostOfRevenue2String.NumericValue,
                OperatingExpense = (double)source.OperatingExpense2String.NumericValue,
                NetIncome = (double)source.NetIncome2String.NumericValue,
                BasicEPS = (double)source.BasicEps2String.NumericValue,
                UpdateDate = DateTime.Now
            });

            // 4 years ago
            sqlIncomeStatements.Add(new SqlIncomeStatement()
            {
                Ticker = source.Ticker,
                Year = DateTime.Now.AddYears(-4).Year,
                Revenue = (double)source.Revenue4String.NumericValue,
                CostOfRevenue = (double)source.CostOfRevenue4String.NumericValue,
                OperatingExpense = (double)source.OperatingExpense4String.NumericValue,
                NetIncome = (double)source.NetIncome4String.NumericValue,
                BasicEPS = (double)source.BasicEps4String.NumericValue,
                UpdateDate = DateTime.Now
            });

            return sqlIncomeStatements;
        }

        public void MapFill(List<SqlIncomeStatement> sourceList)
        {
            sourceList = sourceList.OrderBy(x => x.Year).ToList();

            // 4 years ago
            Revenue4String.NumericValue = (decimal)sourceList[0].Revenue;
            CostOfRevenue4String.NumericValue = (decimal)sourceList[0].CostOfRevenue;
            OperatingExpense4String.NumericValue = (decimal)sourceList[0].OperatingExpense;
            NetIncome4String.NumericValue = (decimal)sourceList[0].NetIncome;
            BasicEps4String.NumericValue = (decimal)sourceList[0].BasicEPS;

            // 2 years ago
            Revenue2String.NumericValue = (decimal)sourceList[1].Revenue;
            CostOfRevenue2String.NumericValue = (decimal)sourceList[1].CostOfRevenue;
            OperatingExpense2String.NumericValue = (decimal)sourceList[1].OperatingExpense;
            NetIncome2String.NumericValue = (decimal)sourceList[1].NetIncome;
            BasicEps2String.NumericValue = (decimal)sourceList[1].BasicEPS;

            // TTM
            RevenueTtmString.NumericValue = (decimal)sourceList[2].Revenue;
            CostOfRevenueTtmString.NumericValue = (decimal)sourceList[2].CostOfRevenue;
            OperatingExpenseTtmString.NumericValue = (decimal)sourceList[2].OperatingExpense;
            NetIncomeTtmString.NumericValue = (decimal)sourceList[2].NetIncome;
            BasicEpsTtmString.NumericValue = (decimal)sourceList[2].BasicEPS;

            return;
        }
    }
}
