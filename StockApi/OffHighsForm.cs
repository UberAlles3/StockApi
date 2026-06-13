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
        private List<ExcelPosition> _positionsList;

        public OffHighsForm(List<ExcelPosition> positionsList, DataTable tradesDataTable)
        {
            InitializeComponent();
            _positionsList = positionsList;
            _tickers = positionsList.Select(x => x.Symbol).ToList(); // Get high mtric stock symbols
            _tradesDataTable = tradesDataTable;
        }

        private async void OffHighs_Load(object sender, EventArgs e)
        {
            ExcelPosition position;

            // Read the trades table and find all tickers the have a metric of over 1.2
            YahooFinanceAPI yahooFinanceAPI = new YahooFinanceAPI();

            //string ticker = tickers[0];
            txtTickerList.Text = ""; // "Ticker      High    Current     Target" + Environment.NewLine;
            foreach (string ticker in _tickers)
            {
                //if (ticker != "AVGO")
                //    continue;
                //if (ticker == "AG")
                //    break;

                // Get latest position instance for the stock
                position = _positionsList.Where(x => x.Symbol == ticker).First();

                // Find last 40 price 
                List<StockQuote> quotes = await yahooFinanceAPI.GetQuotes(ticker, DateTime.Now.AddDays(-42), 42, "1d");

                // Get highest price got last 6 weeks
                decimal high = quotes.Max(x => x.Close);

                txtTickerList.Text += $"{(ticker + "    ").Substring(0, 5)}";
                if (position.Price < (double)high * .88D && position.Price < position.PastYearHigh *.8) // 20% drop
                {
                    txtTickerList.Text += $"     {(high).ToString("00.00").PadLeft(7, ' ')}     {(position.PastYearHigh).ToString("00.00").PadLeft(7, ' ')}    {position.Price.ToString("00.00").PadLeft(7, ' ')}    {(high * .88M).ToString(" 00.00").PadLeft(7, ' ')} ";

                    if(position.BuySell == "Buy") // If already bought in the last 30 days
                    {
                        txtTickerList.Text += $" Bought {position.BuyPrice}";
                    }
                    else
                    {
                        txtTickerList.Text += $" ****** BUY ******";
                    }
                }
                txtTickerList.Text += Environment.NewLine;
            }
        }

        private void OffHighsForm_Resize(object sender, EventArgs e)
        {
            txtTickerList.Height = this.Height - 80;
        }

        public static List<string> GetHighMetricTickers(List<ExcelPosition> positionsList)
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
