using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace OrmLite.SQL_Models
{
    public class CashFlow 
    {
        [AutoIncrement]
        public int Id { get; set; }
        
        public string Ticker { get; set; }
        
        public DateTime StartDate { get; set; }
   
        public DateTime EndDate { get; set; }

        public double FreeCashFlow { get; set; }

        public double OperatingCashFlow { get; set; }

        public double EndCashPosition { get; set; }

        public CashFlow() { }
    }
}
