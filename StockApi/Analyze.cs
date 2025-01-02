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

        public void AnalyzeStockData(StockSummary stockSummary, StockHistory stockHistory, StockFinancials stockFinancials, AnalyzeInputs analyzeInputs)
        {
            StringBuilder output = new StringBuilder();
            // combine trends with
            // one year target
            // EPS
            // Fair Value
            // Profit Margin %
            // Volatility

            // Long Term Price Trend
            float priceTrendMetric = 1F;
            if (stockHistory.HistoricDataToday.Price > stockHistory.HistoricData3YearsAgo.Price * 1.6F)
                priceTrendMetric = 1.1F;
            else if (stockHistory.HistoricDataToday.Price > stockHistory.HistoricData3YearsAgo.Price * 1.2F)
                priceTrendMetric = 1.05F;
            else if (stockHistory.HistoricDataToday.Price > stockHistory.HistoricData3YearsAgo.Price * 1.1F)
                priceTrendMetric = 1.01F;
            else if (stockHistory.HistoricDataToday.Price < stockHistory.HistoricData3YearsAgo.Price * 0.6F)
                priceTrendMetric = .9F;
            else if (stockHistory.HistoricDataToday.Price < stockHistory.HistoricData3YearsAgo.Price * 0.8F)
                priceTrendMetric = .96F;
            else if (stockHistory.HistoricDataToday.Price < stockHistory.HistoricData3YearsAgo.Price * 0.93F)
                priceTrendMetric = .98F;


            output.AppendLine($"Price Trend Metric = {priceTrendMetric}");

            // One Year Target - Not a very valuable metric. 
            float targetPriceMetric = 1F;
            if (stockSummary.OneYearTargetPrice < stockSummary.Price * .95)
                targetPriceMetric = .99F;
            else if (stockSummary.OneYearTargetPrice > stockSummary.Price * 1.05)
                targetPriceMetric = 1.01F;

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
                epsMetric = 1.05F;
            else if (stockSummary.EarningsPerShare > 3)
                epsMetric = 1.03F;
            else if (stockSummary.EarningsPerShare > 1)
                epsMetric = 1.02F;
            else if (stockSummary.EarningsPerShare > 0)
                epsMetric = 1.01F;

            output.AppendLine($"Earnings Metric = {epsMetric}");

            // Price / Book
            float priceBookMetric = 1F;
            if (stockSummary.PriceBookColor == Color.Red)
                priceBookMetric = .982F;
            if (stockSummary.PriceBookColor == Color.Lime)
                priceBookMetric = 1.02F;

            output.AppendLine($"Price Book Metric = {priceBookMetric}");

            // Profit Margin Metric
            float ProfitMarginMetric = 1F;
            if (stockSummary.ProfitMargin < -6)
                ProfitMarginMetric = .962F;
            else if (stockSummary.ProfitMargin < -2)
                ProfitMarginMetric = .981F;
            else if (stockSummary.ProfitMargin > 6)
                ProfitMarginMetric = 1.04F;
            else if (stockSummary.ProfitMargin > 2)
                ProfitMarginMetric = 1.02F;

            output.AppendLine($"Profit Margin Metric = {ProfitMarginMetric}");

            // Dividend Metric
            float dividendMetric = 1F;
            if (stockSummary.Dividend > 8)
                dividendMetric = 1.08F;
            else if (stockSummary.Dividend > 5)
                dividendMetric = 1.05F;
            else if (stockSummary.Dividend > 2)
                dividendMetric = 1.02F;
            else if (stockSummary.Dividend > .5)
                dividendMetric = 1.01F;
            else
                dividendMetric = .99F;

            if (priceTrendMetric < .92F && dividendMetric > 1.04)
                dividendMetric = 1.01F; // if the price is going steeply down, who care about a high dividend

output.AppendLine($"Dividend Metric = {dividendMetric}");

            ///////////////////////////////////// Finacial Metrics
            // Revenue - Should be increasing YOY
            float revenueMetric = 1F;
            if (stockFinancials.RevenueTtm > 0)
            {
                if (stockFinancials.Revenue2 > stockFinancials.Revenue4 * 1.05) // Revenue 2 years ago is 5% above revenue 4 years ago 
                    revenueMetric = 1.025F;
                if (stockFinancials.Revenue2 < stockFinancials.Revenue4 * .98) // Revenue 2 years ago is 1% below revenue 4 years ago 
                    revenueMetric = .98F;
                if (stockFinancials.RevenueTtm > stockFinancials.Revenue2 * 1.05) // Revenue TTM is 5% above revenue 2 years ago 
                    revenueMetric = revenueMetric * 1.025F;
                if (stockFinancials.RevenueTtm < stockFinancials.Revenue2 * .98) // Revenue TTM is 1% below revenue 2 years ago 
                    revenueMetric = revenueMetric * .98F;
                if (stockFinancials.RevenueTtm > stockFinancials.Revenue4 * 1.03) // Revenue TTM is 5% above revenue 4 years ago 
                    revenueMetric = revenueMetric * 1.01F;
                if (stockFinancials.RevenueTtm < stockFinancials.Revenue4 * .98) // Revenue TTM is 1% below revenue 4 years ago 
                    revenueMetric = revenueMetric * .99F;
            }
            output.AppendLine($"Revenue Metric = {revenueMetric}");
            // Profit - Revenue - Cost of Revenue
            float profitMetric = 1F;
            if (stockFinancials.Profit2YearsAgo > stockFinancials.Profit4YearsAgo * 1.01)
                profitMetric = 1.025F;
            if (stockFinancials.Profit2YearsAgo < stockFinancials.Profit4YearsAgo * .99)
                profitMetric = .98F;
            if (stockFinancials.ProfitTTM > stockFinancials.Profit2YearsAgo * 1.01)
                profitMetric *= 1.025F;
            if (stockFinancials.ProfitTTM < stockFinancials.Profit2YearsAgo * .99)
                profitMetric *= .98F;
            if (stockFinancials.ProfitTTM > stockFinancials.Profit4YearsAgo * 1.2)
                profitMetric *= 1.015F;
            else if (stockFinancials.ProfitTTM > stockFinancials.Profit4YearsAgo * 1.02)
                profitMetric *= 1.01F;
            if (stockFinancials.ProfitTTM < stockFinancials.Profit4YearsAgo * .98)
                profitMetric *= .985F;
            if (revenueMetric * profitMetric < .87)
                output.AppendLine($"Profit Metric = {profitMetric}         * Financials are Bad *");
            else
                output.AppendLine($"Profit Metric = {profitMetric}");

            float cashDebtMetric = 1F;
            if (stockFinancials.TotalDebt > stockFinancials.TotalCash * 5) // lots of debt compared to cash
                cashDebtMetric = .96F;
            else if (stockFinancials.TotalCash > stockFinancials.TotalDebt * 2) // lots of cash compared to debt
                cashDebtMetric = 1.03F;
            if (stockFinancials.DebtEquity > 120) // Over 120% D/E is bad
                cashDebtMetric = cashDebtMetric * .96F;
            output.AppendLine($"Cash, Debt Metric = {cashDebtMetric}");

            // Economy
            float ecoMetric =  1 + ((analyzeInputs.EconomyHealth - 5) / 100);
            output.AppendLine($"Economy Metric = {ecoMetric}");

            float totalMetric = priceTrendMetric * targetPriceMetric * epsMetric * priceBookMetric * dividendMetric * ProfitMarginMetric * ecoMetric * revenueMetric * profitMetric * cashDebtMetric;
            output.AppendLine($"----------------------------------------------------");
            string totalMetricString = $"Total Metric = {totalMetric}";
            if (totalMetric < .85F)
            {
                totalMetric = .85F;
                totalMetricString += $"  low end limited to {totalMetric}";
            }
            if (totalMetric > 1.2)
            {
                totalMetric = 1.2F;
                totalMetricString += $"  high end limited to {totalMetric}";
            }

            output.AppendLine(totalMetricString);

            output.AppendLine("");


            ///////////// Setting Price Movement Multipliers
            // Gets the volatility number closer to 1, less exxtreme. 2.6 becomes 1.5
            double volitilityFactor = 1; // Math.Log((Math.Log10(stockSummary.Volatility) + 1)) + 1; 
            if (stockSummary.Volatility < .5)
                volitilityFactor = .86;
            else if (stockSummary.Volatility < .8)
                volitilityFactor = .93;
            else if (stockSummary.Volatility > 2)
                volitilityFactor = 1.14;
            else if (stockSummary.Volatility > 1.2)
                volitilityFactor = 1.07;
            output.AppendLine($"Volitility Factor = { volitilityFactor.ToString("##.##")}");

            analyzeInputs.MovementTargetPercent *= (float)volitilityFactor; // Applying volatility
            output.AppendLine($"Movement % w/ Volatility = { analyzeInputs.MovementTargetPercent.ToString("##.##")}%");
            
            // if stock is like XOM where your not sell a large percentage of shares owned
            // Make the lower and upper movement less
            if(analyzeInputs.QuantityTraded < analyzeInputs.SharesOwned / 4)
            {
                analyzeInputs.MovementTargetPercent = analyzeInputs.MovementTargetPercent * .9F;
                output.AppendLine($"Movement reduced, low share quantity = {analyzeInputs.MovementTargetPercent.ToString("##.##")}");
            }

            float lowerMovementMultiplier = ((100F - (analyzeInputs.MovementTargetPercent * .7F)) / 100F); // if movement is 20% will assign .88 because a down percentage is greater than an up
            float upperMovementMultiplier = ((100F + analyzeInputs.MovementTargetPercent) / 100F); // if movement is 20% will assign 1.2

            // Calculate future buy or sells based on how much it moves and what our target movement is. 
            float buyPrice = stockHistory.HistoricDataToday.Price * lowerMovementMultiplier;
            float sellPrice = stockHistory.HistoricDataToday.Price * upperMovementMultiplier;

            output.AppendLine($"Buy price  applying movement% = {buyPrice.ToString("##.##")}");
            output.AppendLine($"Sell price applying movement% = {sellPrice.ToString("##.##")}");

            buyPrice = buyPrice * totalMetric;
            sellPrice = sellPrice * totalMetric;

            if (sellPrice < stockHistory.HistoricDataToday.Price)
                sellPrice = stockHistory.HistoricDataToday.Price * 1.05F; // Sell a bad stock at a 5% profit if it ever gets there.
            if (buyPrice > stockHistory.HistoricDataToday.Price * .96F)
                buyPrice = stockHistory.HistoricDataToday.Price * .95F; // Next buy can't be more than current price, but buy less

            output.AppendLine($"Buy price  applying total metric = {buyPrice.ToString("##.##")}");
            output.AppendLine($"Sell price applying total metric = {sellPrice.ToString("##.##")}");

             ///////// Limit price so it's with a range
            float limitPrice = (stockSummary.Price * lowerMovementMultiplier) * .5F; // lower buy limit price too low
            if (buyPrice < limitPrice)
                buyPrice = limitPrice;
            //limitPrice = (stockSummary.Price * lowerMovementMultiplier) * 1.3F; // upper buy limit
            //if (buyPrice > limitPrice)
            //    buyPrice = limitPrice;
            //limitPrice = (stockSummary.Price * upperMovementMultiplier) * .6F; // lower sell limit
            //if (sellPrice < limitPrice)
            //    sellPrice = limitPrice;
            limitPrice = (stockSummary.Price * upperMovementMultiplier) * 1.4F; // upper sell limit
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
            if (stockSummary.Dividend > 8 && priceTrendMetric > .92F)
            {
                buyQuantity = buyQuantity * 1.2F;
                sellQuantity = sellQuantity / 1.2F;
                output.AppendLine($"Great dividend. Buy more or sell less. {buyQuantity.ToString("##.##")} {sellQuantity.ToString("##.##")}");
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

            if (buyPrice > stockHistory.HistoricDataToday.Price * .94F)
            {
                buyQuantity = buyQuantity * .8F; // Next buy can't be more than current price, but buy less
                output.AppendLine($"Accumulate this stock slowly.");
            }

            // Year price trend. Lower and Raise based on that.
            if (stockHistory.YearTrend == StockHistory.TrendEnum.Up)
            {
                buyQuantity = buyQuantity * 1.05F;
                sellQuantity = sellQuantity / 1.05F;
                output.AppendLine($"Year up trend applied. {buyQuantity.ToString("##.##")} {sellQuantity.ToString("##.##")}");
            }
            if (stockHistory.YearTrend == StockHistory.TrendEnum.Down)
            {
                buyQuantity = buyQuantity * .95F;
                sellQuantity = sellQuantity / .95F;
                output.AppendLine($"Year down trend applied. {buyQuantity.ToString("##.##")} {sellQuantity.ToString("##.##")}");
            }

            BuyQuantity = Convert.ToInt32(buyQuantity);
            SellQuantity = Convert.ToInt32(sellQuantity);

            if (SellQuantity > Convert.ToInt16(analyzeInputs.SharesOwned / 2.4F)) // Don't sell close to more than half your shares
                SellQuantity = Convert.ToInt16((analyzeInputs.SharesOwned / 2.4F) + .5F);

            // Minimum profit of $30. if selling 10 shares, sell at least $3 increase in price.
            float profit = (SellPrice - stockSummary.Price) * (float)SellQuantity;
            if (profit < 20F && totalMetric > .91F) // if it's a bad stock we are liquidating and the $20 profit doesn't matter. 
            {
                SellPrice = (20F / SellQuantity) + stockSummary.Price;
                output.AppendLine($"Minimum profit set to $20. Sell Price: {SellPrice.ToString("##.##")}");
            }

            AnalysisMetricsOutputText = output.ToString();
        }

        public class AnalyzeInputs
        {
            public int SharesOwned;
            public BuyOrSell LastTradeBuySell;
            public int QuantityTraded;
            public float SharesTradedPrice;
            public float MovementTargetPercent;
            public float EconomyHealth;
        }
    }
}

