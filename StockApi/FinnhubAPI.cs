using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockApi
{
    public class FinnhubAPI
    {
        private const string _apiKey = "d25pcqhr01qhge4do43gd25pcqhr01qhge4do440";
        private const string BaseUrl = "https://finnhub.io/api/v1/";
        //string _requestSecret = "\"X-Finnhub-Secret\": \"d25pcqhr01qhge4do450\"";
        private readonly HttpClient client = new HttpClient();

        public async Task<MarketData> GetQuote(string symbol, DateTime startDate, int numberOfdays)
        {
            int unixStartTimestamp = (int)startDate.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            int unixEndTimestamp = (int)startDate.AddDays(numberOfdays).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;


            // Construct the API URL
            string requestUrl = $"{BaseUrl}quote?symbol=ticker&token={_apiKey}";
            // costs money    var requestUrl = $"{BaseUrl}stock/candle?symbol={symbol}&resolution=D&from={unixStartTimestamp}&to={unixEndTimestamp}&token={_apiKey}";
            decimal currentPrice = 0;

            try
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode(); // Throw an exception if not a success status code

                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response (example for a simple quote)
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    currentPrice = root.GetProperty("c").GetDecimal();
                }
            }
            catch (HttpRequestException e)
            {
                 Console.WriteLine($"Request Error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON Deserialization Error: {e.Message}");
            }

            return null;
        }
    }
}
