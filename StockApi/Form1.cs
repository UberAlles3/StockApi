using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Diagnostics;

// 2dadcc0b -- API key

namespace StockApi
{
    public partial class Form1 : Form
    {
        private static YahooFinance.StockData _stockData = new YahooFinance.StockData();
        
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(100, 0, 0, 0);
            lblTicker.Text = "";
            txtTickerList.Text = "AB" + Environment.NewLine + "ACB" + Environment.NewLine + "AG" + Environment.NewLine + "AM" + Environment.NewLine;
        }

        private async void btnGetOne_click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStockTicker.Text))
            {
                MessageBox.Show("Enter a valid stock ticker.");
                return;
            }

            PreWebCall(); // Sets the form display while the request is executing

            // Execute the request to get html from Yahoo Finance
            string html = await YahooFinance.GetHtmlForTicker(txtStockTicker.Text);
            // Extract the individual data values from the html
            _stockData.Ticker = txtStockTicker.Text;
            YahooFinance.HtmlParser.ExtractDataFromHtml(_stockData, html);

            PostWebCall(_stockData); // displays the data returned
        }

        private async void btnGetAll_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            
            List<string> stockList = new List<string>();
            stockList = txtTickerList.Text.Split(Environment.NewLine).ToList();
            stockList = stockList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList(); // remove blacks

            txtTickerList.Text = ".";
            foreach (string ticker in stockList)
            {
                _stockData.Ticker = ticker.Substring(0, (ticker + ",").IndexOf(",")).ToUpper();

                string html = await YahooFinance.GetHtmlForTicker(_stockData.Ticker);
                txtTickerList.Text += ".";

                // Extract the individual data values from the html
                YahooFinance.HtmlParser.ExtractDataFromHtml(_stockData, html);

                builder.Append($"{_stockData.Ticker}, {_stockData.Beta}, {_stockData.EarningsPerShare}, {_stockData.OneYearTargetPrice}, {_stockData.FairValue}, {_stockData.EstimatedReturn}{Environment.NewLine}");
            }
            txtTickerList.Text = builder.ToString();
        }

        private void PreWebCall()
        {
            btnGetOne.Enabled = false;
            lblTicker.Text = txtStockTicker.Text.ToUpper();
            lblBeta.Text = "...";
            lblEPS.Text = "...";
        }
        private void PostWebCall(YahooFinance.StockData stockData)
        {
            btnGetOne.Enabled = true;
            lblBeta.Text = stockData.Beta.ToString();
            lblEPS.Text = stockData.EarningsPerShare.ToString();
        }
    }
}
