using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Drake.Extensions;

namespace StockApi
{
    public class Performance
    {
        private List<PerformanceItem> _performanceList = new List<PerformanceItem>();
        StockSummary _stockSummary;

        public Performance(StockSummary stockSummary)
        {
            _stockSummary = stockSummary;
        }

        public void GetLatestBuyPerformance(MarketData dowMarket, DataTable positionsDataTable, DataTable tradesDataTable)
        {
            int DateColumn = 0;

            // Get latest DOW level from trades, replace that latest trade's DOW level with this.
            int dowLast;
            if(dowMarket.CurrentLevel.NumericValue > 0)
                dowLast = Convert.ToInt32(dowMarket.CurrentLevel.NumericValue);
            else
            {
                string dow = tradesDataTable.AsEnumerable().Where(x => x[1].ToString().Trim() != "0" && x[1].ToString().Trim() != "").Last().ItemArray[1].ToString();
                dowLast = 0;
                if (dow._IsDecimal())
                {
                    dowLast = Convert.ToInt32(dow);
                }
            }

            // Get last 25 buys
            EnumerableRowCollection<DataRow> positions = positionsDataTable.AsEnumerable().Where(x => x[1].ToString().Trim() != "0" && x[1].ToString().Trim() !="");

            IEnumerable<DataRow> tickerTrades = tradesDataTable.AsEnumerable().Where(x => x[2].ToString() == "Buy" && x[1].ToString().Trim() != "").Skip(700);
            tickerTrades = tickerTrades.OrderByDescending(x => x[DateColumn]).Take(25);

            _performanceList.Clear();
            foreach (DataRow dr in tickerTrades)
            {
                // Search in positions for ticker to get current price 
                
                string ticker = dr.ItemArray[4].ToString();
                string temp = dr.ItemArray[3].ToString();
                int quantity = 0;
                if (temp._IsInt())
                {
                    quantity = Convert.ToInt32(temp);
                }

                temp = positions.Where(x => x[0].ToString() == ticker).First().ItemArray[2].ToString();
                decimal currentPrice = 0;
                if(temp._IsDecimal())
                {
                    currentPrice = Convert.ToDecimal(temp);
                }

                // Get the buy price
                temp = dr.ItemArray[5].ToString();
                decimal buyPrice = 0;
                if (temp._IsDecimal())
                {
                    buyPrice = Convert.ToDecimal(temp);
                }

                // profit/ loss   
                decimal profit = currentPrice - buyPrice;

                // Get the DOW level
                temp = dr.ItemArray[1].ToString();
                int dowLevel = 0;
                if (temp._IsDecimal())
                {
                    dowLevel = Convert.ToInt32(temp);
                }

                PerformanceItem pi = new PerformanceItem()
                {
                    BuyDate = Convert.ToDateTime(dr.ItemArray[0].ToString()),
                    Ticker = ticker,
                    Quantity = quantity,
                    BuyPrice = buyPrice,
                    CurrentPrice = currentPrice,
                    Profit = profit,
                    TotalProfit = quantity * profit,
                    DowLevel = dowLevel
                };

                _performanceList.Add(pi);
            }
            _performanceList.First().DowLevel = dowLast;
        }
        public void ShowPerformanceForm(Form1 form1)
        {
            PerformanceForm pf = new PerformanceForm(_performanceList);
            pf.Owner = form1;
            pf.Show();
        }
    }

    public class PerformanceItem
    {
        public DateTime BuyDate { get; set; }
        public string Ticker { get; set; }
        public int Quantity { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal Profit { get; set; }
        public decimal TotalProfit { get; set; }
        public int DowLevel { get; set; }
    }
}
