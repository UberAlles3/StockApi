using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Globalization;

namespace StockApi
{
    public class YahooFinance
    {
        public static string NotApplicable = "N/A";
        private static readonly object syncObj = new object();

        private string _ticker;
        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value.ToUpper(); }
        }

        protected static async Task<string> GetHtml(string url)
        {
            string html;

            HttpClient cl = new HttpClient();
            cl.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36");
            HttpResponseMessage hrm = await cl.GetAsync(url); 
            html = await hrm.Content.ReadAsStringAsync();
            return html;

            //lock (syncObj)
            //{
            //    WebView wb = new WebView();
            //    //wb.Show();
            //    wb.Navigate(url);
            //    while (wb.NavigationComplete == false)
            //    {
            //        Application.DoEvents();
            //    }
            //    html = wb.Html;
            //    wb.Close();
            //    wb.Dispose();
            //}
            //return html;
        }

    }
 
}

