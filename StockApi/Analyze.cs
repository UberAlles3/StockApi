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
    class Analyze
    {
        public enum BuyOrSell
        {
            Buy,
            Sell
        }

        static TextFile TF = new TextFile();
        static List<PersonalStockData> personalStockDataList = new List<PersonalStockData>();

        static void ReadInStockData()
        {
            string[] parts;
            string currentFolder = Environment.CurrentDirectory;

            if (personalStockDataList.Count > 0)
                return;
            
            TF.OpenFile(currentFolder + @"\Stocks.txt", TextFile.TextFileMode.InputMode);

            foreach (string s in TF)
            {
                parts = s.Split("\t");

                PersonalStockData personalStockData = new PersonalStockData { Ticker = parts[0], SharesOwned = Convert.ToInt32(parts[1])};
                personalStockDataList.Add(personalStockData);

                Debug.WriteLine(s);
            }
            TF.CloseFile();
        }

        public static PersonalStockData PreFillAnalyzeFormData()
        {
            ReadInStockData();

            PersonalStockData personalStockData = personalStockDataList.Find(x => x.Ticker == StockSummary.Ticker);

            return personalStockData;
        }


        public static AnalyzeResults AnalyzeStockData(AnalyzeInputs analyzeInputs)
        {
            StringBuilder builder = new StringBuilder();
            // combine trends with
            // one year target
            // EPS
            // Fair Value
            // Estimated Return %
            // Should we read in excel file?
            // Volatility

            // Trend
            float trendMetric = Convert.ToInt16(StockHistory.YearTrend) + Convert.ToInt16(StockHistory.MonthTrend) + Convert.ToInt16(StockHistory.WeekTrend);
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

            builder.AppendLine($"Price Trend Metric = {trendMetric}");

            // One Year Target
            float targetPriceMetric = 1F;
            if (YahooFinance.OneYearTargetColor == Color.Red)
                targetPriceMetric = .9F;
            if (YahooFinance.OneYearTargetColor == Color.Lime)
                targetPriceMetric = 1.1F;

            // EPS
            float epsMetric = 1F;
            if (StockSummary.EPSColor == Color.Red)
                epsMetric = .9F;
            if (StockSummary.EPSColor == Color.Lime)
                epsMetric = 1.1F;

            // Fair Value
            float fairValueMetric = 1F;
            if (StockSummary.FairValueColor == Color.Red)
                fairValueMetric = .9F;
            if (StockSummary.FairValueColor == Color.Lime)
                fairValueMetric = 1.1F;


            float totalMetric = trendMetric * targetPriceMetric * epsMetric * fairValueMetric;

            float buyPrice = StockHistory.HistoricDataToday.Price * ((100F - analyzeInputs.MovementTargetPercent) / 100F);
            float sellPrice = StockHistory.HistoricDataToday.Price * ((100F + analyzeInputs.MovementTargetPercent) / 100F);
            // Apply metrics
            buyPrice = buyPrice * totalMetric;
            sellPrice = sellPrice * totalMetric;


            AnalyzeResults analyzeResults = new AnalyzeResults();
            if (analyzeInputs.LastTradeBuySell == BuyOrSell.Buy)
            {
                analyzeResults.BuyQuantity = Convert.ToInt16(analyzeInputs.SharesTraded * .8F); // Buy less if you just bought
                analyzeResults.SellQuantity = Convert.ToInt16(analyzeInputs.SharesTraded * 1.1F); // Sell more if you just bought
                if (analyzeResults.SellQuantity > Convert.ToInt16(analyzeInputs.SharesTraded / 2.5F))
                    analyzeResults.SellQuantity = Convert.ToInt16(analyzeInputs.SharesTraded / 2.5F);
            }
            else
            {
                analyzeResults.BuyQuantity = Convert.ToInt16(analyzeInputs.SharesTraded * 1.2F); // Buy more if you just sold
                analyzeResults.SellQuantity = Convert.ToInt16(analyzeInputs.SharesTraded * .8F); // Buy less if you just sold
            }

            analyzeResults.BuyPrice = buyPrice;

            analyzeResults.AnalysisOutput = builder.ToString();

            return analyzeResults;
        }

        public class PersonalStockData
        {
            private string ticker = "";
            private int sharesOwned;

            public string Ticker { get => ticker; set => ticker = value; }
            public int SharesOwned { get => sharesOwned; set => sharesOwned = value; }

            public override string ToString()
            {
                return string.Format(
                    $"{Ticker}, {SharesOwned}"
                );
            }
        }

        public class AnalyzeInputs
        {
            public int SharesOwned;
            public BuyOrSell LastTradeBuySell;
            public int SharesTraded;
            public float MovementTargetPercent;
        }

        public class AnalyzeResults
        {
            public int BuyQuantity;
            public float BuyPrice;
            public int SellQuantity;
            public float SellPrice = 0;
            public string AnalysisOutput;
        }
    }
}

