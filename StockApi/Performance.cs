using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Drake.Extensions;
using PC = StockApi.ExcelManager.PositionColumns;
using TC = StockApi.ExcelManager.TradeColumns;

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
            bool buyAndSold;

            // Get latest DOW level from trades, replace that latest trade's DOW level with this.
            int dowLast;
            if (dowMarket.CurrentLevel.NumericValue > 0)
                dowLast = Convert.ToInt32(dowMarket.CurrentLevel.NumericValue);
            else
            {
                string dow = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.DowLevel].ToString().Trim() != "0" && x[(int)TC.DowLevel].ToString().Trim() != "").Last().ItemArray[(int)TC.DowLevel].ToString();
                dowLast = 0;
                if (dow._IsDecimal())
                {
                    dowLast = Convert.ToInt32(dow);
                }
            }

            // Get last 60 sells to elimate from the buy list
            IEnumerable<DataRow> sells = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Sell" && x[(int)TC.TradeDate].ToString().Trim() != "" && x[(int)TC.QuantityHeld].ToString().Trim() != "0");
            sells = sells.OrderByDescending(x => x[(int)TC.TradeDate]).Take(60).OrderBy(x => x[(int)TC.TradeDate]);

            // Get last 25 buys
            EnumerableRowCollection<DataRow> positions = positionsDataTable.AsEnumerable().Where(x => x[(int)TC.DowLevel].ToString().Trim() != "0" && x[(int)TC.DowLevel].ToString().Trim() != "");

            IEnumerable<DataRow> tickerTrades = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Buy" && x[(int)TC.DowLevel].ToString().Trim() != "").Skip(700);
            tickerTrades = tickerTrades.OrderByDescending(x => x[(int)TC.TradeDate]).Take(25).OrderBy(x => x[(int)TC.TradeDate]);

            _performanceList.Clear();
            foreach (DataRow trade in tickerTrades)
            {
                // Search in positions for ticker to get current price 
                string ticker = trade.ItemArray[(int)TC.Ticker].ToString();
                string temp = trade.ItemArray[(int)TC.QuantityTraded].ToString();
                int quantity = 0;
                if (temp._IsInt())
                {
                    quantity = Convert.ToInt32(temp);
                }


                try
                {
                    temp = positions.Where(x => x[(int)PC.Ticker].ToString() == ticker).First().ItemArray[(int)PC.Price].ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Couldn't find the ticker {ticker} that was in buy trades in the position table.");
                    continue;
                }
                
                decimal currentPrice = 0;
                if (temp._IsDecimal())
                {
                    currentPrice = Convert.ToDecimal(temp);
                }

                // See if there was a later sell off and eliminate it
                buyAndSold = false;
                if (sells.Where(x => x[(int)TC.Ticker].ToString() == ticker).Count() > 0)
                {
                    var sell = sells.Where(x => x[(int)TC.Ticker].ToString() == ticker).Last();

                    if ((DateTime)sell.ItemArray[(int)TC.TradeDate] > (DateTime)trade.ItemArray[(int)TC.TradeDate])
                    {
                        currentPrice = Convert.ToDecimal(sell.ItemArray[(int)TC.TradePrice]);
                        buyAndSold = true;
                    }
                }

                // Get the buy price
                temp = trade.ItemArray[(int)TC.TradePrice].ToString();
                decimal buyPrice = 0;
                if (temp._IsDecimal())
                {
                    buyPrice = Convert.ToDecimal(temp);
                }

                // profit/ loss   
                decimal profit = currentPrice - buyPrice;

                // Get the DOW level
                temp = trade.ItemArray[1].ToString();
                int dowLevel = 0;
                if (temp._IsDecimal())
                {
                    dowLevel = Convert.ToInt32(temp);
                }

                PerformanceItem pi = new PerformanceItem()
                {
                    TradeDate = Convert.ToDateTime(trade.ItemArray[0].ToString()),
                    Ticker = ticker,
                    Quantity = quantity,
                    TradePrice = buyPrice,
                    CurrentPrice = currentPrice,
                    Profit = profit,
                    TotalProfit = quantity * profit,
                    DowLevel = dowLevel,
                    SoldAndBought = buyAndSold
                };

                _performanceList.Add(pi);
            }
            _performanceList.Last().DowLevel = dowLast;
        }
        public void ShowPerformanceForm(Form1 form1)
        {
            PerformanceForm pf = new PerformanceForm(_performanceList);
            pf.Owner = form1;
            pf.Show();
        }

        public List<PerformanceItem> GetLatestSellPerformance(DataTable positionsDataTable, DataTable tradesDataTable)
        {
            StockHistory stockHistory = new StockHistory();
            List<PerformanceItem> performanceList = new List<PerformanceItem>();
            bool soldAndBought = false;

            // Get all positions to get current price
            EnumerableRowCollection<DataRow> positions = positionsDataTable.AsEnumerable().Where(x => x[(int)PC.QuantityHeld].ToString().Trim() != "0" && x[(int)PC.QuantityHeld].ToString().Trim() != "");

            // Get last 60 buys to elimate fro the sell list
            IEnumerable<DataRow> buys = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Buy" && x[(int)TC.TradeDate].ToString().Trim() != "" && x[(int)TC.QuantityHeld].ToString().Trim() != "0");
            buys = buys.OrderByDescending(x => x[(int)TC.TradeDate]).Take(60).OrderBy(x => x[(int)TC.TradeDate]);

            // Get last 25 sells
            IEnumerable<DataRow> sellTrades = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Sell" && x[(int)TC.TradeDate].ToString().Trim() != "" && x[(int)TC.QuantityHeld].ToString().Trim() != "0");
            //tickerTrades = tickerTrades.Skip(tickerTrades.Count() - 30);
            sellTrades = sellTrades.OrderByDescending(x => x[(int)TC.TradeDate]).Take(25).OrderBy(x => x[(int)TC.TradeDate]);

            performanceList.Clear();
            foreach (DataRow trade in sellTrades)
            {
                string ticker = trade.ItemArray[(int)TC.Ticker].ToString();

                // Search in trades for ticker to get quantity sold
                string temp = trade.ItemArray[(int)TC.QuantityTraded].ToString();
                int quantity = 0;
                if (temp._IsInt())
                {
                    quantity = Convert.ToInt32(temp);
                }

                // Search in positions for ticker to get current price 
                if(positions.Where(x => x[(int)PC.Ticker].ToString() == ticker).Count() == 0)
                {
                    continue; // Sold a stock that has been liquidated and has no current price in the positions table
                }
                    
                temp = positions.Where(x => x[(int)PC.Ticker].ToString() == ticker).First().ItemArray[(int)PC.Price].ToString();
                decimal currentPrice = 0;
                if (temp._IsDecimal())
                {
                    currentPrice = Convert.ToDecimal(temp);
                }
                else
                {
                    currentPrice = 0M;
                }

                // See if there was a later buyback and eliminate it
                soldAndBought = false;
                if (buys.Where(x => x[(int)TC.Ticker].ToString() == ticker).Count() > 0)
                {
                    var buy = buys.Where(x => x[(int)TC.Ticker].ToString() == ticker).Last();

                    if ((DateTime)buy.ItemArray[(int)TC.TradeDate] > (DateTime)trade.ItemArray[(int)TC.TradeDate])
                    {
                        currentPrice = Convert.ToDecimal(buy.ItemArray[(int)TC.TradePrice]);
                        soldAndBought = true;
                    }
                }

                // Get the price sold from trades table
                temp = trade.ItemArray[(int)TC.TradePrice].ToString();
                decimal soldPrice = 0;
                if (temp._IsDecimal())
                {
                    soldPrice = Convert.ToDecimal(temp);
                }

                // profit/ loss   
                decimal profit = soldPrice - currentPrice;

                PerformanceItem pi = new PerformanceItem()
                {
                    TradeDate = Convert.ToDateTime(trade.ItemArray[0].ToString()),
                    Ticker = ticker,
                    Quantity = quantity,
                    TradePrice = soldPrice,
                    CurrentPrice = currentPrice,
                    Profit = profit,
                    TotalProfit = quantity * profit,
                    DowLevel = 0,
                    SoldAndBought = soldAndBought
                };

                performanceList.Add(pi);
            }

            return performanceList;
        }

        public async Task<List<PerformanceItem>> GetLiquidationPerformance(DataTable tradesDataTable)
        {
            StockHistory stockHistory = new StockHistory();
            List<PerformanceItem> performanceList = new List<PerformanceItem>();

            // Get liquidations for this last year
            IEnumerable<DataRow> tickerTrades = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Sell" && x[(int)TC.TradeDate].ToString().Trim() != "" && Convert.ToDateTime(x[(int)TC.TradeDate]) > DateTime.Now.AddYears(-1) && x[(int)TC.QuantityHeld].ToString().Trim() == "0").Skip(5);
            tickerTrades = tickerTrades.OrderBy(x => x[(int)TC.TradeDate]).Take(25);

            performanceList.Clear();
            foreach (DataRow trade in tickerTrades)
            {
                // Search in positions for ticker to get current price 
                string ticker = trade.ItemArray[(int)TC.Ticker].ToString();
                string temp = trade.ItemArray[(int)TC.QuantityTraded].ToString();
                int quantity = 0;
                if (temp._IsInt())
                {
                    quantity = Convert.ToInt32(temp);
                }

                // Get current price for sold stock
                decimal currentPrice = await stockHistory.GetTodaysPrice(ticker);

                if(currentPrice == 0)
                {
                    currentPrice = 999;
                }


                // Get the price sold
                temp = trade.ItemArray[(int)TC.TradePrice].ToString();
                decimal soldPrice = 0;
                if (temp._IsDecimal())
                {
                    soldPrice = Convert.ToDecimal(temp);
                }

                // profit/ loss   
                decimal profit = soldPrice - currentPrice;

                PerformanceItem pi = new PerformanceItem()
                {
                    TradeDate = Convert.ToDateTime(trade.ItemArray[0].ToString()),
                    Ticker = ticker,
                    Quantity = quantity,
                    TradePrice = soldPrice,
                    CurrentPrice = currentPrice,
                    Profit = profit,
                    TotalProfit = quantity * profit,
                    DowLevel = 0
                };

                performanceList.Add(pi);
            }

            return performanceList;
        }
        public void ShowLiquidationPerformanceForm(Form1 form1, List<PerformanceItem> performanceList, string title, int formType)
        {
            PerformanceFormLiquidations pf = new PerformanceFormLiquidations(performanceList);
            pf.Text = title;
            pf.Owner = form1;
            pf.formType = formType;
            pf.Show();
        }
    }

    public class PerformanceItem
    {
        public DateTime TradeDate { get; set; }
        public string   Ticker { get; set; }
        public int      Quantity { get; set; }
        public decimal  TradePrice { get; set; }
        public decimal  CurrentPrice { get; set; }
        public decimal  Profit { get; set; }
        public decimal  TotalProfit { get; set; }
        public int      DowLevel { get; set; }
        public bool     SoldAndBought { get; set; }
    }
}
