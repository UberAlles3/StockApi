using ServiceStack.OrmLite;
using SQL_Interface.SQL_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace SQL_Interface
{
    public class SqlInterface
    {
        public static OrmLiteConnectionFactory FinancialStatementFactory()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["StockDataConnection"].ConnectionString;


            return new OrmLiteConnectionFactory(
                 ConfigurationManager.ConnectionStrings["StockDataConnection"].ConnectionString,
                SqlServerDialect.Provider);
        }

        public void SaveStatements()
        {
            Debug.WriteLine("SaveStatements()");
            string connectionString = ConfigurationManager.ConnectionStrings["StockDataConnection"].ConnectionString;


            //var factory = SqlInterface.FinancialStatementFactory();

            //using (IDbConnection db = factory.OpenDbConnection())
            //{
            //    db.CreateTableIfNotExists<CashFlow>();

            //    //foreach (string key in _statementCache.Keys)
            //    //{
            //    //    db.Delete<IncomeStatement>(statement => statement.HashKey == _statementCache[key].HashKey);
            //    //    db.Delete<BalanceSheet>(statement => statement.HashKey == _statementCache[key].HashKey);
            //    //    db.Delete<CashFlow>(statement => statement.HashKey == _statementCache[key].HashKey);
            //    //}

            //    //foreach (string key in _statementCache.Keys)
            //    //{
            //    //    db.Insert<IncomeStatement>(_statementCache[key].IncomeStatement);
            //    //    db.Insert<BalanceSheet>(_statementCache[key].BalanceSheet);
            //    //    db.Insert<CashFlow>(_statementCache[key].CashFlow);
            //    //}
            //}
        }
    }
}
