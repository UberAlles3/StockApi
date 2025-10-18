using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockApi
{
    public class YahooFinance
    {
        public static string NotApplicable = "N/A";
        //private static readonly object syncObj = new object();
        private string _ticker;
        public static List<SearchTerm> SearchTerms = new List<SearchTerm>(); // singleton instance, load once

        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value.ToUpper(); }
        }

        public YahooFinance()
        {
            if (SearchTerms.Count == 0)
                SearchTerms = ConfigurationManager.GetSection("SearchTokens") as List<SearchTerm>;
        }

        public virtual Task<bool> GetStockData(string ticker)
        {
            return Task.FromResult(true);
        }

        protected async Task<string> GetHtmlForTicker(string url, string ticker)
        {
            Ticker = ticker;
            string formattedUrl = url.Replace("???", Ticker);
            string html = "";

            html = await GetHtml(formattedUrl);

            if (html.Length < 3400) // retry
            {
                Thread.Sleep(1000);
                html = await GetHtml(formattedUrl);
                Thread.Sleep(1000);
            }

            return html;
        }

        protected static async Task<string> GetHtml(string url)
        {
            string html;

            HttpClient cl = new HttpClient();
            string browserVer = "Mozilla/5.0 (compatible)";

            cl.DefaultRequestHeaders.UserAgent.ParseAdd($"{browserVer}");
            cl.DefaultRequestHeaders.Accept.ParseAdd("text/html");
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

        ////////////////////////////
        ///    Parsing methods
        ////////////////////////////
        protected static string GetDataByTagName(string html, string tagName, string defaultValue)
        {
            int loc1 = html.IndexOf("<" + tagName) + 1;
            int loc2 = html.IndexOf("</" + tagName, loc1 + 5); //</title
            string middle = "?????";

            if (loc1 == -1)
            {
                return defaultValue;
            }

            try
            {
                middle = html.Substring(loc1 + 1 + tagName.Length, loc2 - loc1 - 1);
            }
            catch (System.Exception x)
            {
                MessageBox.Show($"GetDataByTagName{x.Message}\n{tagName}\n{html}\n{loc1}\n{loc2}");
            }

            return middle;
        }

        public static string GetPartialHtmlFromHtmlBySearchTerm(string html, string searchText, int length)
        {
            int loc1 = 0;

            loc1 = html.IndexOf(searchText);
            if (loc1 == -1)
            {
                return "";
            }

            if (length > html.Length - 1)
                length = html.Length - 2;

            return html.Substring(loc1 + 1, length);
        }

        protected static List<string> GetNumbersFromHtml(string partial)
        {
            var numbers = new List<string>();
            string[] split = partial.Split(">");
            string num;
            foreach (string s in split)
            {
                if (s.Length > 1 && "-0123456789.".IndexOf(s.Substring(0, 1)) > -1 && "0123456789.,".IndexOf(s.Substring(1, 1)) > -1)
                {
                    num = s.Substring(0, s.IndexOf("<"));
                    numbers.Add(num);
                }
                if (numbers.Count > 5)
                    break;
            }

            return numbers;
        }

        protected static string GetValueFromHtmlBySearchTerm(string html, string searchText, string defaultValue, int tagPosition)
        {
            int loc1 = 0;
            int loc2 = 0;

            loc1 = html.IndexOf(searchText);
            if (loc1 == -1)
            {
                return defaultValue;
            }

            string htmlSnippet = html.Substring(loc1 + 1, 250);
            string[] parts = htmlSnippet.Split(">");

            if (parts.Length < 3)
            {
                return defaultValue;
            }

            loc2 = (parts[tagPosition] + "<").IndexOf("<");
            if(loc2 == 0)
            {
                return defaultValue;
            }

            string middle = parts[tagPosition].Substring(0, loc2);
            return middle;
        }
        public bool NotNumber(string value)
        {
            return value == YahooFinance.NotApplicable || value == "" || value == "--" || "-0123456789.,".IndexOf(value.Substring(0, 1)) < 0;
        }

        public static void RenewIPAddress()
        {
            ProcessStartInfo psiRelease = new ProcessStartInfo("ipconfig", "/release");
            psiRelease.CreateNoWindow = true;
            psiRelease.UseShellExecute = false;
            Process.Start(psiRelease)?.WaitForExit();

            ProcessStartInfo psiRenew = new ProcessStartInfo("ipconfig", "/renew");
            psiRenew.CreateNoWindow = true;
            psiRenew.UseShellExecute = false;
            Process.Start(psiRenew)?.WaitForExit();
        }
    }
}

