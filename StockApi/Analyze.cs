using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace StockApi
{
    /// <summary>
    /// 
    /// </summary>
    class Analyze : YahooFinance
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

        public void AnalyzeStockData(StockSummary stockSummary, StockHistory stockHistory, StockFinancials stockFinancials, AnalyzeInputs analyzeInputs)
        {
            _buyless = false;
            StringBuilder output = new StringBuilder();
            // combine trends with
            // one year target
            // EPS
            // Fair Value
            // Profit Margin %
            // Volatility

            // Long Term Price Trend
            decimal priceTrendMetric = 1M;
            if (stockHistory.HistoricDataToday.Price > stockHistory.HistoricData3YearsAgo.Price * 1.6M)
                priceTrendMetric = 1.08M;
            else if (stockHistory.HistoricDataToday.Price > stockHistory.HistoricData3YearsAgo.Price * 1.2M)
                priceTrendMetric = 1.05M;
            else if (stockHistory.HistoricDataToday.Price > stockHistory.HistoricData3YearsAgo.Price * 1.1M)
                priceTrendMetric = 1.01M;
            else if (stockHistory.HistoricDataToday.Price < stockHistory.HistoricData3YearsAgo.Price * 0.6M)
                priceTrendMetric = .9M;
            else if (stockHistory.HistoricDataToday.Price < stockHistory.HistoricData3YearsAgo.Price * 0.8M)
                priceTrendMetric = .96M;
            else if (stockHistory.HistoricDataToday.Price < stockHistory.HistoricData3YearsAgo.Price * 0.93M)
                priceTrendMetric = .98M;

            // Consecutive buys or sells
            List<DataRow> drList =  Form1.TickerTradesDataTable.Select().Take(3).ToList();
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
                priceTrendMetric = (priceTrendMetric + 1) / 2 - .02M;
            if (buys == 3)
                priceTrendMetric = (priceTrendMetric + 1) / 2 - .03M;
            if (sells == 2)
                priceTrendMetric = (priceTrendMetric + 1) / 2 + .02M;
            if (sells == 3)
                priceTrendMetric = (priceTrendMetric + 1) / 2 + .03M;



            output.AppendLine($"Price Trend Metric = {priceTrendMetric.ToString(".00")}");

            // One Year Target - Not a very valuable metric. 
            decimal targetPriceMetric = 1M;
            if (stockSummary.OneYearTargetPriceString.NumericValue < stockSummary.PriceString.NumericValue * .95M)
                targetPriceMetric = .99M;
            else if (stockSummary.OneYearTargetPriceString.NumericValue > stockSummary.PriceString.NumericValue * 1.05M)
                targetPriceMetric = 1.01M;

            output.AppendLine($"One Year Target Metric = {targetPriceMetric.ToString(".00")}");

            // Earnings Per Share
            decimal epsMetric = 1M;
            if (stockSummary.EarningsPerShareString.NumericValue < -6)
                epsMetric = .94M;
            else if (stockSummary.EarningsPerShareString.NumericValue < -3)
                epsMetric = .96M;
            else if (stockSummary.EarningsPerShareString.NumericValue < -1)
                epsMetric = .98M;
            else if (stockSummary.EarningsPerShareString.NumericValue > 6)
                epsMetric = 1.05M;
            else if (stockSummary.EarningsPerShareString.NumericValue > 3)
                epsMetric = 1.03M;
            else if (stockSummary.EarningsPerShareString.NumericValue > 1)
                epsMetric = 1.02M;
            else if (stockSummary.EarningsPerShareString.NumericValue > 0)
                epsMetric = 1.01M;

            output.AppendLine($"Earnings Metric = {epsMetric}");

            // Price / Book
            decimal priceBookMetric = 1M;
            if (stockSummary.PriceBookColor == Color.Red)
                priceBookMetric = .99M;
            if (stockSummary.PriceBookColor == Color.Lime)
                priceBookMetric = 1.01M;

            output.AppendLine($"Price Book Metric = {priceBookMetric.ToString(".00")}");

            // Profit Margin Metric
            decimal profitMarginMetric = 1.00M;
            if (stockSummary.ProfitMarginString.NumericValue < -6)
                profitMarginMetric = .962M;
            else if (stockSummary.ProfitMarginString.NumericValue < -2)
                profitMarginMetric = .981M;
            else if (stockSummary.ProfitMarginString.NumericValue > 6)
                profitMarginMetric = 1.04M;
            else if (stockSummary.ProfitMarginString.NumericValue > 2)
                profitMarginMetric = 1.02M;

            output.AppendLine($"Profit Margin Metric = {profitMarginMetric.ToString(".00")}");

            // Dividend Metric
            decimal dividendMetric = 1M;
            if (stockSummary.DividendString.NumericValue > 8)
                dividendMetric = 1.08M;
            else if (stockSummary.DividendString.NumericValue > 5)
                dividendMetric = 1.05M;
            else if (stockSummary.DividendString.NumericValue > 2)
                dividendMetric = 1.02M;
            else if (stockSummary.DividendString.NumericValue > .5M)
                dividendMetric = 1.01M;
            else
                dividendMetric = .99M;

            if (priceTrendMetric < .92M && dividendMetric > 1.04M)
                dividendMetric = 1.01M; // if the price is going steeply down, who cares about a high dividend

            output.AppendLine($"Dividend Metric = {dividendMetric.ToString(".00")}");

            ///////////////////////////////////// Finacial Metrics
            // Revenue - Should be increasing YOY
            decimal revenueMetric = 1M;
            if (stockFinancials.RevenueTtm > 0)
            {
                if (stockFinancials.Revenue4 > 0)
                {
                    if (stockFinancials.Revenue2 > stockFinancials.Revenue4 * 1.05M) // Revenue 2 years ago is 5% above revenue 4 years ago 
                        revenueMetric = 1.025M;
                    if (stockFinancials.Revenue2 < stockFinancials.Revenue4 * .98M) // Revenue 2 years ago is 1% below revenue 4 years ago 
                        revenueMetric = .98M;
                    if (stockFinancials.RevenueTtm > stockFinancials.Revenue2 * 1.05M) // Revenue TTM is 5% above revenue 2 years ago 
                        revenueMetric = revenueMetric * 1.025M;
                    if (stockFinancials.RevenueTtm < stockFinancials.Revenue2 * .98M) // Revenue TTM is 1% below revenue 2 years ago 
                        revenueMetric = revenueMetric * .98M;
                    if (stockFinancials.RevenueTtm > stockFinancials.Revenue4 * 1.03M) // Revenue TTM is 5% above revenue 4 years ago 
                        revenueMetric = revenueMetric * 1.01M;
                    if (stockFinancials.RevenueTtm < stockFinancials.Revenue4 * .98M) // Revenue TTM is 1% below revenue 4 years ago 
                        revenueMetric = revenueMetric * .99M;
                }
            }
            output.AppendLine($"Revenue Metric = {revenueMetric.ToString(".00")}");
            // Profit - Revenue - Cost of Revenue
            decimal profitMetric = 1M;
            if (stockFinancials.Profit2YearsAgo > stockFinancials.Profit4YearsAgo * 1.01M)
                profitMetric = 1.02M;
            if (stockFinancials.Profit2YearsAgo < stockFinancials.Profit4YearsAgo * .99M)
                profitMetric = .98M;
            if (stockFinancials.ProfitTTM > stockFinancials.Profit2YearsAgo * 1.01M)
                profitMetric *= 1.02M;
            if (stockFinancials.ProfitTTM < stockFinancials.Profit2YearsAgo * .99M)
                profitMetric *= .98M;
            if (stockFinancials.ProfitTTM > stockFinancials.Profit4YearsAgo * 1.2M)
                profitMetric *= 1.01M;
            else if (stockFinancials.ProfitTTM > stockFinancials.Profit4YearsAgo * 1.02M)
                profitMetric *= 1.01M;
            if (stockFinancials.ProfitTTM < stockFinancials.Profit4YearsAgo * .98M)
                profitMetric *= .985M;
            if (revenueMetric * profitMetric < .87M)
                output.AppendLine($"Profit Metric = {profitMetric.ToString(".00")}         * Financials are Bad *");
            else
                output.AppendLine($"Profit Metric = {profitMetric.ToString(".00")}");

            decimal cashDebtMetric = 1M;
            if (stockFinancials.TotalDebt > stockFinancials.TotalCash * 5) // lots of debt compared to cash
                cashDebtMetric = .96M;
            else if (stockFinancials.TotalCash > stockFinancials.TotalDebt * 2) // lots of cash compared to debt
                cashDebtMetric = 1.03M;
            if (stockFinancials.DebtEquity > 120) // Over 120% D/E is bad
                cashDebtMetric = cashDebtMetric * .96M;
            output.AppendLine($"Cash, Debt Metric = {cashDebtMetric.ToString(".00")}");

            // Valuation based on PE and sector
            decimal valuationMetric = 1M;
            if (stockSummary.Valuation == StockSummary.ValuationEnum.OverValued)
                valuationMetric = .98M;
            else if (stockSummary.Valuation == StockSummary.ValuationEnum.UnderValued)
                valuationMetric = 1.02M;
            output.AppendLine($"Valuation = {valuationMetric.ToString(".00")}");

            // Market
            decimal ecoMetric = 1 + ((analyzeInputs.MarketHealth - 5) / 50);
            output.AppendLine($"Market Metric = {ecoMetric.ToString(".00")}");

            decimal totalMetric = priceTrendMetric * targetPriceMetric * epsMetric * priceBookMetric * dividendMetric * profitMarginMetric * ecoMetric * revenueMetric * profitMetric * cashDebtMetric * valuationMetric;
            output.AppendLine($"----------------------------------------------------");
            string totalMetricString = $"Total Metric = {totalMetric.ToString(".00")}";
            if (totalMetric < .8M)
            {
                totalMetric = .8M;
                totalMetricString += $"  low end limited to {totalMetric.ToString(".00")}";
                output.AppendLine($"Liquidate this stock!!!!!");
            }
            if (totalMetric > 1.2M)
            {
                totalMetric = 1.2M;
                totalMetricString += $"  high end limited to {totalMetric.ToString(".00")}";
            }

            output.AppendLine(totalMetricString);

            output.AppendLine("");


            ///////////// Setting Price Movement Multipliers
            // Gets the volatility number closer to 1, less exxtreme. 2.6 becomes 1.5
            decimal volitilityFactor = 1; // Math.Log((Math.Log10(stockSummary.Volatility) + 1)) + 1; 
            if (stockSummary.VolatilityString.NumericValue < .5M)
                volitilityFactor = .86M;
            else if (stockSummary.VolatilityString.NumericValue < .8M)
                volitilityFactor = .93M;
            else if (stockSummary.VolatilityString.NumericValue > 2M)
                volitilityFactor = 1.14M;
            else if (stockSummary.VolatilityString.NumericValue > 1.2M)
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

            buyPrice = buyPrice * totalMetric;
            sellPrice = sellPrice * totalMetric;

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
            decimal limitPrice = (stockSummary.PriceString.NumericValue * lowerMovementMultiplier) * .5M; // lower buy limit price too low
            if (buyPrice < limitPrice)
                buyPrice = limitPrice;
            //limitPrice = (stockSummary.Price * lowerMovementMultiplier) * 1.3M; // upper buy limit
            //if (buyPrice > limitPrice)
            //    buyPrice = limitPrice;
            //limitPrice = (stockSummary.Price * upperMovementMultiplier) * .6M; // lower sell limit
            //if (sellPrice < limitPrice)
            //    sellPrice = limitPrice;
            limitPrice = (stockSummary.PriceString.NumericValue * upperMovementMultiplier) * 1.4M; // upper sell limit
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
            if (stockSummary.DividendString.NumericValue > 8 && priceTrendMetric > .92M)
            {
                buyQuantity = buyQuantity * 1.2M;
                sellQuantity = sellQuantity / 1.2M;
                output.AppendLine($"Great dividend. Buy more or sell less. {buyQuantity.ToString("##.##")} {sellQuantity.ToString("##.##")}");
            }
            else if (stockSummary.DividendString.NumericValue > 4)
            {
                buyQuantity = buyQuantity * 1.1M;
                sellQuantity = sellQuantity / 1.1M;
            }
            else if (stockSummary.DividendString.NumericValue > 2)
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
            if (stockHistory.YearTrend == StockHistory.TrendEnum.Up)
            {
                buyQuantity = buyQuantity * 1.05M;
                sellQuantity = sellQuantity / 1.05M;
                output.AppendLine($"Year up trend applied. {buyQuantity.ToString("##.##")} {sellQuantity.ToString("##.##")}");
            }
            if (stockHistory.YearTrend == StockHistory.TrendEnum.Down)
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
            decimal profit = (SellPrice - stockSummary.PriceString.NumericValue) * SellQuantity;
            if (profit < 20M && totalMetric > .91M) // if it's a bad stock we are liquidating and the $20 profit doesn't matter. 
            {
                SellPrice = (20M / SellQuantity) + stockSummary.PriceString.NumericValue;
                output.AppendLine($"Minimum profit set to $20. Sell Price: {SellPrice.ToString("##.##")}");
            }

            AnalysisMetricsOutputText = output.ToString();
        }

        public class AnalyzeInputs
        {
            public int SharesOwned;
            public BuyOrSell LastTradeBuySell;
            public int QuantityTraded;
            public decimal SharesTradedPrice;
            public decimal MovementTargetPercent;
            public decimal MarketHealth;
        }
    }
}

