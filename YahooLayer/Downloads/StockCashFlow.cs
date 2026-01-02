using log4net;
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
    public class StockCashFlow : YahooFinanceBase
    {
        private static readonly string _financialsUrl = "https://finance.yahoo.com/quote/???/cash-flow/";

        ////////////////////////////////////////////
        ///                Properties
        ////////////////////////////////////////////
        /// Free Cash Flow
        public StringSafeType<Decimal> FreeCashFlowTtmString = new StringSafeType<decimal>("--", "N0");
        public Color FreeCashFlowTtmColor = Color.White;
        public StringSafeType<Decimal> FreeCashFlow2String = new StringSafeType<decimal>("--", "N0");
        public Color FreeCashFlow2Color = Color.White;
        public StringSafeType<Decimal> FreeCashFlow4String = new StringSafeType<decimal>("--", "N0");
        public Color FreeCashFlow4Color = Color.White;

        /// Basic EPS
        public StringSafeType<Decimal> OperatingCashFlowTtmString = new StringSafeType<decimal>("--");
        public Color OperatingCashFlowTtmColor = Color.White;
        public StringSafeType<Decimal> OperatingCashFlow2String = new StringSafeType<decimal>("--");
        public Color OperatingCashFlow2Color = Color.White; 
        public StringSafeType<Decimal> OperatingCashFlow4String = new StringSafeType<decimal>("--");
        public Color OperatingCashFlow4Color = Color.White; 

        /// End Cash Position 
        public StringSafeType<Decimal> EndCashPositionTtmString = new StringSafeType<decimal>("--", "N0");
        public Color EndCashPositionTtmColor = Color.White;
        public StringSafeType<Decimal> EndCashPosition2String = new StringSafeType<decimal>("--", "N0");
        public Color EndCashPosition2Color = Color.White;
        public StringSafeType<Decimal> EndCashPosition4String = new StringSafeType<decimal>("--", "N0");
        public Color EndCashPosition4Color = Color.White;

        public StockCashFlow()
        {
            FreeCashFlowTtmColor = _normalColor;
            FreeCashFlow2Color = _normalColor;
            FreeCashFlow4Color = _normalColor;
            OperatingCashFlowTtmColor = _normalColor;
            OperatingCashFlow2Color = _normalColor;
            OperatingCashFlow4Color = _normalColor;
            EndCashPositionTtmColor = _normalColor;
            EndCashPosition2Color = _normalColor;
            EndCashPosition4Color = _normalColor;
        }

        ////////////////////////////////////////////
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
                    Debug.WriteLine("GetStockCashFlow()");

                    //// Free Cash Flow
                    if (ParseHtmlRowData(html, "Free Cash Flow", FreeCashFlowTtmString, FreeCashFlow2String, FreeCashFlow4String) == false)
                        return false; //=====>>>>>>> Exit

                    //// Operating Cash Flow                                  
                    if (ParseHtmlRowData(html, "Operating Cash Flow", OperatingCashFlowTtmString, OperatingCashFlow2String, OperatingCashFlow4String) == false)
                    {
                        OperatingCashFlowTtmString.StringValue = OperatingCashFlow2String.StringValue = OperatingCashFlow4String.StringValue = "--";
                        return false; //=====>>>>>>> Exit
                    }

                    //// End Cash Position
                    if (ParseHtmlRowData(html, "End Cash Position", EndCashPositionTtmString, EndCashPosition2String, EndCashPosition4String) == false)
                    {
                        EndCashPositionTtmString.StringValue = EndCashPosition2String.StringValue = EndCashPosition4String.StringValue = "--";
                        return false; //=====>>>>>>> Exit
                    }

                    ///////////////////////////////////
                    ///      Save to SQL Server
                    List<SqlCashFlow> rows = MapFrom(this);
                    SqlCrudOperations _finacialStatement = new SqlCrudOperations();
                    _finacialStatement.SaveCashFlows(rows);
                }
                catch (Exception ex)
                {
                    //Program.logger.Error($"******ERROR**********\r\n   {ex.StackTrace}\r\n   {ex.StackTrace}\r\n");

                    logger.Error($"******ERROR**********\r\n   {ex.StackTrace}\r\n   {ex.StackTrace}\r\n");
                    //MessageBox.Show(x.Source + x.Message + "\n" + "GetIncomeStatementData() " + " " + ticker + "\n" + html.Substring(0, html.Length / 10));
                }
            }

            ///////////////////////////////////////////////////
            //                   Set Colors
            // Set Colors of Revenue (if revenue decreasing by 5% every 2 years, a problem
            if (FreeCashFlowTtmString.NumericValue < (FreeCashFlow2String.NumericValue * .95M))
                FreeCashFlowTtmColor = Color.Red;
            else if (FreeCashFlowTtmString.NumericValue > (FreeCashFlow2String.NumericValue * 1.05M))
                FreeCashFlowTtmColor = Color.Lime;
            else
                FreeCashFlowTtmColor = _normalColor;

            if (FreeCashFlow2String.NumericValue < (FreeCashFlow4String.NumericValue * .95M))
                FreeCashFlow2Color = Color.Red;
            else if (FreeCashFlow2String.NumericValue > (FreeCashFlow4String.NumericValue * 1.05M))
                FreeCashFlow2Color = Color.Lime;
            else
                FreeCashFlow2Color = _normalColor;

            // Set Colors for EndCashPositions labels (if profit decreasing by 10% every 2 years, a problem
            if (FreeCashFlowTtmString.NumericValue > 0) // && CostOfRevenueTtmString.NumericValue > 0 && CostOfRevenue4String.NumericValue > 0 <-- Why was this in there?
            {
                if (EndCashPositionTtmString.NumericValue < EndCashPosition2String.NumericValue * .9M)
                    EndCashPositionTtmColor = Color.Red;
                else if (EndCashPositionTtmString.NumericValue > (EndCashPosition2String.NumericValue * 1.11M))
                    EndCashPositionTtmColor = Color.Lime;
                else
                    EndCashPositionTtmColor = _normalColor;

                if (EndCashPosition2String.NumericValue < (EndCashPosition4String.NumericValue * .9M))
                    EndCashPosition2Color = Color.Red;
                else if (EndCashPosition2String.NumericValue > (EndCashPosition4String.NumericValue * 1.11M))
                    EndCashPosition2Color = Color.Lime;
                else
                    EndCashPosition2Color = _normalColor;
            }

            // Set Colors for Basic EPS labels (if EPS decreasing by 10% every 2 years, a problem
            if (OperatingCashFlowTtmString.NumericValue > 0)
            {   // TTM
                if (OperatingCashFlowTtmString.NumericValue < OperatingCashFlow2String.NumericValue * .9M)
                    OperatingCashFlowTtmColor = Color.Red;
                else if (OperatingCashFlowTtmString.NumericValue > (OperatingCashFlow2String.NumericValue * 1.11M))
                    OperatingCashFlowTtmColor = Color.Lime;
                else
                    OperatingCashFlowTtmColor = _normalColor;
            }
            else
            {
                if ((OperatingCashFlowTtmString.NumericValue + 10) < (OperatingCashFlow2String.NumericValue + 10) * .96M)
                    OperatingCashFlowTtmColor = Color.Red;
                else if ((OperatingCashFlowTtmString.NumericValue + 10) > (OperatingCashFlow2String.NumericValue + 10) * 1.04M)
                    OperatingCashFlowTtmColor = Color.Lime;
                else
                    OperatingCashFlowTtmColor = _normalColor;
            }
            if (OperatingCashFlow2String.NumericValue > 0)
            {
                // 2 years ago
                if (OperatingCashFlow2String.NumericValue < (OperatingCashFlow4String.NumericValue * .9M))
                    OperatingCashFlow2Color = Color.Red;
                else if (OperatingCashFlow2String.NumericValue > (OperatingCashFlow4String.NumericValue * 1.11M))
                    OperatingCashFlow2Color = Color.Lime;
                else
                    OperatingCashFlow2Color = _normalColor;
            }
            else
            {
                if ((OperatingCashFlow2String.NumericValue + 10) < (OperatingCashFlow4String.NumericValue + 10) * .96M)
                    OperatingCashFlow2Color = Color.Red;
                else if ((OperatingCashFlow2String.NumericValue + 10) > (OperatingCashFlow4String.NumericValue + 10) * 1.04M)
                    OperatingCashFlow2Color = Color.Lime;
                else
                    OperatingCashFlow2Color = _normalColor;
            }

            return true;
        }

        /// /////////////////////////////////////////////////////////////////////////////////////////
        ///                                    SQL Support
        /// /////////////////////////////////////////////////////////////////////////////////////////
        public bool CheckSqlForRecentData()
        {
            SqlCrudOperations sqlFinancialStatement = new SqlCrudOperations();
            List<SqlCashFlow> entities = sqlFinancialStatement.GetCashFlowList(Ticker);
            Random random = new Random();

            // Generate a random integer between 1 (inclusive) and 4 (exclusive).
            // This means the possible results are 1, 2, or 3.
            int randomNumber = random.Next(1, 4);

            if (entities.Count > 2)
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

        public static List<SqlCashFlow> MapFrom(StockCashFlow source)
        {
            List<SqlCashFlow> sqlIncomeStatements = new List<SqlCashFlow>();

            // TTM
            sqlIncomeStatements.Add(new SqlCashFlow()
            {
                Ticker = source.Ticker,
                Year = DateTime.Now.Year,
                FreeCashFlow = (double)source.FreeCashFlowTtmString.NumericValue,
                OperatingCashFlow = (double)source.OperatingCashFlowTtmString.NumericValue,
                EndCashPosition = (double)source.EndCashPositionTtmString.NumericValue,
                UpdateDate = DateTime.Now
            });

            // 2 years ago
            sqlIncomeStatements.Add(new SqlCashFlow()
            {
                Ticker = source.Ticker,
                Year = DateTime.Now.AddYears(-2).Year,
                FreeCashFlow = (double)source.FreeCashFlow2String.NumericValue,
                OperatingCashFlow = (double)source.OperatingCashFlow2String.NumericValue,
                EndCashPosition = (double)source.EndCashPosition2String.NumericValue,
                UpdateDate = DateTime.Now
            });

            // 4 years ago
            sqlIncomeStatements.Add(new SqlCashFlow()
            {
                Ticker = source.Ticker,
                Year = DateTime.Now.AddYears(-4).Year,
                FreeCashFlow = (double)source.FreeCashFlow4String.NumericValue,
                OperatingCashFlow = (double)source.OperatingCashFlow4String.NumericValue,
                EndCashPosition = (double)source.EndCashPosition4String.NumericValue,
                UpdateDate = DateTime.Now
            });

            return sqlIncomeStatements;
        }

        public void MapFill(List<SqlCashFlow> sourceList)
        {
            sourceList = sourceList.OrderBy(x => x.Year).ToList();

            //// 4 years ago
            FreeCashFlow4String.NumericValue = (decimal)sourceList[0].FreeCashFlow;
            OperatingCashFlow4String.NumericValue = (decimal)sourceList[0].OperatingCashFlow;
            EndCashPosition4String.NumericValue = (decimal)sourceList[0].EndCashPosition;

            //// 2 years ago
            FreeCashFlow2String.NumericValue = (decimal)sourceList[1].FreeCashFlow;
            OperatingCashFlow2String.NumericValue = (decimal)sourceList[1].OperatingCashFlow;
            EndCashPosition2String.NumericValue = (decimal)sourceList[1].EndCashPosition;

            //// TTM
            FreeCashFlowTtmString.NumericValue = (decimal)sourceList[2].FreeCashFlow;
            OperatingCashFlowTtmString.NumericValue = (decimal)sourceList[2].OperatingCashFlow;
            EndCashPositionTtmString.NumericValue = (decimal)sourceList[2].EndCashPosition;

            return;
        }
    }
}
