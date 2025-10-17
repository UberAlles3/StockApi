using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StockApi.Downloads
{
    class StockDownloads
    {
        public StockSummary stockSummary = new StockSummary();
        public StockHistory stockHistory = new StockHistory();
        public StockIncomeStatement stockIncomeStatement = new StockIncomeStatement();
        public StockStatistics stockStatistics = new StockStatistics();

        private string _ticker = "";

        public StockDownloads(string ticker)
        {
            _ticker = ticker;
        }

        public int DownloadAll()
        {
            return 0;
        }

        public async Task<bool> GetStockStatistics()
        {
            bool found = await stockStatistics.GetStatisticData(_ticker);

            return found;
        }










    }
}
