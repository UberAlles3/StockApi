using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace SqlLayer.SQL_Models
{
    [Alias("IncomeStatement")]
    public class SqlIncomeStatement
    {
        public SqlIncomeStatement() { }

        [AutoIncrement]
        public int Id { get; set; }

        public string Ticker { get; set; }

        public int Year { get; set; }

        public double Revenue { get; set; }

        public double CostOfRevenue { get; set; }

        public double OperatingExpense { get; set; }

        public double NetIncome { get; set; }

        public double BasicEPS { get; set; }
        
        public DateTime UpdateDate { get; set; }
    }
}
