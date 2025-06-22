using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using PC = StockApi.ExcelManager.PositionColumns;

namespace StockApi
{
    public partial class OffHighs : Form
    {
        private DataTable _positionsDataTable;

        public OffHighs(DataTable positionsDataTable)
        {
            InitializeComponent();
            _positionsDataTable = positionsDataTable;
        }

        private void OffHighs_Load(object sender, EventArgs e)
        {
            // Read the trades table and find all tickers the have a metric of over 1.2
            List<string> tickers = GetTickers();

            

        }

        List<string> GetTickers()
        {
            List<string> tickers = new List<string>();

            IEnumerable<DataRow> tickersDR = _positionsDataTable.AsEnumerable().Where(x => x[(int)PC.Metric].ToString().Contains("1.3") || x[(int)PC.Metric].ToString().Contains("1.3"));
            tickersDR = tickersDR.OrderBy(x => x[(int)PC.Ticker]);

            foreach (DataRow trade in tickersDR)
            {
                // Search in positions for ticker to get current price 
                tickers.Add(trade.ItemArray[(int)PC.Ticker].ToString());
            }


            return tickers;
        }

    }
}
