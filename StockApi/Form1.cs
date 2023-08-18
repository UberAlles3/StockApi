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
        public Form1()
        {
            InitializeComponent();
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://yh-finance-complete.p.rapidapi.com/yhf?ticker=pypl"),
                Headers =
                {
                    { "X-RapidAPI-Key", "5b43e47e81msh209d3a319f60d02p1e399ajsne949d592c311" },
                    { "X-RapidAPI-Host", "yh-finance-complete.p.rapidapi.com" },
                },
            };

            await GetHttpResponse(client, request);
         }

        private static async Task GetHttpResponse(HttpClient client, HttpRequestMessage request)
        {
            using (var response = client.SendAsync(request))
            {
                var body = await response.Result.Content.ReadAsStringAsync();
                dynamic stock = JsonConvert.DeserializeObject(body);
                decimal beta = 0;
                bool success = decimal.TryParse(stock.summaryDetail.beta.ToString(), out beta);

                ///
                Debug.WriteLine(beta);
            }
        }
    }
}
