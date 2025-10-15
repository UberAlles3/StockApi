using ServiceStack.OrmLite;
using OrmLite.SQL_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace OrmLite
{ 
    public class SqlInterface
    {
        public static OrmLiteConnectionFactory FinancialStatementFactory()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["StockDataConnection"].ConnectionString;

            OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);

            return factory;
        }

        public void SaveCashFlow()
        {
            Debug.WriteLine("SaveCashFlow()");

            var factory = FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<CashFlow>();

                db.Delete<CashFlow>(x => x.StartDate > DateTime.Now.AddYears(-5));

                //foreach (string key in _statementCache.Keys)
                //{
                CashFlow cashFlow = new CashFlow() { StartDate = DateTime.Now.Date, EndDate = DateTime.Now.Date, EndCashPosition = 111, FreeCashFlow = 222.222, Ticker = "AAPL", OperatingCashFlow = 555 };      
                db.Insert<CashFlow>(cashFlow);
                //    db.Insert<BalanceSheet>(_statementCache[key].BalanceSheet);
                //    db.Insert<CashFlow>(_statementCache[key].CashFlow);
                //}
            }
        }
    }
}
