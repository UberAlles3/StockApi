using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockApi
{
    public class FpmAPI
    {
        static string _apiKey = "irXC95S3Hmlih4AtFtDkxj49w1OUpLZs";
        string requestUrl = $"https://financialmodelingprep.com/stable/quote?symbol=[ticker]&apikey={_apiKey}";
        bool tooManyRequests = false; // free plan is 250 request a day

        private readonly HttpClient client = new HttpClient();

        public async Task<MarketData> GetQuote(string ticker, bool showMessageBox)
        {
            //string symbol = "^DJI"; // Example stock symbol
            MarketData marketData = new MarketData();

            if (tooManyRequests == true)
                return marketData;

            requestUrl = requestUrl.Replace("[ticker]", ticker);

            try
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode(); // Throw an exception if not a success status code

                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response (example for a simple quote)
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    marketData.PreviousClose.NumericValue = root[0].GetProperty("previousClose").GetDecimal();
                    marketData.CurrentLevel.NumericValue = root[0].GetProperty("price").GetDecimal();
                    marketData.RetreivedDate = DateTime.Now;
                }
            }
            catch (HttpRequestException e)
            {
                if (e.Message.ToLower().Contains("too many requests"))
                {
                    tooManyRequests = true;
                }
                else
                {
                    if (showMessageBox)
                        MessageBox.Show($"FPM API Request Error: {ticker}   {e.Message}");
                }
            }
            catch (JsonException e)
            {
                if (showMessageBox)
                    MessageBox.Show($"JSON Deserialization Error: {e.Message}");
            }

            return marketData;
        }
    }
}
