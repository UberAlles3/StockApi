using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace SqlLayer.SQL_Models
{

    [Alias("Metric")]
    public class SqlMetric
    {
        public SqlMetric() { }

        [AutoIncrement]
        public int Id { get; set; }

        public string Ticker { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public double PriceTrend { get; set; }

        public double EarningsPerShare { get; set; }

        public double TargetPrice { get; set; }

        public double PriceBook { get; set; }

        public double Dividend { get; set; }

        public double ProfitMargin { get; set; }

        public double Revenue { get; set; }

        public double Profit { get; set; }

        public double BasicEps { get; set; }

        public double CashDebt { get; set; }

        public double Valuation { get; set; }

        public double CashFlow { get; set; }
        
        public double FinalMetric { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
