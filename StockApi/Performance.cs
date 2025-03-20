using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Drake.Extensions;

namespace StockApi
{
    public class Performance
    {

        public Performance()
        {

        }

        public void GetLatestBuyPerformance(DataTable _positionsDataTable, DataTable _tradesDataTable)
        {
            int DateColumn = 0;

            // Get last 20 buys
            //var tickerTrades = _tradesDataTable.AsEnumerable().Where(x => x[4].ToString().ToLower() == txtStockTicker.Text.ToLower());

            var positions = _positionsDataTable.AsEnumerable().Where(x => x[1].ToString().Trim() != "0" && x[1].ToString().Trim() !="");
            
            var tickerTrades = _tradesDataTable.AsEnumerable().Where(x => x[2].ToString() == "Buy" && x[1].ToString().Trim() != "").Skip(700);
            tickerTrades = tickerTrades.OrderByDescending(x => x[DateColumn]).Take(20);




            foreach(DataRow dr in tickerTrades)
            {
                // Search in positions for ticker to get current price 
                string ticker = dr.ItemArray[4].ToString();
                string temp = positions.Where(x => x[0].ToString() == ticker).First().ItemArray[2].ToString();
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

            }



        }
    }
}
