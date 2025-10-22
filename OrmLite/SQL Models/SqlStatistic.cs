using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace SqlLayer.SQL_Models
{
    [Alias("Statistics")]
    public class SqlStatistic
    {
        public SqlStatistic() { }

        [AutoIncrement]
        public int Id { get; set; }

        public string Ticker { get; set; }

        public double Cash { get; set; }

        public double Debt { get; set; }
        
        public double DebtEquity { get; set; }

        public double ShortInterest { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
