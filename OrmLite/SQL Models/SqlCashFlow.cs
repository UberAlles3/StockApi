using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace SqlLayer.Models
{
    public class SqlCashFlow 
    {
        public SqlCashFlow() { }

        [AutoIncrement]
        public int Id { get; set; }
        
        public string Ticker { get; set; }
        
        public int Year { get; set; }
   
        public double FreeCashFlow { get; set; }

        public double OperatingCashFlow { get; set; }

        public double EndCashPosition { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
