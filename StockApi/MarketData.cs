using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YahooLayer;

namespace StockApi
{
    public class MarketData 
    {
        public string Ticker { get; set; }
        public DateTime RetreivedDate { get; set; }
        //public string Ticker { get; set; } = "";
        public StringSafeType<decimal> PreviousClose { get; set; } = new StringSafeType<decimal>("");
        public StringSafeType<decimal> CurrentLevel { get; set; } = new StringSafeType<decimal>("");

        public int Change
        {
            get => Convert.ToInt32(CurrentLevel.NumericValue - PreviousClose.NumericValue);
        }
        public decimal PercentageChange
        {
            get
            {
                decimal change = 0;
                
                if(PreviousClose.NumericValue > 0 )
                    change = ((CurrentLevel.NumericValue - PreviousClose.NumericValue) / PreviousClose.NumericValue) * 100;
                
                return change;  
            }
        }

        public Color MarketColor
        {
            get
            {
                Color color = Form1.TextForeColor;

                if (Change > PreviousClose.NumericValue / 1000)
                    color = Color.LimeGreen;
                else if (Change < -PreviousClose.NumericValue / 1000)
                    color = Color.Red;

                return color;
            }
        }
    }

    public class Markets
    {
        private MarketData dow;
        private MarketData sAndPdow;
        private MarketData nasdaq;

        public MarketData Dow { get => dow; set => dow = value; }
        public MarketData Nasdaq { get => nasdaq; set => nasdaq = value; }
        public MarketData SAndP { get => sAndPdow; set => sAndPdow = value; }

        public Markets()
        {
            Dow = new MarketData();
            Nasdaq = new MarketData();
            SAndP = new MarketData();
        }

        public async Task<Markets> GetAllMarketData()
        {
            try
            {
                Dow = await GetMarketData("^GSPC", true);
                Nasdaq = await GetMarketData("^DJI", true);
                SAndP = await GetMarketData("^IXIC", true);
            }
            catch 
            {
                Dow.CurrentLevel.StringValue = "0";
                Nasdaq.CurrentLevel.StringValue = "0";
                SAndP.CurrentLevel.StringValue = "0";
            }

            return this;
        }

        public async Task<MarketData> GetMarketData(string ticker, bool showMessageBox)
        {
            FpmAPI fpmAPI = new FpmAPI();
          
            MarketData marketData = await fpmAPI.GetQuote(ticker, showMessageBox);

            return marketData;
        }
    }
}
