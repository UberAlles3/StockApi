using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using PC = StockApi.ExcelManager.PositionColumns;
using TC = StockApi.ExcelManager.TradeColumns;
using System.Threading.Tasks;

namespace StockApi
{
    public partial class OffHighs : Form
    {
        private DataTable _positionsDataTable;
        private DataTable _tradesDataTable;

        public OffHighs(DataTable positionsDataTable, DataTable tradesDataTable)
        {
            InitializeComponent();
            _positionsDataTable = positionsDataTable;
            _tradesDataTable = tradesDataTable;
        }

        private async void OffHighs_Load(object sender, EventArgs e)
        {
            // Read the trades table and find all tickers the have a metric of over 1.2
            List<string> tickers = GetTickers();
            YahooFinanceAPI yahooFinanceAPI = new YahooFinanceAPI();

            // Get all buys for the last week
            IEnumerable<DataRow> tickerTrades = _tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Buy" && x[(int)TC.DowLevel].ToString().Trim() != "").Skip(700);
            tickerTrades = tickerTrades.OrderByDescending(x => x[(int)TC.TradeDate]).Take(20).OrderBy(x => x[(int)TC.TradeDate]);

            //string ticker = tickers[0];
            txtTickerList.Text = "Ticker      High    Current     Target" + Environment.NewLine;
            foreach (string ticker in tickers)
            {
                // Find last five price by week
                List<StockQuote> quotes = await yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddDays(-40), 40, "5d");
                // Get highest price got last 5 weeks
                decimal high = quotes.Max(x => x.Close);

                if (quotes.Last().Close < high * .94M)
                {
                    txtTickerList.Text += $"{(ticker + "   ").Substring(0, 5)}    {(high).ToString("00.00").PadLeft(7, ' ')}    {quotes.Last().Close.ToString("00.00").PadLeft(7, ' ')}    {(high * .94M).ToString(" 00.00").PadLeft(7, ' ')} ";

                    DataRow trade = tickerTrades.Where(x => x[(int)TC.Ticker].ToString() == ticker).LastOrDefault();

                    if(trade != null && Convert.ToDateTime(trade.ItemArray[(int)TC.TradeDate]) > DateTime.Now.AddDays(-10)) // If already bought in the last 10 days
                    {
                        txtTickerList.Text += $" Already bought at {trade.ItemArray[(int)TC.TradePrice]}";
                    }

                    txtTickerList.Text += Environment.NewLine;
                    //MessageBox.Show(ticker);
                }
            }
        }

        List<string> GetTickers()
        {
            List<string> tickers = new List<string>();

            IEnumerable<DataRow> tickersDR = _positionsDataTable.AsEnumerable().Where(x => x[(int)PC.Metric].ToString().Contains("1.2") || x[(int)PC.Metric].ToString().Contains("1.3"));
            tickersDR = tickersDR.OrderBy(x => x[(int)PC.Ticker]);

            foreach (DataRow trade in tickersDR)
            {
                // Fill tickers list
                tickers.Add(trade.ItemArray[(int)PC.Ticker].ToString());
            }

            return tickers;
        }

    }
}
