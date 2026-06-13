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
using YahooLayer;
using System.IO;
using ExcelDataReader;

namespace StockApi
{
    public partial class OffHighsForm : Form
    {
        private List<string> _tickers;
        private DataTable _tradesDataTable;

        public OffHighsForm(List<ExcelPositions> positionsList, DataTable tradesDataTable)
        {
            InitializeComponent();
            _tickers = positionsList.Select(x => x.Symbol).ToList();
            _tradesDataTable = tradesDataTable;
        }

        private async void OffHighs_Load(object sender, EventArgs e)
        {
            // Read the trades table and find all tickers the have a metric of over 1.2
            //List<string> tickers = GetHighMetricTickers();
            YahooFinanceAPI yahooFinanceAPI = new YahooFinanceAPI();

            // Get all buys for the last week
            IEnumerable<DataRow> tickerTrades = _tradesDataTable.AsEnumerable().Where(x => x[(int)TC.BuySell].ToString() == "Buy" && x[(int)TC.DowLevel].ToString().Trim() != "").Skip(700);
            tickerTrades = tickerTrades.OrderByDescending(x => x[(int)TC.TradeDate]).Take(70).OrderBy(x => x[(int)TC.TradeDate]);
            tickerTrades = tickerTrades.Where(x => Convert.ToDateTime(x[(int)TC.TradeDate].ToString()) > DateTime.Now.AddDays(-30));

            //string ticker = tickers[0];
            txtTickerList.Text = ""; // "Ticker      High    Current     Target" + Environment.NewLine;
            foreach (string ticker in _tickers)
            {
                //if (ticker != "AVGO")
                //    continue;

                // Find last 40 price 
                List<StockQuote> quotes = await yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddDays(-42), 42, "1d");
                // Find last five price by day
                List<StockQuote> current = await yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddDays(-3), 4, "1d");


                // Get highest price got last 6 weeks
                decimal high = quotes.Max(x => x.Close);

                txtTickerList.Text += $"{(ticker + "    ").Substring(0, 5)}";
                if (current.Last().Close < high * .88M) // 12% drop
                {
                    txtTickerList.Text += $"    {(high).ToString("00.00").PadLeft(7, ' ')}    {current.Last().Close.ToString("00.00").PadLeft(7, ' ')}    {(high * .88M).ToString(" 00.00").PadLeft(7, ' ')} ";

                    DataRow trade = tickerTrades.Where(x => x[(int)TC.Ticker].ToString() == ticker).LastOrDefault();

                    if(trade != null && Convert.ToDateTime(trade.ItemArray[(int)TC.TradeDate]) > DateTime.Now.AddDays(-32)) // If already bought in the last 30 days
                    {
                        txtTickerList.Text += $" Already bought at {trade.ItemArray[(int)TC.TradePrice]}";
                    }
                }
                txtTickerList.Text += Environment.NewLine;
            }
        }

        private void OffHighsForm_Resize(object sender, EventArgs e)
        {
            txtTickerList.Height = this.Height - 80;
        }

        public static List<string> GetHighMetricTickers(List<ExcelPositions> positionsList)
        {
            List<string> tickers = positionsList.Where(x => x.TotalMetric > 1.2).Select(x => x.Symbol).ToList();

            return tickers;
        }

        public static List<string> GetWatchListTickers(DataTable positionsDataTable)
        {
            List<string> tickers = new List<string>();

            IEnumerable<DataRow> tickersDR = positionsDataTable.AsEnumerable().Where(x => x[(int)PC.QuantityHeld].ToString() == "0" && (x[(int)PC.Metric].ToString().Contains("1.1") || x[(int)PC.Metric].ToString().Contains("1.2") || x[(int)PC.Metric].ToString().Contains("1.3")));
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
