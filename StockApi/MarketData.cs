using Drake.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public async Task<MarketData> GetMarketData(string ticker, bool showMessageBox)
        {
            FpmAPI fpmAPI = new FpmAPI();
            MarketData marketData = await fpmAPI.GetQuote(ticker, showMessageBox);

            return marketData;
        }
    }
}
