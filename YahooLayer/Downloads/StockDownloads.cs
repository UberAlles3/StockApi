using SqlLayer;
using SqlLayer.SQL_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YahooLayer
{
    public class StockDownloads
    {
        public StockSummary stockSummary = new StockSummary();
        public StockHistory stockHistory = new StockHistory();
        public StockIncomeStatement stockIncomeStatement = new StockIncomeStatement();
        public StockStatistics stockStatistics = new StockStatistics();
        public StockCashFlow stockCashFlow = new StockCashFlow();

        private string _ticker = "";

        public StockDownloads(string ticker)
        {
            _ticker = ticker;
        }

        public async Task<bool> GetAllStockData()
        {
            bool found = true;

            SqlCrudOperations _finacialStatement = new SqlCrudOperations();

            found = await GetSummary();
            if(found)
            {
                stockSummary.sqlTicker = _finacialStatement.GetTicker(_ticker);

                stockIncomeStatement = new StockIncomeStatement(); // initializes all properties
                stockStatistics = new StockStatistics(); // initializes all properties
                stockCashFlow = new StockCashFlow(); // initializes all properties

                if (stockSummary.sqlTicker.IsPreRevenueCompany == false && stockSummary.sqlTicker.IsFund == false) // This is a normal stock
                {
                    await GetStatistics();
                    await GetIncomeStatement();
                    await GetCashFlow();
                }
                else // either a fund or pre-revenue
                {
                    if (stockSummary.sqlTicker.IsPreRevenueCompany == true) // Pre-revenue, get only statistics
                    {
                        await GetCashFlow();
                        await GetIncomeStatement();
                        await GetStatistics();
                    }
                }

                await GetHistory();
            }

            return found;
        }

        public async Task<bool> GetSummary()
        {
            stockSummary = new StockSummary();
            bool found = await stockSummary.GetStockData(_ticker);
            return found;
        }

        public async Task<bool> GetStatistics()
        {
            stockStatistics = new StockStatistics(); // initializes all properties
            bool found = await stockStatistics.GetStockData(_ticker);

            return found;
        }

        public async Task<bool> GetIncomeStatement()
        {
            stockIncomeStatement = new StockIncomeStatement(); // initializes all properties
            bool found = await stockIncomeStatement.GetStockData(_ticker);

            return found;
        }

        public async Task<bool> GetHistory()
        {
            stockHistory = new StockHistory(); // initializes all properties

            // get 3 year ago price
            stockHistory.HistoricDisplayList = await stockHistory.GetPriceHistoryFor3Year(_ticker, stockSummary);

            if (stockHistory.HistoricDisplayList.Count > 0)
                stockHistory.HistoricData3YearsAgo = stockHistory.HistoricDisplayList.Last();
            else
                stockHistory.HistoricData3YearsAgo = new StockHistory.HistoricPriceData() { Ticker = stockSummary.Ticker, Price = stockSummary.PriceString.NumericValue };

            stockHistory.SetTrends();

            return true;
        }

        public async Task<bool> GetCashFlow()
        {
            stockCashFlow = new StockCashFlow(); // initializes all properties
            bool found = await stockCashFlow.GetStockData(_ticker);

            return found;
        }
    }
}
