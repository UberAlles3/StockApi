using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlLayer.SQL_Models
{
    public enum ValuationEnum
    {
        UnderValued,
        FairValue,
        OverValued
    }

    [Alias("Summary")]
    public class SqlSummary
    {
        public SqlSummary() { }

        [AutoIncrement]
        public int Id { get; set; }

        public string Ticker { get; set; }

        public ValuationEnum Valuation { get; set; }

        public double Dividend { get; set; }

        public double EarningsPerShare { get; set; }

        public double ProfitMargin { get; set; }

        public double PriceBook { get; set; }

        public double OneYearTargetPrice { get; set; }

        public double Price { get; set; }

        public double Volatility { get; set; }

        public double YearsRangeLow { get; set; }

        public double YearsRangeHigh { get; set; }

        public double ForwardPE { get; set; }
        public DateTime? EarningsDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
