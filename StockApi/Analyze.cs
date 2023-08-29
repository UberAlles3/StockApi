using CommonClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        public int BuyQuantity;
        public float BuyPrice;
        public int SellQuantity;
        public float SellPrice = 0;
        public string AnalysisMetricsOutputText;

        public void AnalyzeStockData(StockSummary stockSummary, StockHistory stockHistory, AnalyzeInputs analyzeInputs)
        {
            StringBuilder output = new StringBuilder();
            // combine trends with
            // one year target
            // EPS
            // Fair Value
            // Estimated Return %
            // Volatility

            // Price Trend
            float trendMetric = Convert.ToInt16(stockHistory.YearTrend) + Convert.ToInt16(stockHistory.MonthTrend) + Convert.ToInt16(stockHistory.WeekTrend);
            if(trendMetric == 0) // Very downward trend
                trendMetric = .88F;
            else if (trendMetric == 1)
                trendMetric = .92F;
            else if (trendMetric == 2)
                trendMetric = .96F;
            else if (trendMetric == 3)
                trendMetric = 1.00F;
            else if (trendMetric == 4)
                trendMetric = 1.04F; 
            else if (trendMetric == 5)
                trendMetric = 1.08F; 
            else if (trendMetric == 6)
                trendMetric = 1.12F; // Very upward trend

            output.AppendLine($"Price Trend Metric = {trendMetric}");

            // One Year Target
            float targetPriceMetric = 1F;
            if (stockSummary.OneYearTargetPrice < stockSummary.Price * .95)
                targetPriceMetric = .96F;
            else if (stockSummary.OneYearTargetPrice > stockSummary.Price * 1.05)
                targetPriceMetric = 1.04F;

            output.AppendLine($"One Year Target Metric = {targetPriceMetric}");

            // Earnings Per Share
            float epsMetric = 1F;
            if (stockSummary.EarningsPerShare < -6)
                epsMetric = .94F;
            else if (stockSummary.EarningsPerShare < -3)
                epsMetric = .96F;
            else if (stockSummary.EarningsPerShare < -1)
                epsMetric = .98F;
            else if (stockSummary.EarningsPerShare > 6)
                epsMetric = 1.06F;
            else if (stockSummary.EarningsPerShare > 3)
                epsMetric = 1.04F;
            else if (stockSummary.EarningsPerShare > 1)
                epsMetric = 1.02F;

            output.AppendLine($"Earnings Metric = {epsMetric}");

            // Fair Value
            float fairValueMetric = 1F;
            if (stockSummary.FairValueColor == Color.Red)
                fairValueMetric = .9F;
            if (stockSummary.FairValueColor == Color.Lime)
                fairValueMetric = 1.1F;

            output.AppendLine($"Fair Value Metric = {fairValueMetric}");

            // Estimated Return Metric
            float estimatedReturnMetric = 1F;
            if (stockSummary.EstimatedReturn < -6)
                estimatedReturnMetric = .96F;
            else if (stockSummary.EstimatedReturn < -2)
                estimatedReturnMetric = .98F;
            else if (stockSummary.EstimatedReturn > 6)
                estimatedReturnMetric = 1.04F;
            else if (stockSummary.EstimatedReturn > 2)
                estimatedReturnMetric = 1.02F;

            output.AppendLine($"Estimated Return Metric = {estimatedReturnMetric}");

            // Dividend Metric
            float dividendMetric = 1F;
            if (stockSummary.Dividend > 8)
                dividendMetric = 1.14F;
            else if (stockSummary.Dividend > 5)
                dividendMetric = 1.10F;
            else if (stockSummary.Dividend > 2)
                dividendMetric = 1.06F;
            else if (stockSummary.Dividend > .5)
                dividendMetric = 1.02F;
            else
                dividendMetric = .98F;

            output.AppendLine($"Dividend Metric = {dividendMetric}");

            float totalMetric = trendMetric * targetPriceMetric * epsMetric * fairValueMetric * dividendMetric * estimatedReturnMetric;

            output.AppendLine($"Total Metric = {totalMetric}");

            ///////////// Setting Price Movement Multipliers
            // Gets the volatility number closer to 1, less exxtreme. 2.6 becomes 1.5
            double volitilityFactor = Math.Log((Math.Log10(stockSummary.Volatility) + 1)) + 1; 
            analyzeInputs.MovementTargetPercent *= (float)volitilityFactor; // Applying volatility
            output.AppendLine($"Movement % w/ Volatility = { analyzeInputs.MovementTargetPercent.ToString("##.##")}");
            
            // if stock is like XOM where your not sell a large percentage of shares owned
            // Make the lower and upper movement less
            if(analyzeInputs.QuantityTraded < analyzeInputs.SharesOwned / 4)
            {
                analyzeInputs.MovementTargetPercent = analyzeInputs.MovementTargetPercent * .9F;
                output.AppendLine($"Movement reduced, low share quantity = {analyzeInputs.MovementTargetPercent.ToString("##.##")}");
            }

            float lowerMovementMultiplier = ((100F - analyzeInputs.MovementTargetPercent) / 100F); // if movement is 20% will assign .8
            float upperMovementMultiplier = ((100F + analyzeInputs.MovementTargetPercent) / 100F); // if movement is 20% will assign 1.2

            // Calculate future buy or sells based on how much it moves and what our target movement is. 
            float buyPrice = stockHistory.HistoricDataToday.Price * lowerMovementMultiplier;
            float sellPrice = stockHistory.HistoricDataToday.Price * upperMovementMultiplier;

            output.AppendLine($"Buy price  applying movement% = {buyPrice.ToString("##.##")}");
            output.AppendLine($"Sell price applying movement% = {sellPrice.ToString("##.##")}");

            buyPrice = buyPrice * totalMetric;
            sellPrice = sellPrice * totalMetric;

            output.AppendLine($"Buy price  applying total metric = {buyPrice}");
            output.AppendLine($"Sell price applying total metric = {sellPrice}");

             ///////// Limit price so it's with a range
            float limitPrice = (stockSummary.Price * lowerMovementMultiplier) * .8F; // lower buy limit price to low
            if (buyPrice < limitPrice)
                buyPrice = limitPrice;
            limitPrice = (stockSummary.Price * lowerMovementMultiplier) * 1.1F; // upper buy limit
            if (buyPrice > limitPrice)
                buyPrice = limitPrice;
            limitPrice = (stockSummary.Price * upperMovementMultiplier) * .9F; // lower sell limit
            if (sellPrice < limitPrice)
                sellPrice = limitPrice;
            limitPrice = (stockSummary.Price * upperMovementMultiplier) * 1.1F; // upper sell limit
            if (sellPrice > limitPrice)
                sellPrice = limitPrice;

            BuyPrice = buyPrice;
            SellPrice = sellPrice;


            //////////////////////////////////
            //      Quantity to Buy/Sell
            //////////////////////////////////
            float buyQuantity = 0F;
            float sellQuantity = 0F;


            if (analyzeInputs.LastTradeBuySell == BuyOrSell.Buy)
            {
                buyQuantity = analyzeInputs.QuantityTraded * .8F; // Buy less if you just bought
                sellQuantity = analyzeInputs.QuantityTraded * 1.08F; // Sell more if you just bought
            }
            else
            {
                buyQuantity = analyzeInputs.QuantityTraded * 1.08F; // Buy more if you just sold
                sellQuantity = analyzeInputs.QuantityTraded * .8F; // Sell less if you just sold
            }

            // Dividend 
            if (stockSummary.Dividend > 8)
            {
                buyQuantity = buyQuantity * 1.2F;
                sellQuantity = sellQuantity / 1.2F;
            }
            else if (stockSummary.Dividend > 4)
            {
                buyQuantity = buyQuantity * 1.1F;
                sellQuantity = sellQuantity / 1.1F;
            }
            else if (stockSummary.Dividend > 2)
            {
                buyQuantity = buyQuantity * 1.05F;
                sellQuantity = sellQuantity / 1.05F;
            }

            BuyQuantity = Convert.ToInt32(buyQuantity);
            SellQuantity = Convert.ToInt32(sellQuantity);

            if (SellQuantity > Convert.ToInt16(analyzeInputs.SharesOwned / 2.4F)) // Don't sell close to more than half your shares
                SellQuantity = Convert.ToInt16((analyzeInputs.SharesOwned / 2.4F) + .5F);

            AnalysisMetricsOutputText = output.ToString();
        }

        public class AnalyzeInputs
        {
            public int SharesOwned;
            public BuyOrSell LastTradeBuySell;
            public int QuantityTraded;
            public float SharesTradedPrice;
            public float MovementTargetPercent;
        }
    }
}

