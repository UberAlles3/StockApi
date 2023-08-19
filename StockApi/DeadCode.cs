using System;
using System.Collections.Generic;
using System.Text;

namespace StockApi
{
    class DeadCode
    {
        //string jsonResponse = "";

        //HttpRequestMessage request = CreateHttpRequest(txtStockTicker.Text);
        //await GetHttpResponse(request, jsonResponse);


        //private static HttpRequestMessage CreateHttpRequest(string stockTicker)
        //{
        //    return new HttpRequestMessage
        //    {
        //        Method = HttpMethod.Get,
        //        RequestUri = new Uri("https://yh-finance-complete.p.rapidapi.com/yhf?ticker=" + stockTicker),
        //        Headers =
        //        {
        //            { "X-RapidAPI-Key", "5b43e47e81msh209d3a319f60d02p1e399ajsne949d592c311" },
        //            { "X-RapidAPI-Host", "yh-finance-complete.p.rapidapi.com" },
        //        },
        //    };
        //}

        //private static async Task GetHttpResponse(HttpRequestMessage request, string jsonReponse)
        //{
        //    var client = new HttpClient();

        //    using (var response = client.SendAsync(request))
        //    {
        //        string jsonResponse = await response.Result.Content.ReadAsStringAsync();
        //        ParseJsonResponse(jsonResponse);
        //    }
        //}

        //private static void ParseJsonResponse(string body)
        //{
        //    dynamic stock = JsonConvert.DeserializeObject(body);

        //    try
        //    {
        //        string error = stock.error;
        //        if (error != null)
        //        {
        //            MessageBox.Show("API Error:" + Environment.NewLine + error + Environment.NewLine + "Possibly an invalid ticker.");
        //            return;
        //        }
        //    }
        //    catch
        //    {
        //    }

        //    decimal beta = 0;
        //    bool success = decimal.TryParse(stock.summaryDetail.beta.ToString(), out beta);

        //    if (success)
        //    {
        //        _StockData = new StockData();
        //        _StockData.Beta = beta;
        //    }


        //    Debug.WriteLine(beta);
        //}

    }
}
