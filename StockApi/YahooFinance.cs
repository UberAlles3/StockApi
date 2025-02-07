using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace StockApi
{
    public class YahooFinance
    {
        public static string NotApplicable = "N/A";
        //private static readonly object syncObj = new object();
        private string _ticker;
        protected static List<SearchTerm> SearchTerms = new List<SearchTerm>(); // singleton instance, load once

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

        protected static async Task<string> GetHtml(string url)
        {
            string html;

            HttpClient cl = new HttpClient();
            cl.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
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

        protected async Task<string> GetHtmlForTicker(string url, string ticker)
        {
            Ticker = ticker;
            string formattedUrl = url.Replace("???", Ticker);
            return await GetHtml(formattedUrl);
        }

        ////////////////////////////
        ///    Parsing methods
        ////////////////////////////
        protected static string GetDataByTagName(string html, string tagName, string defaultValue)
        {
            int loc1 = html.IndexOf("<" + tagName) + 1;
            int loc2 = html.IndexOf("</" + tagName, loc1 + 5); //</title
            if (loc1 == -1)
            {
                return defaultValue;
            }

            string middle = html.Substring(loc1 + 1 + tagName.Length, loc2 - loc1 - 1);
            return middle;
        }

        protected static string GetPartialHtmlFromHtmlBySearchTerm(string html, string searchText, int length)
        {
            int loc1 = 0;

            loc1 = html.IndexOf(searchText);
            if (loc1 == -1)
            {
                return "";
            }

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

            string middle = parts[tagPosition].Substring(0, loc2);
            return middle;
        }

        //protected static string GetValueFromHtml(string html, string data_test_name, string defaultValue)
        //{
        //    int loc1 = 0;
        //    int loc2 = 0;

        //    loc1 = html.IndexOf("data-test=\"" + data_test_name + "\"");
        //    if (loc1 == -1)
        //    {
        //        return defaultValue.ToString();
        //    }

        //    loc1 = html.IndexOf(">", loc1 + 1);
        //    loc2 = html.IndexOf("<", loc1 + 1);

        //    string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
        //    return middle;
        //}

        //protected static string GetDecimalValueFromHtml(string html, string data_test_name, string defaultValue)
        //{
        //    string temp = GetValueFromHtml(html, data_test_name, defaultValue);
        //    if (temp != YahooFinance.NotApplicable)
        //        return temp;
        //    else
        //        return defaultValue;
        //}

        //protected static string GetDataByClassName(string html, string class_name, string defaultValue)
        //{
        //    int loc1 = html.IndexOf("class=\"" + class_name + "\"");
        //    if (loc1 == -1)
        //    {
        //        return defaultValue;
        //    }
        //    loc1 = html.IndexOf(">", loc1 + 1);
        //    int loc2 = html.IndexOf("<", loc1 + 1);
        //    string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
        //    return middle;
        //}

        //protected static string GetValueFromHtmlBySearchText(string html, string searchText, string defaultValue)
        //{
        //    int loc1 = 0;
        //    int loc2 = 0;

        //    loc1 = html.IndexOf(searchText);
        //    if (loc1 == -1)
        //    {
        //        return defaultValue;
        //    }

        //    loc1 = html.IndexOf(">", loc1 - 4);
        //    loc2 = html.IndexOf("<", loc1 + 1);

        //    string middle = html.Substring(loc1 + 1, loc2 - loc1 - 1);
        //    return middle;
        //}
    }
}

