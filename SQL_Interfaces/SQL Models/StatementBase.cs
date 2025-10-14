using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.DataAnnotations;

namespace StockApi.SQL_Models
{
    public class StatementBase
    {
        [AutoIncrement]
        public int Id { get; set; }

        public string Ticker { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
