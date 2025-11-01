using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlLayer.SQL_Models
{
    [Alias("Ticker")]
    public class SqlTicker
    {
        public SqlTicker() { }

        [AutoIncrement]
        public int Id { get; set; }

        public string Ticker { get; set; }

        public bool IsFund { get; set; }

        public bool IsPreRevenueCompany { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
