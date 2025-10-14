using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace StockApi.SQL_Code
{
    class SQLInterface
    {
        public static OrmLiteConnectionFactory FinancialStatementFactory()
        {
            return new OrmLiteConnectionFactory(
                 ConfigurationManager.ConnectionStrings["StockDataConnection"].ConnectionString,
                SqlServerDialect.Provider);
        }

        private void SaveStatements()
        {
            Debug.WriteLine("SaveStatements()");
            var factory = SQLInterface.FinancialStatementFactory();

            //using (IDbConnection db = factory.OpenDbConnection())
            //{
            //    db.CreateTableIfNotExists<IncomeStatement>();
            //    db.CreateTableIfNotExists<BalanceSheet>();
            //    db.CreateTableIfNotExists<CashFlow>();

            //    foreach (string key in _statementCache.Keys)
            //    {
            //        db.Delete<IncomeStatement>(statement => statement.HashKey == _statementCache[key].HashKey);
            //        db.Delete<BalanceSheet>(statement => statement.HashKey == _statementCache[key].HashKey);
            //        db.Delete<CashFlow>(statement => statement.HashKey == _statementCache[key].HashKey);
            //    }

            //    foreach (string key in _statementCache.Keys)
            //    {
            //        db.Insert<IncomeStatement>(_statementCache[key].IncomeStatement);
            //        db.Insert<BalanceSheet>(_statementCache[key].BalanceSheet);
            //        db.Insert<CashFlow>(_statementCache[key].CashFlow);
            //    }
            //}
        }
    }
}
