using ServiceStack.OrmLite;
using SQL_Interfaces.SQL_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace SQL_Interfaces
{
    public class SqlInterfaces
    {
        public static OrmLiteConnectionFactory FinancialStatementFactory()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["StockDataConnection"].ConnectionString;

            OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);

            return factory;
        }

        public void SaveStatements()
        {
            Debug.WriteLine("SaveStatements()");

            var factory = SqlInterfaces.FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<CashFlow>();

                //foreach (string key in _statementCache.Keys)
                //{
                //    db.Delete<IncomeStatement>(statement => statement.HashKey == _statementCache[key].HashKey);
                //    db.Delete<BalanceSheet>(statement => statement.HashKey == _statementCache[key].HashKey);
                //    db.Delete<CashFlow>(statement => statement.HashKey == _statementCache[key].HashKey);
                //}

                //foreach (string key in _statementCache.Keys)
                //{
                //    db.Insert<IncomeStatement>(_statementCache[key].IncomeStatement);
                //    db.Insert<BalanceSheet>(_statementCache[key].BalanceSheet);
                //    db.Insert<CashFlow>(_statementCache[key].CashFlow);
                //}
            }
        }
    }
}
