using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Diagnostics;

// 2dadcc0b -- API key

namespace StockApi
{
    public partial class Form1 : Form
    {
        private static string _url = "https://finance.yahoo.com/quote/";
        private static StockData _stockData = new StockData();
        
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.BackColor = Color.FromArgb(100, 0, 0, 0);
            lblTicker.Text = "";
            txtTickerList.Text = "INTC" + Environment.NewLine + "DHC" + Environment.NewLine + "VZ" + Environment.NewLine;
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
            string web = await GetHtmlFromYahooFinance(txtStockTicker.Text);

            // Extract the individual data values from the html
            YahooFinanceHtmlParser.ExtractDataFromHtml(_stockData, web);

            PostWebCall(_stockData); // displays the data returned
        }

        private async Task<string> GetHtmlFromYahooFinance(string ticker)
        {
            HttpClient cl = new HttpClient();
            HttpResponseMessage hrm = await cl.GetAsync(_url + ticker);
            string web = await hrm.Content.ReadAsStringAsync();
            return web;
        }

        private void PreWebCall()
        {
            btnGetOne.Enabled = false;
            lblTicker.Text = txtStockTicker.Text.ToUpper();
            lblBeta.Text = "...";
            lblEPS.Text = "...";
        }
        private void PostWebCall(StockData stockData)
        {
            btnGetOne.Enabled = true;
            lblBeta.Text = stockData.Beta.ToString();
            lblEPS.Text = stockData.EarningsPerShare.ToString();
        }

        private void btnGetAll_Click(object sender, EventArgs e)
        {

        }
    }
}
