using StockApi.Downloads;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace StockApi
{
    public class Analyze : YahooFinance
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

        public decimal AnalyzeStockData(StockDownloads stockDownloads, AnalyzeInputs analyzeInputs, bool forMetricOnly)
        {
            _buyless = false;
            StringBuilder output = new StringBuilder();
            // combine trends with
            // one year target
            // EPS
            // Fair Value
            // Profit Margin %
            // Volatility
            _ticker = stockDownloads.stockSummary.Ticker;
            Debug.WriteLine(stockDownloads.stockSummary.Ticker);

            // Long Term Price Trend
            decimal priceTrendMetric = 1M;
            if (stockDownloads.stockHistory.HistoricDataToday.Price > stockDownloads.stockHistory.HistoricData3YearsAgo.Price * 2M)
                if (stockDownloads.stockSummary.ForwardPEString.NumericValue > 100)
                    priceTrendMetric = 1.06M;
                else
                    priceTrendMetric = 1.07M;
            else if (stockDownloads.stockHistory.HistoricDataToday.Price > stockDownloads.stockHistory.HistoricData3YearsAgo.Price * 1.6M)
                if (stockDownloads.stockSummary.ForwardPEString.NumericValue > 100)
                    priceTrendMetric = 1.04M;
                else
                    priceTrendMetric = 1.05M;
            else if (stockDownloads.stockHistory.HistoricDataToday.Price > stockDownloads.stockHistory.HistoricData3YearsAgo.Price * 1.3M)
                priceTrendMetric = 1.04M;
            else if (stockDownloads.stockHistory.HistoricDataToday.Price > stockDownloads.stockHistory.HistoricData3YearsAgo.Price * 1.2M)
                priceTrendMetric = 1.02M;
            else if (stockDownloads.stockHistory.HistoricDataToday.Price > stockDownloads.stockHistory.HistoricData3YearsAgo.Price * 1.1M)
                priceTrendMetric = 1.01M;
            else if (stockDownloads.stockHistory.HistoricDataToday.Price < stockDownloads.stockHistory.HistoricData3YearsAgo.Price * 0.6M)
                priceTrendMetric = .9M;
            else if (stockDownloads.stockHistory.HistoricDataToday.Price < stockDownloads.stockHistory.HistoricData3YearsAgo.Price * 0.8M)
                priceTrendMetric = .96M;
            else if (stockDownloads.stockHistory.HistoricDataToday.Price < stockDownloads.stockHistory.HistoricData3YearsAgo.Price * 0.93M)
                priceTrendMetric = .98M;

            output.AppendLine($"Price Trend Metric = {priceTrendMetric.ToString(".00")}");

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

            // One Year Target - Not a very valuable metric. 
            decimal targetPriceMetric = 1M;
            if (stockDownloads.stockSummary.OneYearTargetPriceString.NumericValue < stockDownloads.stockSummary.PriceString.NumericValue * .95M)
                targetPriceMetric = .99M;
            else if (stockDownloads.stockSummary.OneYearTargetPriceString.NumericValue > stockDownloads.stockSummary.PriceString.NumericValue * 1.05M)
                targetPriceMetric = 1.01M;

            output.AppendLine($"One Year Target Metric = {targetPriceMetric.ToString(".00")}");

            // Earnings Per Share
            decimal epsMetric = 1M;
            if (stockDownloads.stockSummary.EarningsPerShareString.NumericValue < -6)
                epsMetric = .94M;
            else if (stockDownloads.stockSummary.EarningsPerShareString.NumericValue < -3)
                epsMetric = .96M;
            else if (stockDownloads.stockSummary.EarningsPerShareString.NumericValue < -1)
                epsMetric = .98M;
            else if (stockDownloads.stockSummary.EarningsPerShareString.NumericValue > 6)
                epsMetric = 1.05M;
            else if (stockDownloads.stockSummary.EarningsPerShareString.NumericValue > 3)
                epsMetric = 1.03M;
            else if (stockDownloads.stockSummary.EarningsPerShareString.NumericValue > 1)
                epsMetric = 1.02M;
            else if (stockDownloads.stockSummary.EarningsPerShareString.NumericValue > 0)
                epsMetric = 1.01M;

            output.AppendLine($"Earnings Metric = {epsMetric}");

            // Price / Book
            decimal priceBookMetric = 1M;
            if (stockDownloads.stockSummary.PriceBookString.NumericValue > 5)
                priceBookMetric = .99M;
            else if (stockDownloads.stockSummary.PriceBookString.NumericValue < 1)
                priceBookMetric = 1.01M;

            output.AppendLine($"Price Book Metric = {priceBookMetric.ToString(".00")}");

            // Profit Margin Metric
            decimal profitMarginMetric = 1.00M;
            if (stockDownloads.stockSummary.ProfitMarginString.NumericValue < -6)
                profitMarginMetric = .962M;
            else if (stockDownloads.stockSummary.ProfitMarginString.NumericValue < -2)
                profitMarginMetric = .981M;
            else if (stockDownloads.stockSummary.ProfitMarginString.NumericValue > 6)
                profitMarginMetric = 1.04M;
            else if (stockDownloads.stockSummary.ProfitMarginString.NumericValue > 2)
                profitMarginMetric = 1.02M;

            output.AppendLine($"Profit Margin Metric = {profitMarginMetric.ToString(".00")}");

            // Dividend Metric
            decimal dividendMetric = 1M;
            if (stockDownloads.stockSummary.DividendString.NumericValue > 8)
                dividendMetric = 1.08M;
            else if (stockDownloads.stockSummary.DividendString.NumericValue > 5)
                dividendMetric = 1.05M;
            else if (stockDownloads.stockSummary.DividendString.NumericValue > 2)
                dividendMetric = 1.02M;
            else if (stockDownloads.stockSummary.DividendString.NumericValue > .5M)
                dividendMetric = 1.01M;
            else
                dividendMetric = .99M;

            if (priceTrendMetric < .92M && dividendMetric > 1.04M)
                dividendMetric = (1M + dividendMetric) / 2M; // if the price is going steeply down, who cares about a high dividend

            output.AppendLine($"Dividend Metric = {dividendMetric.ToString(".00")}");

            //////////////////////////////////////////////////////////////////////////////////
            ///                            Income Statement Metrics

            // Revenue 
            decimal revenueMetric = 1M;
            revenueMetric = SetYearOverYearTrend(stockDownloads.stockIncomeStatement.Revenue4String, stockDownloads.stockIncomeStatement.Revenue2String, stockDownloads.stockIncomeStatement.RevenueTtmString, 0);
            output.AppendLine($"Revenue Metric = {revenueMetric.ToString(".00")}");


            /////////// Profit - Revenue - Cost of Revenue
            decimal profitMetric = 1M;
            profitMetric = SetYearOverYearTrend(stockDownloads.stockIncomeStatement.Profit4String, stockDownloads.stockIncomeStatement.Profit2String, stockDownloads.stockIncomeStatement.ProfitTtmString, 1);

            if (revenueMetric * profitMetric < .87M)
                output.AppendLine($"Profit Metric = {profitMetric.ToString(".00")}         * Financials are Bad *");
            else
                output.AppendLine($"Profit Metric = {profitMetric.ToString(".00")}");

            decimal cashDebtMetric = 1M;
            if (stockDownloads.stockStatistics.TotalDebt > stockDownloads.stockStatistics.TotalCash * 5) // lots of debt compared to cash
                cashDebtMetric = .96M;
            else if (stockDownloads.stockStatistics.TotalCash > stockDownloads.stockStatistics.TotalDebt * 2) // lots of cash compared to debt
                cashDebtMetric = 1.03M;
            if (stockDownloads.stockStatistics.DebtEquityString.NumericValue > 120) // Over 120% D/E is bad
                cashDebtMetric = cashDebtMetric * .96M;
            output.AppendLine($"Cash, Debt Metric = {cashDebtMetric.ToString(".00")}");


            /////////// Basic Earning per share growth
            decimal basicEpsMetric = 1.0M;
 
            basicEpsMetric = SetYearOverYearTrend(stockDownloads.stockIncomeStatement.BasicEps4String, stockDownloads.stockIncomeStatement.BasicEps2String, stockDownloads.stockIncomeStatement.BasicEpsTtmString, -2);

             // All negative earnings, downgrade the earnings metric
            if (stockDownloads.stockIncomeStatement.BasicEpsTtmString.NumericValue < 0 && stockDownloads.stockIncomeStatement.BasicEps2String.NumericValue < 0 && stockDownloads.stockIncomeStatement.BasicEps4String.NumericValue < 0 && basicEpsMetric > 1.02M)
                basicEpsMetric -= .02M;

            output.AppendLine($"Basic EPS Metric = {basicEpsMetric.ToString(".00")}");


            //////////////////////////////////////////////////////////////////////////////////
            ///                             Cash Flow Metrics
            // Operationg Cash Flow
            decimal operCashFlowMetric = 1M;
            operCashFlowMetric = SetYearOverYearTrend(stockDownloads.stockCashFlow.OperatingCashFlow4String, stockDownloads.stockCashFlow.OperatingCashFlow2String, stockDownloads.stockCashFlow.OperatingCashFlowTtmString, -4);
            output.AppendLine($"Oper. Cash Flow Metric = {operCashFlowMetric.ToString(".00")}");

            // Free Cash Flow
            decimal freeCashFlowMetric = 1M;
            freeCashFlowMetric = SetYearOverYearTrend(stockDownloads.stockCashFlow.FreeCashFlow4String, stockDownloads.stockCashFlow.FreeCashFlow2String, stockDownloads.stockCashFlow.FreeCashFlowTtmString, -4);
            output.AppendLine($"Free Cash Flow Metric = {freeCashFlowMetric.ToString(".00")}");

            // End Cash Position
            decimal endCashMetric = 1M;
            endCashMetric = SetYearOverYearTrend(stockDownloads.stockCashFlow.EndCashPosition4String, stockDownloads.stockCashFlow.EndCashPosition2String, stockDownloads.stockCashFlow.EndCashPositionTtmString, -4);
            output.AppendLine($"End Cash Metric = {endCashMetric.ToString(".00")}");

            // Extra ratio calculations
            decimal FcfRatio = 1;
            decimal finalCashFlowMetric = (operCashFlowMetric + freeCashFlowMetric + endCashMetric) / 3;
            if (stockDownloads.stockIncomeStatement.NetIncomeTtmString.NumericValue > 1000)
            {
                FcfRatio = stockDownloads.stockCashFlow.FreeCashFlowTtmString.NumericValue / stockDownloads.stockIncomeStatement.NetIncomeTtmString.NumericValue;
                if (FcfRatio > 1.05M) finalCashFlowMetric = finalCashFlowMetric * 1.01M;
                else if (FcfRatio > .9M) finalCashFlowMetric = finalCashFlowMetric * 1.005M;
                else if (FcfRatio < .3M) finalCashFlowMetric = finalCashFlowMetric * 0.99M;
                else if (FcfRatio < .6M) finalCashFlowMetric = finalCashFlowMetric * 0.995M;
            }
            else
            {
                if (stockDownloads.stockIncomeStatement.NetIncomeTtmString.NumericValue < -10000)
                {
                    finalCashFlowMetric = (decimal)AdjustMetric((double)finalCashFlowMetric, -4); // less impact
                }
            }

            decimal cashRatio = 1; // How long cash can last, burn thru, for operating expenses
            if (stockDownloads.stockCashFlow.EndCashPositionTtmString.NumericValue > 0 && stockDownloads.stockIncomeStatement.OperatingExpenseTtmString.NumericValue > 0)
                cashRatio = stockDownloads.stockCashFlow.EndCashPositionTtmString.NumericValue / stockDownloads.stockIncomeStatement.OperatingExpenseTtmString.NumericValue;

            if (cashRatio > 2.2M)
                finalCashFlowMetric = finalCashFlowMetric * 1.008M;
            if (cashRatio < 1.2M)
                finalCashFlowMetric = finalCashFlowMetric * .992M;

            output.AppendLine($"Final Cash Flow Metric = {finalCashFlowMetric.ToString(".00")}");

            ///////////////////////////////////////////////////////////////
            ///               Valuation based on PE and sector
            decimal valuationMetric = 1M;
            if (stockDownloads.stockSummary.Valuation == StockSummary.ValuationEnum.OverValued)
                valuationMetric = .98M;
            else if (stockDownloads.stockSummary.Valuation == StockSummary.ValuationEnum.UnderValued)
                valuationMetric = 1.02M;
            output.AppendLine($"Valuation = {valuationMetric.ToString(".00")}");

            //// Calculate total metric
            decimal totalMetric =    priceTrendMetric   * epsMetric     * ((targetPriceMetric + priceBookMetric) / 2) * 
                    dividendMetric * profitMarginMetric * revenueMetric * ((profitMetric + basicEpsMetric) / 2) * 
                    cashDebtMetric * valuationMetric    * finalCashFlowMetric;

            output.AppendLine($"----------------------------------------------------");
            string totalMetricString = $"Total Metric = {totalMetric.ToString(".000")}";
            if (totalMetric < .78M)
            {
                totalMetric = .78M;
                totalMetricString += $"  low end limited to {totalMetric.ToString(".00")}";
                output.AppendLine($"Liquidate this stock!!!!!");
            }
            if (totalMetric > 1.32M)
            {
                totalMetric = 1.32M;
                totalMetricString += $"  high end limited to {totalMetric.ToString(".00")}";
            }
            output.AppendLine(totalMetricString);

            totalMetric = Math.Round(totalMetric, 2);
            if (forMetricOnly == true)
            {
                AnalysisMetricsOutputText = output.ToString();
                return totalMetric; //========================>>>>>>>>>>>>>>>>>>>>  Get out
            }

            output.AppendLine("");

            ///////////////////////////////////////////////////////////
            ///                     BUY and SELL
            ///////////// Setting Price Movement Multipliers
            output.AppendLine($"Buys Sells Metric = {buySellMetric.ToString(".00")}");

            // Adjust based on series of buys or sells
            totalMetric = totalMetric * buySellMetric;

            // Gets the volatility number closer to 1, less exxtreme. 2.6 becomes 1.5
            decimal volitilityFactor = 1; // Math.Log((Math.Log10(stockDownloads.stockSummary.Volatility) + 1)) + 1; 
            if (stockDownloads.stockSummary.VolatilityString.NumericValue < .5M)
                volitilityFactor = .86M;
            else if (stockDownloads.stockSummary.VolatilityString.NumericValue < .8M)
                volitilityFactor = .93M;
            else if (stockDownloads.stockSummary.VolatilityString.NumericValue > 2M)
                volitilityFactor = 1.14M;
            else if (stockDownloads.stockSummary.VolatilityString.NumericValue > 1.2M)
                volitilityFactor = 1.07M;
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

            buyPrice = buyPrice * ((totalMetric + 1) / 2);
            sellPrice = sellPrice * ((totalMetric + 1) / 2);

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
            if (stockDownloads.stockSummary.DividendString.NumericValue > 8 && priceTrendMetric > .92M)
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
            if (profit < 20M && totalMetric > .91M) // if it's a bad stock we are liquidating and the $20 profit doesn't matter. 
            {
                SellPrice = (20M / (SellQuantity + 3)) + stockDownloads.stockSummary.PriceString.NumericValue;
                output.AppendLine($"Minimum profit set to $20. Sell Price: {SellPrice.ToString("##.##")}");
            }

            AnalysisMetricsOutputText = output.ToString();

            return totalMetric;
        }

        private static decimal SetYearOverYearTrend(StringSafeType<decimal> year4, StringSafeType<decimal> year2, StringSafeType<decimal> ttm, int adjustment)
        {
            CrunchThreeResult crt = new CrunchThreeResult();
            decimal val = 0;

            try
            {
                crt = CrunchThree((double)year4.NumericValue, (double)year2.NumericValue, (double)ttm.NumericValue);
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, adjustment); // less impact
                val = (decimal)crt.FinalMetric;
            }
             catch(Exception ex) 
            {
                Debug.WriteLine($"{_ticker} {crt.FinalMetric} {ex.Message}");
            }

            return (decimal)crt.FinalMetric;
        }

        public static CrunchThreeResult CrunchThree(double one, double two, double three)
        {
            CrunchThreeResult crt = new CrunchThreeResult();

            if (one + two + three == 0)
            {
                Program.logger.Info($"{_ticker} had zeros in it.");
                crt.FinalMetric = 1;
                return crt;
            }

            // Find Abs(minimum)
            double minimum = Math.Min(one, Math.Min(two, three)); // example -2, 3, -1 = 4    2, 3, 6 = 4   - 11, 100, 100 = 22
            double maximum = Math.Max(one, Math.Max(two, three)); // example -2, 3, -1 = 4    2, 3, 6 = 4   - 11, 100, 100 = 22
            if (minimum < 0)
                minimum = Math.Abs(minimum) * 2;
            if (maximum < 0)
                maximum = Math.Abs(maximum) * 2;

            if (minimum * 6 < maximum)
                minimum += maximum;

            // Add to all 3 numbers
            one += minimum; two += minimum; three += minimum;

            // Get ratios between them
            crt.Ratio1 = (double)two / (double)one;
            crt.Ratio2 = (double)three / (double)two;
            crt.Ratio3 = (double)three / (double)one;

            // Get the Log() of the ratios and dvide by a factor of 8
            crt.Log1 = 1 + Math.Log(crt.Ratio1) / 3; // diff for earlier less important
            crt.Log2 = 1 + Math.Log(crt.Ratio2) / 2.2D;
            crt.Log3 = 1 + Math.Log(crt.Ratio3) / 2.2D;

            crt.FinalMetric = (crt.Log1 + crt.Log2 + crt.Log3) / 3;

            if (crt.FinalMetric > 1.07D)
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, -4); // Lessen metric weight
            if (crt.FinalMetric > 1.06D)
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, -2); // Lessen metric weight
            if (crt.FinalMetric < .93D)
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, -4); // Lessen metric weight
            if (crt.FinalMetric < .94D)
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, -2); // Lessen metric weight

            return crt;
        }

        public static double AdjustMetric(double metric, double factor) // negative number less important, positive more important, range -5 to +5
        {
            double newMetric = metric;

            if (factor < 0)
                newMetric = metric - Math.Log(metric) / (6 + factor);
            if (factor > 0)
                newMetric = metric - Math.Log(metric) / (6 - factor);

            return newMetric;
        }

        public class CrunchThreeResult
        {
            public double Ratio1;
            public double Ratio2;
            public double Ratio3;
            public double Log1;
            public double Log2;
            public double Log3;
            public double FinalMetric;
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

