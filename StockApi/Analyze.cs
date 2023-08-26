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
        public string AnalysisOutput;

        public void AnalyzeStockData(StockSummary stockSummary, StockHistory stockHistory, AnalyzeInputs analyzeInputs)
        {
            StringBuilder output = new StringBuilder();
            // combine trends with
            // one year target
            // EPS
            // Fair Value
            // Estimated Return %
            // Volatility

            // Trend
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
            else if (trendMetric == 5)
                trendMetric = 1.12F; // Very upward trend

            output.AppendLine($"Price Trend Metric = {trendMetric}");

            // One Year Target
            float targetPriceMetric = 1F;
            if (stockSummary.OneYearTargetPrice < stockSummary.Price * .9)
                targetPriceMetric = .9F;
            else if (stockSummary.OneYearTargetPrice > stockSummary.Price * 1.1)
                targetPriceMetric = 1.1F;

            output.AppendLine($"One Year Target Metric = {targetPriceMetric}");

            // Earnings Per Share
            float epsMetric = 1F;
            if (stockSummary.EarningsPerShare < -1)
                epsMetric = .92F;
            else if (stockSummary.EarningsPerShare > 1)
                epsMetric = 1.08F;

            output.AppendLine($"Earnings Metric = {epsMetric}");

            // Fair Value
            float fairValueMetric = 1F;
            if (stockSummary.FairValueColor == Color.Red)
                fairValueMetric = .9F;
            if (stockSummary.FairValueColor == Color.Lime)
                fairValueMetric = 1.1F;

            output.AppendLine($"Fair Value Metric = {fairValueMetric}");

            // One Year Target
            float oneYearTargetMetric = 1F;
            if (stockSummary.OneYearTargetColor == Color.Red)
                oneYearTargetMetric = .9F;
            else if (stockSummary.OneYearTargetColor == Color.Lime)
                oneYearTargetMetric = 1.1F;

            output.AppendLine($"One Year Target Metric = {oneYearTargetMetric}");

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

            float totalMetric = trendMetric * targetPriceMetric * epsMetric * fairValueMetric * dividendMetric;

            output.AppendLine($"Total Metric = {totalMetric}");

            // Volatility  Change movement % 
            analyzeInputs.MovementTargetPercent *= Convert.ToSingle(stockSummary.Volatility);
            output.AppendLine($"Movement % w/ Volatility = { analyzeInputs.MovementTargetPercent}");

            // Calculate future buy or sells
            float buyPrice = stockHistory.HistoricDataToday.Price * ((100F - analyzeInputs.MovementTargetPercent) / 100F);
            float sellPrice = stockHistory.HistoricDataToday.Price * ((100F + analyzeInputs.MovementTargetPercent) / 100F);

            // Apply metrics
            buyPrice = buyPrice * totalMetric;
            sellPrice = sellPrice * totalMetric;

            if(stockSummary.EarningsPerShare > 1)
            {
                double f = (stockSummary.EarningsPerShare + 9) / 10;
                double g = Math.Log10(f) + 1D;
                double newPrice = buyPrice * g;
                float limitPrice = stockSummary.Price * ((100F - analyzeInputs.MovementTargetPercent / 2) / 100F);

                // Don't let newPrice go above half way between current price and normal buy price.
                if (newPrice > limitPrice)
                    newPrice = limitPrice;

                buyPrice = (float)newPrice;
            }

            // Buy Quantity
            if (analyzeInputs.LastTradeBuySell == BuyOrSell.Buy)
            {
                BuyQuantity = Convert.ToInt16(analyzeInputs.SharesTraded * .8F); // Buy less if you just bought
                SellQuantity = Convert.ToInt16(analyzeInputs.SharesTraded * 1.1F); // Sell more if you just bought
                if (SellQuantity > Convert.ToInt16(analyzeInputs.SharesTraded / 2.5F))
                    SellQuantity = Convert.ToInt16(analyzeInputs.SharesTraded / 2.5F);
            }
            else
            {
                BuyQuantity = Convert.ToInt16(analyzeInputs.SharesTraded * 1.2F); // Buy more if you just sold
                SellQuantity = Convert.ToInt16(analyzeInputs.SharesTraded * .8F); // Buy less if you just sold
            }

            // Math.Log(21.1, 10) + 1

            BuyPrice = buyPrice;

            AnalysisOutput = output.ToString();
        }

        public class AnalyzeInputs
        {
            public int SharesOwned;
            public BuyOrSell LastTradeBuySell;
            public int SharesTraded;
            public float SharesTradedPrice;
            public float MovementTargetPercent;
        }
    }
}

