using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace StockApi
{
    public class FinnhubAPI
    {
        string _apiKey = "d25pcqhr01qhge4do43gd25pcqhr01qhge4do440";
        string _requestSecret = "\"X-Finnhub-Secret\": \"d25pcqhr01qhge4do450\"";
        private readonly HttpClient client = new HttpClient();

        public async void Test()
        {
            string symbol = "AAPL"; // Example stock symbol

            // Construct the API URL
            string requestUrl = $"https://finnhub.io/api/v1/quote?symbol=DJIA&token={_apiKey}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode(); // Throw an exception if not a success status code

                string responseBody = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response (example for a simple quote)
                using (JsonDocument doc = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = doc.RootElement;
                    decimal currentPrice = root.GetProperty("c").GetDecimal();
                    Console.WriteLine($"Current price of {symbol}: {currentPrice}");
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
        }
    }
}
