using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace StockApi
{
    class YahooFinanceAPI
    {
        private HttpClient _httpClient;
        private readonly string _baseUri = "https://query2.finance.yahoo.com/v8/finance/chart/[ticker]";
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public async Task<List<StockQuote>> GetQuotes(string ticker, DateTime startDate, int numberOfdays, string interval)
        {
            // Convert date to UNIX seconds from 1970
            int unixStartTimestamp = (int)startDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            int unixEndTimestamp = (int)startDate.AddDays(numberOfdays).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            List<StockQuote> listQuotes = new List<StockQuote>();

            QuoteRoot quotes = await GetJsonAsync(ticker, unixStartTimestamp.ToString(), unixEndTimestamp.ToString(), interval);
            //QuoteRoot quotes = await GetJsonAsync("intc", unixStartTimestamp.ToString(), unixEndTimestamp.ToString());
            //quotes = await GetJsonAsync("vgslx", unixStartTimestamp.ToString(), unixEndTimestamp.ToString());

            foreach (string close in quotes.chart.result[0].indicators.quote[0].close)
            {
                listQuotes.Add(new StockQuote() { Ticker = ticker, Close = Convert.ToDecimal(close) });
            }

            int i = 0;
            foreach (int timestamp in quotes.chart.result[0].timestamp)
            {
                DateTime quoteDate = unixEpoch.AddSeconds(Convert.ToInt32(timestamp));
                
                if(quoteDate.Date == DateTime.Now.Date)
                {
                    listQuotes[i].Price = Convert.ToDecimal(quotes.chart.result[0].meta.regularMarketPrice.ToString("0.000"));
                }

                //if (listQuotes[i].Price == 0)
                //{
                //    listQuotes[i].Price = 999;
                //}

                listQuotes[i++].QuoteDate = quoteDate.Date;
            }

            i = 0;
            foreach (string volume in quotes.chart.result[0].indicators.quote[0].volume)
            {
                string temp = volume;

                if (temp == null)
                    temp = "0";

                if (temp.Length > 9)
                    temp = volume.Substring(0, volume.Length - 3);
                
                listQuotes[i++].Volume = Convert.ToInt32(temp);
            }

            return listQuotes;
        }

        public async Task<QuoteRoot> GetJsonAsync(string ticker, string period1, string period2, string interval)
        {
            CancellationToken token = new CancellationToken();
            Uri requrestUri;
            // int unixTimestamp = (int)DateTime.UtcNow.AddDays(-7).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            using (_httpClient = new HttpClient())
            {
                _httpClient.BaseAddress = new Uri(_baseUri);

                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                requrestUri = CreateUri(ticker, period1, period2, interval);
                string json = "";

                // Call the finance API
                using (var response = await _httpClient.GetAsync(requrestUri, token))
                {
                    using (var content = response.Content)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            json = await content.ReadAsStringAsync();
                            QuoteRoot qr = null;
                            
                            try
                            {
                                qr = JsonConvert.DeserializeObject<QuoteRoot>(json);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"JsonConvert.DeserializeObject {json} {ex.Message}");
                            }
                            
                            return qr;
                        }
                        throw new GeneralExceptions().HandleJsonRequestExceptions(ticker, (int)response.StatusCode, await response.Content.ReadAsStringAsync(), requrestUri.ToString());
                    }
                }
            }
        }
        private Uri CreateUri(string ticker, string period1, string period2, string interval)
        {
            // 'https://query2.finance.yahoo.com/v8/finance/chart/[ticker]?symbol=[ticker]&period1=1744492585&period2=1745097662&interval=1d' 
            UriBuilder builder = new UriBuilder(_baseUri.Replace("[ticker]", ticker))
            {
                Query = $"symbol={ticker}&" +
                        $"period1={period1}&" +
                        $"period2={period2}&" +
                        $"interval={interval}"
            };
            return builder.Uri;
        }
    }

    public class GeneralExceptions
    {
        public Exception HandleJsonRequestExceptions(string ticker, int code, string content, string Uri)
        {
            string exceptionMessage = $"GetJsonAsync({ticker}, string period1, string period2, {content}, {Uri})";

            return new Exception(exceptionMessage);
        }

        private class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }
        private class ForbiddenException : Exception
        {
            public ForbiddenException(string message) : base(message) { }
        }
        private class BadRequest : Exception
        {
            public BadRequest(string message) : base(message) { }
        }
    }


    /////////////////////////////////////////////////
    ///            Stock Quote Class 
    /////////////////////////////////////////////////
    public class StockQuote
    {
        public string Ticker { get; set; }
        public DateTime QuoteDate { get; set; }
        //public string Ticker { get; set; } = "";
        public decimal Close { get; set; }
        public decimal Price { get; set; } // Only valued for current date
        public long Volume { get; set; }
    }

    /////////////////////////////////////////////////
    ///     Json to C# Deserialization Classes 
    /////////////////////////////////////////////////
    public class Adjclose
    {
        public List<string> adjclose { get; set; }
    }

    public class JJJChart
    {
        public List<JJJResult> result { get; set; }
        public object error { get; set; }
    }

    public class CurrentTradingPeriod
    {
        public JJJPre pre { get; set; }
        public JJJRegular regular { get; set; }
        public JJJPost post { get; set; }
    }

    public class JJJIndicators
    {
        public List<JJJQuote> quote { get; set; }
        public List<Adjclose> adjclose { get; set; }
    }

    public class JJJMeta
    {
        public string currency { get; set; }
        public string symbol { get; set; }
        public string exchangeName { get; set; }
        public string fullExchangeName { get; set; }
        public string instrumentType { get; set; }
        public int firstTradeDate { get; set; }
        public int regularMarketTime { get; set; }
        public bool hasPrePostMarketData { get; set; }
        public int gmtoffset { get; set; }
        public string timezone { get; set; }
        public string exchangeTimezoneName { get; set; }
        public double regularMarketPrice { get; set; }
        public double fiftyTwoWeekHigh { get; set; }
        public double fiftyTwoWeekLow { get; set; }
        public double regularMarketDayHigh { get; set; }
        public double regularMarketDayLow { get; set; }
        public long regularMarketVolume { get; set; }
        public string longName { get; set; }
        public string shortName { get; set; }
        public double chartPreviousClose { get; set; }
        public int priceHint { get; set; }
        public CurrentTradingPeriod currentTradingPeriod { get; set; }
        public string dataGranularity { get; set; }
        public string range { get; set; }
        public List<string> validRanges { get; set; }
    }

    public class JJJPost
    {
        public string timezone { get; set; }
        public int end { get; set; }
        public int start { get; set; }
        public int gmtoffset { get; set; }
    }

    public class JJJPre
    {
        public string timezone { get; set; }
        public int end { get; set; }
        public int start { get; set; }
        public int gmtoffset { get; set; }
    }

    public class JJJQuote
    {
        public List<string> open { get; set; }
        public List<string> volume { get; set; }
        public List<string> high { get; set; }
        public List<string> low { get; set; }
        public List<string> close { get; set; }
    }

    public class JJJRegular
    {
        public string timezone { get; set; }
        public int end { get; set; }
        public int start { get; set; }
        public int gmtoffset { get; set; }
    }

    public class JJJResult
    {
        public JJJMeta meta { get; set; }
        public List<int> timestamp { get; set; }
        public JJJIndicators indicators { get; set; }
    }

    public class QuoteRoot
    {
        public JJJChart chart { get; set; }
    }
}
