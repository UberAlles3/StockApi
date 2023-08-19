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
using Yahoo.Finance;

// 2dadcc0b -- API key

namespace StockApi
{
    public partial class Form1 : Form
    {
        private static StockData _StockData;

        public Form1()
        {
            InitializeComponent();
        }


        private async void btnGetOne_click(object sender, EventArgs e)
        {
            EquitySummaryData equitySummaryData = new EquitySummaryData();

            string url = "https://finance.yahoo.com/quote/intc";
            HttpClient cl = new HttpClient();
            HttpResponseMessage hrm = await cl.GetAsync(url);
            string web = await hrm.Content.ReadAsStringAsync();

            equitySummaryData.Beta =  System.Convert.ToSingle(GetDataByDataTestName(web, "BETA_5Y-value"));



            

            string jsonResponse = "";
            
            if(string.IsNullOrEmpty(txtStockTicker.Text))
            {
                MessageBox.Show("Enter a valid stock ticker.");
                return;
            }
            
            HttpRequestMessage request = CreateHttpRequest(txtStockTicker.Text);
            await GetHttpResponse(request, jsonResponse);
        }

        private static HttpRequestMessage CreateHttpRequest(string stockTicker)
        {
            return new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://yh-finance-complete.p.rapidapi.com/yhf?ticker=" + stockTicker),
                Headers =
                {
                    { "X-RapidAPI-Key", "5b43e47e81msh209d3a319f60d02p1e399ajsne949d592c311" },
                    { "X-RapidAPI-Host", "yh-finance-complete.p.rapidapi.com" },
                },
            };
        }

        private static async Task GetHttpResponse(HttpRequestMessage request, string jsonReponse)
        {
            var client = new HttpClient();

            using (var response = client.SendAsync(request))
            {
                string jsonResponse = await response.Result.Content.ReadAsStringAsync();
                ParseJsonResponse(jsonResponse);
            }
        }

        private static void ParseJsonResponse(string body)
        {
            dynamic stock = JsonConvert.DeserializeObject(body);

            try
            {
                string error = stock.error;
                if (error != null)
                {
                    MessageBox.Show("API Error:" + Environment.NewLine + error + Environment.NewLine + "Possibly an invalid ticker.");
                    return;
                }
            }
            catch
            {
            }

            decimal beta = 0;
            bool success = decimal.TryParse(stock.summaryDetail.beta.ToString(), out beta);

            if (success)
            {
                _StockData = new StockData();
                _StockData.Beta = beta;
            }


            Debug.WriteLine(beta);
        }

        private string GetDataByDataTestName(string web_data, string data_test_name)
        {
            int loc1 = 0;
            int loc2 = 0;

            loc1 = web_data.IndexOf("data-test=\"" + data_test_name + "\"");
            if (loc1 == -1)
            {
                throw new Exception("Unable to find data with data test name '" + web_data + "' inside web data.");
            }

            loc1 = web_data.IndexOf(">", loc1 + 1);
            loc2 = web_data.IndexOf("<", loc1 + 1);

            string middle = web_data.Substring(loc1 + 1, loc2 - loc1 - 1);
            return middle;
        }
    }
}
