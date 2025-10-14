using System;
using System.Collections.Generic;
using System.Text;

namespace SQL_Interface.SQL_Models
{
    public class CashFlow : StatementBase
    {
        public double FreeCashFlow { get; set; }

        public double OperatingCashFlow { get; set; }

        public double EndCashPosition { get; set; }

        public CashFlow() { }
    }
}
