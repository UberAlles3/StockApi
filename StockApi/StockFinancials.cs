using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace StockApi 
{
    public class StockFinancials : YahooFinance
    {
        private static readonly string _statisticsUrl = "https://finance.yahoo.com/quote/???/key-statistics/";
        private static readonly string _financialsUrl = "https://finance.yahoo.com/quote/???/financials/";

        public Color ShortInterestColor = Color.LightSteelBlue;

        private string shortInterestString = NotApplicable;
        private float shortInterest = 0;

        public string ShortInterestString
        {
            get => shortInterestString;
            set
            {
                shortInterestString = value;
                if (ShortInterestString == YahooFinance.NotApplicable || ShortInterestString == "" || ShortInterestString == "--" || "-0123456789.".IndexOf(value.Substring(0, 1)) < 0)
                    ShortInterest = 0;
                else
                    ShortInterest = Convert.ToSingle(ShortInterestString);
            }
        }

        public float ShortInterest
        {
            get => shortInterest;
            set
            {
                shortInterest = value;
                if (shortInterest > 8)
                    ShortInterestColor = Color.Red;
                else if (ShortInterest < 2)
                    ShortInterestColor = Color.Lime;
                else
                    ShortInterestColor = Color.LightSteelBlue;
            }
        }

        ////////////////////////////////////////////
        ///                Methods
        ////////////////////////////////////////////
        public async Task<string> GetHtmlForTicker(string url, string ticker)
        {
            Ticker = ticker;
            string formattedUrl = url.Replace("???", Ticker);
            return await GetHtml(formattedUrl);
        }

        public async Task<bool> GetShortInterest(string ticker)
        {
            Ticker = ticker;
            List<SearchTerm> searchTerms = ConfigurationManager.GetSection("SearchTokens") as List<SearchTerm>;

            string html = await GetHtmlForTicker(_statisticsUrl, Ticker);

            // Short Interest
            string searchTerm = searchTerms.Find(x => x.Name == "Short Interest").Term;
            shortInterestString = GetValueFromHtmlBySearchTerm(html, searchTerm, YahooFinance.NotApplicable, 4);
            if (shortInterestString != YahooFinance.NotApplicable && shortInterestString.IndexOf("%") > 0)
                ShortInterestString = shortInterestString.Substring(0, shortInterestString.IndexOf("%"));
            else
                ShortInterestString = YahooFinance.NotApplicable;

            return true;
        }




    }
}
