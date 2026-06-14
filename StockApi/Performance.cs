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
//using PC = StockApi.ExcelManager.PositionColumns;
using TC = StockApi.ExcelManager.TradeColumns;
using YahooLayer;

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

        public void GetLatestBuyPerformance(MarketData dowMarket, List<ExcelPosition> positionList, List<ExcelTrade> tradeList)
        {
            bool buyAndSold;

            // Get latest DOW level from trades, replace that latest trade's DOW level with this.
            int dowLast;
            if (dowMarket.CurrentLevel.NumericValue > 0)
                dowLast = Convert.ToInt32(dowMarket.CurrentLevel.NumericValue);
            else
            {
                //string dow = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.DowLevel].ToString().Trim() != "0" && x[(int)TC.DowLevel].ToString().Trim() != "").Last().ItemArray[(int)TC.DowLevel].ToString();
                string dow = tradeList.Last().DOW.ToString();

                dowLast = 0;
                if (dow._IsDecimal())
                {
                    dowLast = Convert.ToInt32(dow);
                }
            }

            // Get last 25 buys
            List<ExcelTrade> buyTrades = tradeList.Where(x => x.BuySell == "Buy" || x.BuySell == "Buy Shrt").OrderByDescending(x => x.TradeDate).Take(25).ToList();
            buyTrades = buyTrades.OrderByDescending(x => x.TradeDate).Take(25).OrderBy(x => x.TradeDate).ToList();

            _performanceList.Clear();
            foreach (ExcelTrade trade in buyTrades)
            {
                // Search in positions for ticker to get current price 
                string ticker = trade.Symbol;

                // Search in positions for ticker to get current price 
                if (positionList.Where(x => x.Symbol == ticker).Count() == 0)
                {
                    continue; // Sold a stock that has been liquidated and has no current price in the positions table
                }

                ExcelPosition position = positionList.Where(x => x.Symbol == ticker).First();

                decimal currentPrice = 0;
                currentPrice = (decimal)position.Price;

                // See if there was a later sell off and eliminate it
                buyAndSold = false;
                if (position.BuySell == "Sell")
                {
                    buyAndSold = true;
                }

                // profit/ loss   
                decimal profit = currentPrice - (decimal)position.BuyPrice;

                // Get the DOW level
                string temp = trade.DOW.ToString();
                int dowLevel = 0;
                if (temp._IsDecimal())
                {
                    dowLevel = Convert.ToInt32(temp);
                }

                PerformanceItem pi = new PerformanceItem()
                {
                    TradeDate = trade.TradeDate,
                    Ticker = ticker,
                    Quantity = (int)position.BuyQuantity,
                    TradePrice = (decimal)position.BuyPrice,
                    CurrentPrice = currentPrice,
                    Profit = profit,
                    TotalProfit = (decimal)position.BuyQuantity * profit,
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

        public List<PerformanceItem> GetLatestSellPerformance(List<ExcelPosition> positionList, DataTable tradesDataTable)
        {
            StockHistory stockHistory = new StockHistory();
            List<PerformanceItem> performanceList = new List<PerformanceItem>();
            bool soldAndBought = false;

            // Get last 25 sells
            IEnumerable<DataRow> sellTrades = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Sell" && x[(int)TC.TradeDate].ToString().Trim() != "" && x[(int)TC.QuantityHeld].ToString().Trim() != "0");
            sellTrades = sellTrades.OrderByDescending(x => x[(int)TC.TradeDate]).Take(25).OrderBy(x => x[(int)TC.TradeDate]);

            performanceList.Clear();
            foreach (DataRow trade in sellTrades)
            {
                string ticker = trade.ItemArray[(int)TC.Ticker].ToString();

                // Search in positions for ticker to get current price 
                if(positionList.Where(x => x.Symbol == ticker).Count() == 0)
                {
                    continue; // Sold a stock that has been liquidated and has no current price in the positions table
                }

                ExcelPosition position = positionList.Where(x => x.Symbol == ticker).First();
                
                decimal currentPrice = 0;
                currentPrice = (decimal)position.Price;

                // See if there was a later buyback and eliminate it
                soldAndBought = false;
                if (position.BuySell == "Buy")
                {
                    soldAndBought = true;
                }

                // profit/ loss   
                decimal profit = (decimal)position.SellPrice - currentPrice;

                PerformanceItem pi = new PerformanceItem()
                {
                    TradeDate = Convert.ToDateTime(trade.ItemArray[0].ToString()),
                    Ticker = ticker,
                    Quantity = (int)position.SellQuantity,
                    TradePrice = (decimal)position.SellPrice,
                    CurrentPrice = currentPrice,
                    Profit = profit,
                    TotalProfit = (decimal)position.SellQuantity * profit,
                    DowLevel = 0,
                    SoldAndBought = soldAndBought
                };

                performanceList.Add(pi);
            }

            return performanceList;
        }

        public async Task<List<PerformanceItem>> GetLiquidationPerformance(List<ExcelPosition> positionList, DataTable tradesDataTable)
        {
            StockHistory stockHistory = new StockHistory();
            List<PerformanceItem> performanceList = new List<PerformanceItem>();

            // Get liquidations for this last year
            IEnumerable<DataRow> tickerTrades = tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Sell" && x[(int)TC.TradeDate].ToString().Trim() != "" && Convert.ToDateTime(x[(int)TC.TradeDate]) > DateTime.Now.AddYears(-1) && x[(int)TC.QuantityHeld].ToString().Trim() == "0").Skip(5);
            tickerTrades = tickerTrades.OrderBy(x => x[(int)TC.TradeDate]).Take(25);

            performanceList.Clear();
            foreach (DataRow trade in tickerTrades)
            {
                // Get current price 
                string ticker = trade.ItemArray[(int)TC.Ticker].ToString();
                string temp = trade.ItemArray[(int)TC.QuantityTraded].ToString();
                int quantity = 0;

                var count = positionList.Where(x => x.Symbol == ticker).Count();

                if (count > 0)
                    continue;

                if (temp._IsInt())
                {
                    quantity = Convert.ToInt32(temp);
                }

                // Get current price for sold stock, some stock can get delisted
                decimal currentPrice = 0;
                try
                {
                    currentPrice = await stockHistory.GetTodaysPrice(ticker);

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Not Found"))
                        continue;
                    else
                        throw ex;
                }

                if (currentPrice == 0)
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
