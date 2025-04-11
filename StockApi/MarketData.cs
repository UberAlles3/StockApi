using Drake.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StockApi
{
    public class MarketData
    {
        public DateTime RetreivedDate { get; set; }
        public string Ticker { get; set; } = "";
        public StringSafeNumeric<decimal> PreviousClose { get; set; } = new StringSafeNumeric<decimal>("");
        public StringSafeNumeric<decimal> CurrentLevel { get; set; } = new StringSafeNumeric<decimal>("");
        public decimal Change
        {
            get => CurrentLevel.NumericValue - PreviousClose.NumericValue;
        }
        public decimal PercentageChange
        {
            get
            {
                decimal change = 0;
                
                if(PreviousClose.NumericValue > 0 )
                    change = ((CurrentLevel.NumericValue - PreviousClose.NumericValue) / PreviousClose.NumericValue) * 100;
                
                return change;  
            }
        }

        public Color MarketColor
        {
            get
            {
                Color color = Color.LightSteelBlue;

                if (Change > PreviousClose.NumericValue / 1000)
                    color = Color.LimeGreen;
                else if (Change < -PreviousClose.NumericValue / 1000)
                    color = Color.Red;

                return color;
            }
        }

        public static MarketData GetMarketData(StockSummary stockSummary, string html, string searchTerm)
        {
            MarketData marketData = new MarketData();
            marketData.RetreivedDate = DateTime.Now;

            string htmlSnippet = "";
            searchTerm = StockSummary.SearchTerms.Find(x => x.Name == searchTerm).Term;
            marketData.Ticker = searchTerm.Replace("\\", "");
            htmlSnippet = stockSummary.GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 1500);
            if (htmlSnippet.Length > 200)
            {
                string temp = stockSummary.GetPartialHtmlFromHtmlBySearchTerm(htmlSnippet, "regularMarketPrice", 100);
                if (temp.Length > 20)
                {
                    marketData.CurrentLevel.StringValue = temp.Substring(19, 12).Replace(":", "")._TrimSuffix(".");
                    temp = stockSummary.GetPartialHtmlFromHtmlBySearchTerm(htmlSnippet, "previousClose", 100);
                    if (temp.Length > 20)
                    {
                        marketData.PreviousClose.StringValue = temp.Substring(14, 12).Replace(":", "")._TrimSuffix(".");
                    }
                }
            }

            return marketData;
        }
    }
}
