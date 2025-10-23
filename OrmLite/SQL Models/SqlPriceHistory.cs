using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace SqlLayer.SQL_Models
{
    [Alias("PriceHistory")]
    public class SqlPriceHistory
    {
        public SqlPriceHistory() { }

        [AutoIncrement]
        public int Id { get; set; }

        public string Ticker { get; set; }

        public string PeriodType { get; set; }
        
        public DateTime PriceDate { get; set; }

        public double Price { get; set; }

        public double Volume { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
