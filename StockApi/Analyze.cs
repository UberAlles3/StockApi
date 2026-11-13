using SqlLayer;
using SqlLayer.SQL_Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using YahooLayer;

namespace StockApi
{
    public class Analyze : YahooFinanceBase
    {
        public enum BuyOrSell
        {
            Buy,
            Sell
        }

        private bool _buyless = false;

        public int BuyQuantity;
        public decimal BuyPrice;
        public int SellQuantity;
        public decimal SellPrice = 0;
        public string AnalysisMetricsOutputText;
        private static string _ticker = "";

        // Public Metric Values
        public decimal PriceTrendMetric = 1M;
        public decimal TargetPriceMetric = 1M;
        public decimal EarningsPerShareMetric = 1M;
        public decimal PriceBookMetric = 1M;
        public decimal ProfitMarginMetric = 1.00M;
        public decimal DividendMetric = 1M;
        public decimal RevenueMetric = 1M;
        public decimal ProfitMetric = 1M;
        public decimal BasicEpsMetric = 1M;
        public decimal CashDebtMetric = 1M;
        public decimal PegRatioMetric = 1M;
        public decimal OperatingCashFlowMetric = 1M;
        public decimal FreeCashFlowMetric = 1M;
        public decimal EndCashMetric = 1M;
        public decimal FinalCashFlowMetric;
        public decimal ValuationMetric = 1M;
        public decimal FinalMetric = 0;

        public decimal AnalyzeStockData(StockDownloads stockDownloads, AnalyzeInputs analyzeInputs, bool forMetricOnly)
        {
            _buyless = false;
            _ticker = stockDownloads.stockSummary.Ticker;
            StringBuilder output = new StringBuilder();

            Debug.WriteLine(_ticker);

            //////////////////////////////////////////////////////////////////////////////////
            ///                            Summary Metrics
            
            ///////// Long Term Price Trend
            PriceTrendMetric = stockDownloads.stockHistory.HistoricDataToday.Price / stockDownloads.stockHistory.HistoricData3YearsAgo.Price;
            PriceTrendMetric = SoftLimit(PriceTrendMetric, .5M, 2M);
            PriceTrendMetric = AdjustMetric(PriceTrendMetric, -18M);

            if (stockDownloads.stockStatistics.ForwardPEString.NumericValue > 100) // Color.FromArgb(205, 232, 202);Overvalued stocks get downgraded
                PriceTrendMetric = PriceTrendMetric - .01M;


            ////////// One Year Target - Not a very valuable metric. 
            TargetPriceMetric = stockDownloads.stockSummary.OneYearTargetPriceString.NumericValue / stockDownloads.stockSummary.PriceString.NumericValue;
            TargetPriceMetric = AdjustMetric(TargetPriceMetric, -10M);
            TargetPriceMetric = SoftLimit(TargetPriceMetric, .9M, 1.01M);

            ////////// Earnings Per Share
            EarningsPerShareMetric = AdjustMetric((1 + (stockDownloads.stockSummary.EarningsPerShareString.NumericValue / 100M)), -1.2M);
            EarningsPerShareMetric = HardLimit(EarningsPerShareMetric, .97M, 1.06M);

            ////////// Price / Book
            if (stockDownloads.stockStatistics.PriceBookString.NumericValue > 5)
                PriceBookMetric = .99M;
            else if (stockDownloads.stockStatistics.PriceBookString.NumericValue < 1)
                PriceBookMetric = 1.01M;

            ////////// Profit Margin Metric
            ProfitMarginMetric = ((stockDownloads.stockSummary.ProfitMarginString.NumericValue + 280) / 280);
            ProfitMarginMetric = AdjustMetric(ProfitMarginMetric, -1);
            ProfitMarginMetric = SoftLimit(ProfitMarginMetric, .97M, 1.035M);
            ProfitMarginMetric = SoftLimit(ProfitMarginMetric, .96M, 1.045M);

            /////////// Dividend Metric
            DividendMetric = ((stockDownloads.stockSummary.DividendString.NumericValue + 120M) / 120M) - .005M;
            DividendMetric = SoftLimit(DividendMetric, .97M, 1.07M);
            DividendMetric = SoftLimit(DividendMetric, .97M, 1.08M);

            if (PriceTrendMetric < .92M && DividendMetric > 1.04M)
                DividendMetric = (1M + DividendMetric) / 2M; // if the price is going steeply down, who cares about a high dividend

            //////////////////////////////////////////////////////////////////////////////////
            ///                            Income Statement Metrics



            if (stockDownloads.stockSummary.sqlTicker.IsFund == false && stockDownloads.stockSummary.sqlTicker.IsPreRevenueCompany == false) // some type of stocks don't have income statements
            {
                /////////// Revenue 
                RevenueMetric = SetYearOverYearTrend(stockDownloads.stockIncomeStatement.Revenue4String, stockDownloads.stockIncomeStatement.Revenue2String, stockDownloads.stockIncomeStatement.RevenueTtmString, 0);

                /////////// Profit - Revenue - Cost of Revenue
                ProfitMetric = SetYearOverYearTrend(stockDownloads.stockIncomeStatement.Profit4String, stockDownloads.stockIncomeStatement.Profit2String, stockDownloads.stockIncomeStatement.ProfitTtmString, 0);

                /////////// Basic earnings per share growth
                BasicEpsMetric = SetYearOverYearTrend(stockDownloads.stockIncomeStatement.BasicEps4String, stockDownloads.stockIncomeStatement.BasicEps2String, stockDownloads.stockIncomeStatement.BasicEpsTtmString, -2);
                BasicEpsMetric = SoftLimit(BasicEpsMetric, .96M, 1.054M);
            }

            // All negative earnings, downgrade the earnings metric
            if (stockDownloads.stockIncomeStatement.BasicEpsTtmString.NumericValue < 0 && stockDownloads.stockIncomeStatement.BasicEps2String.NumericValue < 0 && stockDownloads.stockIncomeStatement.BasicEps4String.NumericValue < 0 && BasicEpsMetric > 1.02M)
                BasicEpsMetric -= .02M;

            //////////////////////////////////////////////////////////////////////////////////
            ///                            Statistics Metrics
            /////////// Cash Debt
            if (stockDownloads.stockStatistics.TotalDebt > stockDownloads.stockStatistics.TotalCash * 5) // lots of debt compared to cash
                CashDebtMetric = .97M;
            else if (stockDownloads.stockStatistics.TotalCash > stockDownloads.stockStatistics.TotalDebt * 2) // lots of cash compared to debt
                CashDebtMetric = 1.03M;

            /////////// Peg Ratio
            if(stockDownloads.stockStatistics.PegRatioString.NumericValue != 0)
            {
                if (stockDownloads.stockStatistics.PegRatioString.NumericValue > 2.8M)
                    PegRatioMetric = .986M; // Much Overvalued
                else if (stockDownloads.stockStatistics.PegRatioString.NumericValue > 1.9M)
                    PegRatioMetric = .992M; // Overvalued
                else if (stockDownloads.stockStatistics.PegRatioString.NumericValue < .32M)
                    PegRatioMetric = 1.013M; // Much Undervalued
                else if (stockDownloads.stockStatistics.PegRatioString.NumericValue < .68M)
                    PegRatioMetric = 1.01M; // Undervalued
            }

            if (stockDownloads.stockStatistics.DebtEquityString.NumericValue > 130) // Over 120% D/E is bad
                CashDebtMetric = CashDebtMetric * .97M;
            else if (stockDownloads.stockStatistics.DebtEquityString.NumericValue > 200) // Over 120% D/E is bad
                CashDebtMetric = CashDebtMetric * .96M;


            //////////////////////////////////////////////////////////////////////////////////
            ///                             Cash Flow Metrics
            ////////// Operationg Cash Flow

            if (stockDownloads.stockSummary.sqlTicker.IsFund == false) // fund or ETFs don't have cash flow data
            {
                OperatingCashFlowMetric = SetYearOverYearTrend(stockDownloads.stockCashFlow.OperatingCashFlow4String, stockDownloads.stockCashFlow.OperatingCashFlow2String, stockDownloads.stockCashFlow.OperatingCashFlowTtmString, -2);

                ////////// Free Cash Flow
                FreeCashFlowMetric = SetYearOverYearTrend(stockDownloads.stockCashFlow.FreeCashFlow4String, stockDownloads.stockCashFlow.FreeCashFlow2String, stockDownloads.stockCashFlow.FreeCashFlowTtmString, -2);

                /////////// End Cash Position
                EndCashMetric = SetYearOverYearTrend(stockDownloads.stockCashFlow.EndCashPosition4String, stockDownloads.stockCashFlow.EndCashPosition2String, stockDownloads.stockCashFlow.EndCashPositionTtmString, -2);
            }

            FinalCashFlowMetric = (OperatingCashFlowMetric + FreeCashFlowMetric + EndCashMetric) / 3;

            ////////// Extra ratio calculations. Adjusting finalCashFlowMetric 
            decimal FcfRatio = 1;
            if (stockDownloads.stockIncomeStatement.NetIncomeTtmString.NumericValue > 1000)
            {
                FcfRatio = stockDownloads.stockCashFlow.FreeCashFlowTtmString.NumericValue / stockDownloads.stockIncomeStatement.NetIncomeTtmString.NumericValue;
                if (FcfRatio > 1.05M) FinalCashFlowMetric = FinalCashFlowMetric * 1.01M;
                else if (FcfRatio > .9M) FinalCashFlowMetric = FinalCashFlowMetric * 1.005M;
                else if (FcfRatio < .3M) FinalCashFlowMetric = FinalCashFlowMetric * 0.99M;
                else if (FcfRatio < .6M) FinalCashFlowMetric = FinalCashFlowMetric * 0.995M;
            }
            else
            {
                if (stockDownloads.stockIncomeStatement.NetIncomeTtmString.NumericValue < -10000)
                {
                    FinalCashFlowMetric = (decimal)AdjustMetric((double)FinalCashFlowMetric, -4); // less impact
                }
            }

            ////////// Cash Ratio
            decimal cashRatio = 1; // How long cash can last, burn thru, for operating expenses
            if (stockDownloads.stockCashFlow.EndCashPositionTtmString.NumericValue > 0 && stockDownloads.stockIncomeStatement.OperatingExpenseTtmString.NumericValue > 0)
                cashRatio = stockDownloads.stockCashFlow.EndCashPositionTtmString.NumericValue / stockDownloads.stockIncomeStatement.OperatingExpenseTtmString.NumericValue;

            if (cashRatio > 2M)
                FinalCashFlowMetric = FinalCashFlowMetric * 1.006M;
            if (cashRatio < .4M)
                FinalCashFlowMetric = FinalCashFlowMetric * .994M;

            FinalCashFlowMetric = Decimal.Round(FinalCashFlowMetric, 3);

            //////////////////////////////////////////
            ///   Valuation based on PE and sector
            if (stockDownloads.stockSummary.Valuation == StockSummary.ValuationEnum.OverValued)
                ValuationMetric = .98M;
            else if (stockDownloads.stockSummary.Valuation == StockSummary.ValuationEnum.UnderValued)
                ValuationMetric = 1.02M;

            if (stockDownloads.stockStatistics.CalculatedPEString.NumericValue > 0 && stockDownloads.stockStatistics.CalculatedPEString.NumericValue > (decimal)stockDownloads.stockSummary.AverageSectorPE * 1.8M) // Over valued
                ValuationMetric = .975M;

            if (stockDownloads.stockStatistics.CalculatedPEString.NumericValue > 0 && stockDownloads.stockStatistics.CalculatedPEString.NumericValue < (decimal)stockDownloads.stockSummary.AverageSectorPE * .6M) // Under valued
                ValuationMetric = 1.025M;

            //// Calculate total metric
            FinalMetric = PriceTrendMetric * EarningsPerShareMetric * ((TargetPriceMetric + PriceBookMetric) / 2) *
                    DividendMetric * ProfitMarginMetric * RevenueMetric * ((ProfitMetric + BasicEpsMetric) / 2) *
                    CashDebtMetric * ValuationMetric * FinalCashFlowMetric * PegRatioMetric;

            FinalMetric = Decimal.Round(FinalMetric, 3);

            ///////////////////////////////////
            ///      Save to SQL Server
            SqlMetric sqlMetric = new SqlMetric();
            sqlMetric.BasicEps     = (double)BasicEpsMetric;
            sqlMetric.CashDebt     = (double)CashDebtMetric;
            sqlMetric.PegRatio     = (double)PegRatioMetric;
            sqlMetric.CashFlow     = (double)FinalCashFlowMetric;
            sqlMetric.Dividend     = (double)DividendMetric;
            sqlMetric.EarningsPerShare = (double)EarningsPerShareMetric;
            sqlMetric.FinalMetric  = (double)FinalMetric;
            sqlMetric.Month        = DateTime.Now.Month;
            sqlMetric.PriceBook    = (double)PriceBookMetric;
            sqlMetric.PriceTrend   = (double)PriceTrendMetric;
            sqlMetric.Profit       = (double)ProfitMetric;
            sqlMetric.ProfitMargin = (double)ProfitMarginMetric;
            sqlMetric.Revenue      = (double)RevenueMetric;
            sqlMetric.Ticker       = _ticker;
            sqlMetric.TargetPrice  = (double)TargetPriceMetric;
            sqlMetric.UpdateDate   = DateTime.Now;
            sqlMetric.Valuation    = (double)ValuationMetric;
            sqlMetric.Year         = DateTime.Now.Year;

            SqlCrudOperations _finacialStatement = new SqlCrudOperations();
            _finacialStatement.SaveMetrics(sqlMetric);

            FinalMetric = Math.Round(FinalMetric, 2);
            if (forMetricOnly == true)
            {
                AnalysisMetricsOutputText = output.ToString();
                return FinalMetric; //========================>>>>>>>>>>>>>>>>>>>>  Get out
            }

            ///////////////////////////////////////////////////////////////
            ///                     BUY and SELL
            ///////////////////////////////////////////////////////////////

            // Consecutive buys or sells
            decimal buySellMetric = 1M;
            if (forMetricOnly == false)
            {
                List<DataRow> drList = Form1.TickerTradesDataTable.Select().Take(3).ToList();
                int buys = 0;
                int sells = 0;
                foreach (DataRow dr in drList)
                {
                    if (dr.ItemArray[2].ToString() == "Buy")
                    {
                        if (sells > 0)
                            break;
                        buys++;
                    }
                    if (dr.ItemArray[2].ToString() == "Sell")
                    {
                        if (buys > 0)
                            break;
                        sells++;
                    }
                }

                if (buys == 2)
                    buySellMetric = .98M;
                if (buys == 3)
                    buySellMetric = .96M;
                if (sells == 2)
                    buySellMetric = 1.02M;
                if (sells == 3)
                    buySellMetric = 1.04M;
            }

            output.AppendLine($"Buys Sells Metric = {buySellMetric.ToString(".00")}");

            // Adjust based on series of buys or sells
            FinalMetric = FinalMetric * buySellMetric;

            // Gets the volatility number closer to 1, less exxtreme. 2.6 becomes 1.5
            decimal volitilityFactor = 1; // Math.Log((Math.Log10(stockDownloads.stockSummary.Volatility) + 1)) + 1; 

            volitilityFactor = (decimal)AdjustMetric((double)stockDownloads.stockSummary.VolatilityString.NumericValue, -6D) - .01M;

            output.AppendLine($"Volitility Factor = { volitilityFactor.ToString("##.##")}");

            analyzeInputs.MovementTargetPercent *= volitilityFactor; // Applying volatility
            output.AppendLine($"Movement % w/ Volatility = { analyzeInputs.MovementTargetPercent.ToString("##.##")}%");

            // if stock is like XOM where your not sell a large percentage of shares owned
            // Make the lower and upper movement less
            if (analyzeInputs.QuantityTraded < analyzeInputs.SharesOwned / 4)
            {
                analyzeInputs.MovementTargetPercent = analyzeInputs.MovementTargetPercent * .9M;
                output.AppendLine($"Movement reduced, low share quantity = {analyzeInputs.MovementTargetPercent.ToString("##.##")}");
            }

            decimal lowerMovementMultiplier = ((100M - (analyzeInputs.MovementTargetPercent * .7M)) / 100M); // if movement is 20% will assign .88 because a down percentage is greater than an up
            decimal upperMovementMultiplier = ((100M + analyzeInputs.MovementTargetPercent) / 100M); // if movement is 20% will assign 1.2

            // Calculate future buy or sells based on how much it moves and what our target movement is. 
            decimal buyPrice = analyzeInputs.SharesTradedPrice * lowerMovementMultiplier;
            decimal sellPrice = analyzeInputs.SharesTradedPrice * upperMovementMultiplier;

            output.AppendLine($"Buy price  applying movement% = {buyPrice.ToString("##.##")}");
            output.AppendLine($"Sell price applying movement% = {sellPrice.ToString("##.##")}");

            buyPrice = buyPrice * ((FinalMetric + 1) / 2);
            sellPrice = sellPrice * ((FinalMetric + 1) / 2);

            if (sellPrice < analyzeInputs.SharesTradedPrice)
                sellPrice = analyzeInputs.SharesTradedPrice * 1.05M; // Sell a bad stock at a 5% profit if it ever gets there.
            if (buyPrice > analyzeInputs.SharesTradedPrice * .88M)
            {
                _buyless = true;
                buyPrice = analyzeInputs.SharesTradedPrice * .88M; // Next buy can't so near the current price, but buy less
            }

            output.AppendLine($"Buy price  applying total metric = {buyPrice.ToString("##.##")}");
            output.AppendLine($"Sell price applying total metric = {sellPrice.ToString("##.##")}");

            ///////// Limit price so it's with a range
            decimal limitPrice = (stockDownloads.stockSummary.PriceString.NumericValue * lowerMovementMultiplier) * .5M; // lower buy limit price too low
            if (buyPrice < limitPrice)
                buyPrice = limitPrice;
            limitPrice = (stockDownloads.stockSummary.PriceString.NumericValue * upperMovementMultiplier) * 1.4M; // upper sell limit
            if (sellPrice > limitPrice)
                sellPrice = limitPrice;

            BuyPrice = buyPrice;
            SellPrice = sellPrice;


            //////////////////////////////////
            //      Quantity to Buy/Sell
            //////////////////////////////////
            decimal buyQuantity = 0M;
            decimal sellQuantity = 0M;

            if (analyzeInputs.LastTradeBuySell == BuyOrSell.Buy)
            {
                buyQuantity = analyzeInputs.QuantityTraded * .8M; // Buy less if you just bought
                sellQuantity = analyzeInputs.QuantityTraded * 1.08M; // Sell more if you just bought
            }
            else
            {
                buyQuantity = analyzeInputs.QuantityTraded * 1.08M; // Buy more if you just sold
                sellQuantity = analyzeInputs.QuantityTraded * .8M; // Sell less if you just sold
            }

            // Dividend 
            if (stockDownloads.stockSummary.DividendString.NumericValue > 8 && PriceTrendMetric > .92M)
            {
                buyQuantity = buyQuantity * 1.2M;
                sellQuantity = sellQuantity / 1.2M;
                output.AppendLine($"Great dividend. Buy more or sell less. {buyQuantity.ToString("##.##")} {sellQuantity.ToString("##.##")}");
            }
            else if (stockDownloads.stockSummary.DividendString.NumericValue > 4)
            {
                buyQuantity = buyQuantity * 1.1M;
                sellQuantity = sellQuantity / 1.1M;
            }
            else if (stockDownloads.stockSummary.DividendString.NumericValue > 2)
            {
                buyQuantity = buyQuantity * 1.05M;
                sellQuantity = sellQuantity / 1.05M;
            }

            if (_buyless)
            {
                buyQuantity = buyQuantity * .8M;
                output.AppendLine($"Accumulate this stock slowly.");
            }

            // Year price trend. Lower and Raise based on that.
            if (stockDownloads.stockHistory.YearTrend == StockHistory.TrendEnum.Up)
            {
                buyQuantity = buyQuantity * 1.05M;
                sellQuantity = sellQuantity / 1.05M;
                output.AppendLine($"Year up trend applied. {buyQuantity.ToString("##.##")} {sellQuantity.ToString("##.##")}");
            }
            if (stockDownloads.stockHistory.YearTrend == StockHistory.TrendEnum.Down)
            {
                buyQuantity = buyQuantity * .95M;
                sellQuantity = sellQuantity / .95M;
                output.AppendLine($"Year down trend applied. {buyQuantity.ToString("##.##")} {sellQuantity.ToString("##.##")}");
            }

            BuyQuantity = Math.Max(1, Convert.ToInt32(buyQuantity));
            SellQuantity = Math.Max(1, Convert.ToInt32(sellQuantity));

            if (SellQuantity > 1 && SellQuantity > Convert.ToInt16(analyzeInputs.SharesOwned / 2.4F)) // Don't sell close to more than half your shares
                SellQuantity = Convert.ToInt16((analyzeInputs.SharesOwned / 2.4F) + .5F);

            // Minimum profit of $30. if selling 10 shares, sell at least $3 increase in price.
            decimal profit = (SellPrice - stockDownloads.stockSummary.PriceString.NumericValue) * SellQuantity;
            if (profit < 20M && FinalMetric > .91M) // if it's a bad stock we are liquidating and the $20 profit doesn't matter. 
            {
                SellPrice = (20M / (SellQuantity + 3)) + stockDownloads.stockSummary.PriceString.NumericValue;
                output.AppendLine($"Minimum profit set to $20. Sell Price: {SellPrice.ToString("##.##")}");
            }

            AnalysisMetricsOutputText = output.ToString();

            return FinalMetric;
        }






        public string FormatMetricData()
        {
            StringBuilder output = new StringBuilder();
            
            output.AppendLine($"Price Trend Metric     {PriceTrendMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"One Year Target Metric {TargetPriceMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Earnings Metric        {EarningsPerShareMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Price Book Metric      {PriceBookMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Profit Margin Metric   {ProfitMarginMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Dividend Metric        {DividendMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Revenue Metric         {RevenueMetric.ToString("0.000").Replace("0.", " .")}");

            if (RevenueMetric * ProfitMetric < .87M)
            output.AppendLine($"Profit Metric          {ProfitMetric.ToString("0.000").Replace("0.", " .")}         * Financials are Bad *");
            else
            output.AppendLine($"Profit Metric          {ProfitMetric.ToString("0.000").Replace("0.", " .")}");

            output.AppendLine($"Basic EPS Metric       {BasicEpsMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Cash, Debt Metric      {CashDebtMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"PEG Ratio Metric       {PegRatioMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Oper. Cash Flow        {OperatingCashFlowMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Free Cash Flow         {FreeCashFlowMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"End Cash Metric        {EndCashMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Final Cash Flow Metric {FinalCashFlowMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"Valuation              {ValuationMetric.ToString("0.000").Replace("0.", " .")}");
            output.AppendLine($"----------------------------------------------------");

            string totalMetricString = $"Total Metric           {FinalMetric.ToString(".000").Replace("0.", " .")}";
            if (FinalMetric < .78M)
            {
                FinalMetric = .78M;
                totalMetricString += $"  low end limited to {FinalMetric.ToString(".00")}";
                output.AppendLine($"Liquidate this stock!!!!!");
            }
            if (FinalMetric > 1.32M)
            {
                FinalMetric = 1.32M;
                totalMetricString += $"  high end limited to {FinalMetric.ToString(".00")}";
            }
            output.AppendLine(totalMetricString);
            output.AppendLine("");

            return output.ToString();
        }







        public string JointTradeTargets(StockDownloads stockDownloads)
        {
            decimal finalFactor = 1.0M;
            decimal percentToApply = 35;

            StringBuilder output = new StringBuilder();

            var position = ExcelManager.PositionList.Where(x => x.Symbol == stockDownloads.stockSummary.Ticker).FirstOrDefault();
            var jointPosition = ExcelManager.JointPositionList.Where(x => x.Symbol == stockDownloads.stockSummary.Ticker).FirstOrDefault();
            decimal metric = 1;
            if(position != null)
                metric = (decimal)position.TotalMetric;

            if (jointPosition.BuySell == null)
            {
                // get buy sell from joint trades
                var jointTrade = ExcelManager.JointTradeList.Where(x => x.Symbol == stockDownloads.stockSummary.Ticker).LastOrDefault();
                jointPosition.BuySell = jointTrade.BuySell;
                if(jointPosition.BuySell == "Buy")
                {
                    jointPosition.BuyPrice = jointTrade.Price;
                    jointPosition.BuyQuantity = jointTrade.Quantity;
                }
                else
                {
                    jointPosition.SellPrice = jointTrade.Price;
                    jointPosition.SellQuantity = jointTrade.Quantity;
                }
            }

            string lastBuyorSell = jointPosition.BuySell.Substring(0, 3).ToLower();
            decimal lastTradePrice;
            if(lastBuyorSell == "buy")
            {
                lastTradePrice = (decimal)jointPosition.BuyPrice;
            }
            else  // sell
            {
                lastTradePrice = (decimal)jointPosition.SellPrice;
            }

            // Look for price trends. Topping out or bottoming out. 1 year to 3 months. Last 3 months to today.
            // Slope of 1 year to 3 months ago.
            decimal priceYear    = stockDownloads.stockHistory.HistoricDataYearAgo.Price;

            if (stockDownloads.stockHistory.HistoricDataMonthAgo == null || stockDownloads.stockHistory.HistoricData3MonthsAgo == null)
                stockDownloads.stockHistory.GetPriceHistoryForTodayWeekMonthYear(stockDownloads.stockSummary.Ticker, stockDownloads.stockSummary);
            decimal price3Months = stockDownloads.stockHistory.HistoricData3MonthsAgo.Price;
            
            decimal priceMonth   = stockDownloads.stockHistory.HistoricDataMonthAgo.Price;
            decimal priceWeek    = stockDownloads.stockHistory.HistoricDataWeekAgo.Price;
            decimal priceToday   = stockDownloads.stockHistory.HistoricDataToday.Price;

            decimal yearMonthlyGain = (((price3Months - priceYear) / 9) / price3Months) * 100;
            decimal threeMonthsMonthlyGain = (((priceToday - price3Months) / 3) / priceToday * 100);

            output.AppendLine($"Price year ago:         {priceYear.ToString("0.00").PadLeft(6)}");
            output.AppendLine($"Price 3 months ago:     {price3Months.ToString("0.00").PadLeft(6)}");
            output.AppendLine($"Price today:            {priceToday.ToString("0.00").PadLeft(6)}");
            output.AppendLine($"Monthly gain year:     {yearMonthlyGain.ToString("0.00").PadLeft(6)}%");
            output.AppendLine($"Monthly gain 3 months: {threeMonthsMonthlyGain.ToString("0.00").PadLeft(6)}%");

            // Are both gains above 2%
            if (threeMonthsMonthlyGain > 2 && yearMonthlyGain > 2)
            {
                finalFactor += .1M;
                output.AppendLine($"Both gains above 2%");
            }

            // Is the latest 3 month gains better the the previous 9 month gains?
            if (threeMonthsMonthlyGain > yearMonthlyGain * 1.2M)
            {
                finalFactor += .1M;
                output.AppendLine($"Gains are rising        {finalFactor.ToString("0.00").PadLeft(6)}");
            }

            output.AppendLine($"Stock Metric:           {metric.ToString("0.00").PadLeft(6)}");

            finalFactor += ((metric - 1.02M) / 2M);
            output.AppendLine($"Final Factor:           {finalFactor.ToString("0.00").PadLeft(6)}");

            decimal volitilityFactor = 1; // Math.Log((Math.Log10(stockDownloads.stockSummary.Volatility) + 1)) + 1; 
            volitilityFactor = (decimal)AdjustMetric((double)stockDownloads.stockSummary.VolatilityString.NumericValue, -6D) - .01M;
            output.AppendLine($"Volitility Factor:      {volitilityFactor.ToString("0.00").PadLeft(6)}");

            percentToApply *= volitilityFactor;
            output.AppendLine($"Percent to apply:       {percentToApply.ToString("0.00").PadLeft(6)}");
            output.AppendLine($"Buy Percent:            {(percentToApply / finalFactor).ToString("0.00").PadLeft(6)}");
            output.AppendLine($"Sell Percent:           {(percentToApply * finalFactor).ToString("0.00").PadLeft(6)}");


            ////////// One Year Target - Not a very valuable metric. 
            decimal targetPriceMetric = 1M;
            targetPriceMetric = stockDownloads.stockSummary.OneYearTargetPriceString.NumericValue / stockDownloads.stockSummary.PriceString.NumericValue;
            targetPriceMetric = AdjustMetric(targetPriceMetric, -10M);
            targetPriceMetric = SoftLimit(targetPriceMetric, .9M, 1.01M);
            output.AppendLine($"One year target metric: {targetPriceMetric.ToString("0.00").PadLeft(6)}");

            output.AppendLine($"Last traded price:      {lastTradePrice.ToString("0.00").PadLeft(6)}");

            decimal highest3MonthPrice = Math.Max(Math.Max(price3Months, priceMonth), priceWeek);
            output.AppendLine($"Highest Price 3M:       {highest3MonthPrice.ToString("0.00").PadLeft(6)}");

            BuyPrice = lastTradePrice * 1 / (1 + ((percentToApply / finalFactor) / 100));
            SellPrice = lastTradePrice * (1 + ((percentToApply * finalFactor) / 100));

            output.AppendLine($"Raw Buy Price:          {BuyPrice.ToString("0.00").PadLeft(6)}");
            output.AppendLine($"Raw Sell Price:         {SellPrice.ToString("0.00").PadLeft(6)}");

            if (highest3MonthPrice > lastTradePrice)
            {
                BuyPrice = lastTradePrice * 1 / (1 + ((percentToApply / finalFactor) / 100)) + (highest3MonthPrice - lastTradePrice);
            }

            return output.ToString();
        }

        public class AnalyzeInputs
        {
            public int SharesOwned;
            public BuyOrSell LastTradeBuySell;
            public int QuantityTraded;
            public decimal SharesTradedPrice;
            public decimal MovementTargetPercent;
        }
    }
}

