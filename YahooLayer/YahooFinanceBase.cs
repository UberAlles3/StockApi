using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace YahooLayer
{
    public class YahooFinanceBase
    {
        public static string NotApplicable = "N/A";
        //private static readonly object syncObj = new object();
        private static string _ticker;
        private static List<SearchTerm> searchTerms = null; // singleton instance, load once
        public static readonly ILog logger = LogManager.GetLogger(typeof(YahooFinanceBase));
        protected static Color _normalColor = Color.LightSteelBlue;

        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value.ToUpper(); }
        }

        public static List<SearchTerm> SearchTerms 
        {
            get
            {
                if (searchTerms == null)
                    searchTerms = new List<SearchTerm>();
                if (searchTerms.Count == 0)
                    searchTerms = ConfigurationManager.GetSection("SearchTokens") as List<SearchTerm>;
                return searchTerms;
            }

            set => searchTerms = value; 
        }

        public YahooFinanceBase()
        {
            // TODO takeout
            //SearchTerms = new List<SearchTerm>();

            //if (SearchTerms.Count == 0)
            //    SearchTerms = ConfigurationManager.GetSection("SearchTokens") as List<SearchTerm>;
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
            catch (System.Exception ex)
            {
                logger.Error($"{tagName}\n{html}\n{loc1}\n{loc2}\n{ex.Message}  {ex.StackTrace}", ex);
                return "Error. Check Log.";
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

            string middle = "";
            try
            {
                middle = parts[tagPosition].Substring(0, loc2);
            }
            catch (Exception ex)
            {
                logger.Error($"*******ERROR**********\r\n   searchText={searchText}\r\n   loc1={loc1}    \r\nloc2={loc2}\r\n   {ex.Message}\r\n   {ex.StackTrace}\r\n");
            }

            return middle;
        }

        protected bool ParseHtmlRowData(string html, string searchTerm, StringSafeType<decimal> property0, StringSafeType<decimal> property2, StringSafeType<decimal> property4)
        {
            string partial = "";
            List<string> numbers;

            searchTerm = YahooFinanceBase.SearchTerms.Find(x => x.Name == searchTerm).Term;
            partial = GetPartialHtmlFromHtmlBySearchTerm(html, searchTerm, 320);

            if (partial.Length < 100) // Some stocks like Vangaurd don't have cash flow, exit
            {
                numbers = null;
                return false; //=====>>>>>>>
            }

            numbers = GetNumbersFromHtml(partial);
            if (numbers.Count > 0)
                property0.StringValue = numbers[0].Trim();
            if (numbers.Count > 2)
                property2.StringValue = numbers[2].Trim();
            if (numbers.Count > 4)
                property4.StringValue = numbers[4].Trim();
            else if (numbers.Count > 3)
                property4.StringValue = numbers[3].Trim();

            return true;
        }

        public bool NotNumber(string value)
        {
            return value == YahooFinanceBase.NotApplicable || value == "" || value == "--" || "-0123456789.,".IndexOf(value.Substring(0, 1)) < 0;
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

        public static decimal SetYearOverYearTrend(StringSafeType<decimal> year4, StringSafeType<decimal> year2, StringSafeType<decimal> ttm, int adjustment)
        {
            CrunchThreeResult crt = new CrunchThreeResult();
            decimal val = 0;

            try
            {
                crt = CrunchThree((double)year4.NumericValue, (double)year2.NumericValue, (double)ttm.NumericValue);
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, adjustment); // less impact
                val = (decimal)crt.FinalMetric;
            }
            catch (Exception ex)
            {
                //Debug.WriteLine($"{_ticker} {crt.FinalMetric} {ex.Message}");
                logger.Error($"{ex.Message}\r\n  {ex.StackTrace}\r\n");
            }

            return Decimal.Round((decimal)crt.FinalMetric, 3);
        }

        public static CrunchThreeResult CrunchThree(double one, double two, double three)
        {
            CrunchThreeResult crt = new CrunchThreeResult();

            if (one + two + three == 0)
            {
                logger.Info($"{_ticker} had zeros in it.");
                crt.FinalMetric = 1;
                return crt;
            }

            // Find Abs(minimum)
            double minimum = Math.Min(one, Math.Min(two, three)); // example -2, 3, -1 = 4    2, 3, 6 = 4   - 11, 100, 100 = 22
            double maximum = Math.Max(one, Math.Max(two, three)); // example -2, 3, -1 = 4    2, 3, 6 = 4   - 11, 100, 100 = 22
            if (minimum < 0)
                minimum = Math.Abs(minimum) * 2;
            if (maximum < 0)
                maximum = Math.Abs(maximum) * 2;

            if (minimum * 6 < maximum)
                minimum += maximum;

            // Add to all 3 numbers
            one += minimum; two += minimum; three += minimum;

            // Get ratios between them
            crt.Ratio1 = (double)two / (double)one;
            crt.Ratio2 = (double)three / (double)two;
            crt.Ratio3 = (double)three / (double)one;

            // Get the Log() of the ratios and dvide by a factor of 8
            crt.Log1 = 1 + Math.Log(crt.Ratio1) / 3; // diff for earlier less important
            crt.Log2 = 1 + Math.Log(crt.Ratio2) / 2.2D;
            crt.Log3 = 1 + Math.Log(crt.Ratio3) / 2.2D;

            crt.FinalMetric = (crt.Log1 + crt.Log2 + crt.Log3) / 3;

            if (crt.FinalMetric > 1.07D)
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, -3); // Lessen metric weight
            if (crt.FinalMetric > 1.06D)
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, -1); // Lessen metric weight
            if (crt.FinalMetric < .93D)
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, -3); // Lessen metric weight
            if (crt.FinalMetric < .94D)
                crt.FinalMetric = AdjustMetric(crt.FinalMetric, -1); // Lessen metric weight

            return crt;
        }

        public static decimal AdjustMetric(decimal metric, decimal factor) // negative number less important, positive more important, range -5 to +5
        {
            decimal newMetric = metric;
            newMetric = (decimal)AdjustMetric((double)metric, (double)factor);

            return Math.Round(newMetric, 3);
        }
        public static double AdjustMetric(double metric, double factor) // negative number less important, positive more important, range -5 to +5
        {
            double newMetric = metric;
            double absFactor = Math.Abs(factor);

            if (factor == 0)
                return metric;

            newMetric = ((absFactor) + Math.Log(metric)) / (absFactor);

            return Math.Round(newMetric, 3);
        }

        public static double HardLimit(double metric, double min, double max) // negative number less important, positive more important, range -5 to +5
        {
            return (double)HardLimit((decimal)metric, (decimal)min, (decimal)max);
        }
        public static decimal HardLimit(decimal metric, decimal min, decimal max) // negative number less important, positive more important, range -5 to +5
        {
            decimal newMetric = metric;

            if (metric < min)
                newMetric = min;
            if (metric > max)
                newMetric = max;

            return Math.Round(newMetric, 3);
        }
        public static double SoftLimit(double metric, double min, double max) // negative number less important, positive more important, range -5 to +5
        {
            return (double)SoftLimit((decimal)metric, (decimal)min, (decimal)max);
        }
        public static decimal SoftLimit(decimal metric, decimal min, decimal max) // negative number less important, positive more important, range -5 to +5
        {
            decimal newMetric = metric;

            if (newMetric > max) // limit
                newMetric = (((newMetric + max) / 2) + max) / 2;
            if (newMetric < min) // limit
                newMetric = (((newMetric + min) / 2) + min) / 2;

            return Math.Round(newMetric, 3);
        }

        public class CrunchThreeResult
        {
            public double Ratio1;
            public double Ratio2;
            public double Ratio3;
            public double Log1;
            public double Log2;
            public double Log3;
            public double FinalMetric;
        }
    }
    public class SearchTerm
    {
        public string Name { get; set; }
        public string Term { get; set; }
    }
}

