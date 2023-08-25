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

namespace StockApi
{
    public class YahooFinance // General purpose functions
    {
        public static string NotApplicable = "N/A";

        private string _ticker;

        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value.ToUpper(); }
        }

        public enum FairValueEnum
        {
            Overvalued,
            FairValue,
            Undervalued
        }

        protected static async Task<string> GetHtml(string url)
        {
            HttpClient cl = new HttpClient();
            HttpResponseMessage hrm = await cl.GetAsync(url);
            string html = await hrm.Content.ReadAsStringAsync();
            return html;
        }

    }
 
}

