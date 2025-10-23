using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;
using ServiceStack.OrmLite;
using SqlLayer.SQL_Models;

namespace SqlLayer
{
    public class SqlFinancialStatement
    {
        /// <summary>
        /// Get the database connection
        /// </summary>
        /// <returns></returns>
        public static OrmLiteConnectionFactory FinancialStatementFactory()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["StockDataConnection"].ConnectionString;
            OrmLiteConnectionFactory factory = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);
            return factory;
        }
        public SqlCashFlow SqlCashFlow { get; set; }
        public SqlIncomeStatement SqlIncomeStatement { get; set; }
        public SqlStatistic SqlStatistics { get; set; }

 
        public SqlFinancialStatement()
        {
            //this.IncomeStatement = new IncomeStatement();
            this.SqlCashFlow = new SqlCashFlow();
        }

        //public void SaveFinancials()
        //{
        //    Debug.WriteLine("SaveFinancials()");

        //    var factory = FinancialStatementFactory();

        //    using (IDbConnection db = factory.OpenDbConnection())
        //    {
        //        db.CreateTableIfNotExists<SqlCashFlow>();

        //        db.Delete<SqlCashFlow>(x => x.Year > DateTime.Now.AddYears(-5).Year);

        //        //foreach (string key in _statementCache.Keys)
        //        //{
        //        SqlCashFlow cashFlow = new SqlCashFlow() { Year = DateTime.Now.Year, EndCashPosition = 111, FreeCashFlow = 222.222, Ticker = "AAPL", OperatingCashFlow = 555, UpdateDate = DateTime.Now.Date};
        //        db.Insert<SqlCashFlow>(cashFlow);
        //        //    db.Insert<BalanceSheet>(_statementCache[key].BalanceSheet);
        //        //    db.Insert<CashFlow>(_statementCache[key].CashFlow);
        //        //}
        //    }
        //}

        public void SaveIncomeStatements(List<SqlIncomeStatement> sqlIncomeStatements)
        {
            Debug.WriteLine("SaveIncomeStatements()");

            if (sqlIncomeStatements.Count == 0)
                return;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlIncomeStatement>();
                db.Delete<SqlIncomeStatement>(x => x.Ticker == sqlIncomeStatements[0].Ticker && x.Year > DateTime.Now.AddYears(-5).Year);

                foreach (SqlIncomeStatement row in sqlIncomeStatements)
                {
                  db.Insert<SqlIncomeStatement>(row);
                }
            }
        }

        public List<SqlIncomeStatement> GetIncomeStatements(string ticker)
        {
            List<SqlIncomeStatement> sqlIncomeStatementsList;

            Debug.WriteLine("GetIncomeStatements()");

            var factory = FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlStatistic>();
                sqlIncomeStatementsList = db.Select<SqlIncomeStatement>(x => x.Ticker == ticker && x.Year > DateTime.Now.Year - 5);
            }

            return sqlIncomeStatementsList;
        }

        public void SaveStatistics(SqlStatistic sqlStatistic)
        {
            Debug.WriteLine("SaveStatistics()");

            if (sqlStatistic == null)
                return;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlStatistic>();

                db.Delete<SqlStatistic>(x => x.Ticker == sqlStatistic.Ticker);

                db.CreateTableIfNotExists<SqlStatistic>();
                db.Insert<SqlStatistic>(sqlStatistic);
            }
        }

        public List<SqlStatistic> GetStatistics(string ticker)
        {
            List<SqlStatistic> sqlStatisticsList;

            Debug.WriteLine("GetStatistics()");

            var factory = FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlStatistic>();
                sqlStatisticsList = db.Select<SqlStatistic>(x => x.Ticker == ticker);
            }

            return sqlStatisticsList;
        }

        public void SavePriceHistories(List<SqlPriceHistory> sqlPriceHistories)
        {
            Debug.WriteLine("SavePriceHistories()");

            if (sqlPriceHistories.Count == 0)
                return;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlPriceHistory>();
                db.Delete<SqlPriceHistory>(x => x.Ticker == sqlPriceHistories[0].Ticker);

                foreach (SqlPriceHistory row in sqlPriceHistories)
                {
                    if(row != null)
                       db.Insert<SqlPriceHistory>(row);
                }
            }
        }

        public List<SqlPriceHistory> GetPriceHistories(string ticker)
        {
            List<SqlPriceHistory> sqlPriceHistoriesList;

            Debug.WriteLine("GetPriceHistories()");

            var factory = FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlPriceHistory>();
                sqlPriceHistoriesList = db.Select<SqlPriceHistory>(x => x.Ticker == ticker);
            }

            return sqlPriceHistoriesList;
        }
    }
}
