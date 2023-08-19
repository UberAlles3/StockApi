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
        private static string url = "https://finance.yahoo.com/quote/";

        public Form1()
        {
            InitializeComponent();
        }


        private async void btnGetOne_click(object sender, EventArgs e)
        {
            EquitySummaryData equitySummaryData = new EquitySummaryData();

            if (string.IsNullOrEmpty(txtStockTicker.Text))
            {
                MessageBox.Show("Enter a valid stock ticker.");
                return;
            }

            lblBeta.Text = "...";

            HttpClient cl = new HttpClient();
            HttpResponseMessage hrm = await cl.GetAsync(url + txtStockTicker.Text);
            string web = await hrm.Content.ReadAsStringAsync();

            equitySummaryData.Beta =  System.Convert.ToSingle(GetDataByDataTestName(web, "BETA_5Y-value"));
            lblBeta.Text = equitySummaryData.Beta.ToString();
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
