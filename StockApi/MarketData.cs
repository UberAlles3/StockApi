﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StockApi
{
    public class MarketData
    {
        public DateTime RetreivedDate { get; set; }
        public string Ticker { get; set; } = "";
        public StringSafeNumeric<decimal> PreviousClose { get; set; } = new StringSafeNumeric<decimal>("");
        public StringSafeNumeric<decimal> CurrentLevel { get; set; } = new StringSafeNumeric<decimal>("");
        public decimal Change
        {
            get => CurrentLevel.NumericValue - PreviousClose.NumericValue;
        }
        public decimal PercentageChange
        {
            get => ((CurrentLevel.NumericValue - PreviousClose.NumericValue) / PreviousClose.NumericValue) * 100; 
        }

        public Color MarketColor
        {
            get
            {
                Color color = Color.LightSteelBlue;

                if (Change > PreviousClose.NumericValue / 1000)
                    color = Color.LimeGreen;
                else if (Change < -PreviousClose.NumericValue / 1000)
                    color = Color.Red;

                return color;
            }
        }
    }
}
