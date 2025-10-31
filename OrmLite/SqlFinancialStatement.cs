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
        public static OrmLiteConnectionFactory FactorySingleton = null;

        /// <summary>
        /// Get the database connection
        /// </summary>
        public static OrmLiteConnectionFactory FinancialStatementFactory()
        {
            if(FactorySingleton == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["StockDataConnection"].ConnectionString;
                FactorySingleton = new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider);
            }
            return FactorySingleton;
        }

        public SqlFinancialStatement()
        {
        }


        public void SaveSummary(SqlSummary sqlSummary)
        {
            Debug.WriteLine("SaveSummary()");

            if (sqlSummary == null)
                return;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlSummary>();
                db.Delete<SqlSummary>(x => x.Ticker == sqlSummary.Ticker);
                db.Insert<SqlSummary>(sqlSummary);
            }
        }

        public List<SqlSummary> GetSummary(string ticker)
        {
            List<SqlSummary> sqlSummaryList;

            Debug.WriteLine("GetSummary()");

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlSummary>();
                sqlSummaryList = db.Select<SqlSummary>(x => x.Ticker == ticker);
            }

            return sqlSummaryList;
        }


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
                db.CreateTableIfNotExists<SqlIncomeStatement>();
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

        public void SaveCashFlows(List<SqlCashFlow> sqlCashFlows)
        {
            Debug.WriteLine("SaveCashFlows()");

            if (sqlCashFlows.Count == 0)
                return;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlCashFlow>();
                db.Delete<SqlCashFlow>(x => x.Ticker == sqlCashFlows[0].Ticker && x.Year > DateTime.Now.AddYears(-5).Year);

                foreach (SqlCashFlow row in sqlCashFlows)
                {
                    db.Insert<SqlCashFlow>(row);
                }
            }
        }

        public List<SqlCashFlow> GetCashFlows(string ticker)
        {
            List<SqlCashFlow> sqlCashFlowsList;

            Debug.WriteLine("GetCashFlows()");

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlCashFlow>();
                sqlCashFlowsList = db.Select<SqlCashFlow>(x => x.Ticker == ticker && x.Year > DateTime.Now.Year - 5);
            }

            return sqlCashFlowsList;
        }

        public void SaveMetrics(SqlMetric sqlMetric)
        {
            Debug.WriteLine("SaveMetrics()");

            if (sqlMetric == null)
                return;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlMetric>();
                db.Delete<SqlMetric>(x => x.Ticker == sqlMetric.Ticker && x.Year == DateTime.Now.Year && x.Month == DateTime.Now.Month);
                db.Insert<SqlMetric>(sqlMetric);
            }
        }

        public List<SqlMetric> GetMetrics()
        {
            List<SqlMetric> sqlMetricList;

            Debug.WriteLine("GetMetrics()");

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlMetric>();
                sqlMetricList = db.Select<SqlMetric>(x => x.Year == DateTime.Now.Year && x.Month == DateTime.Now.Month);
            }

            return sqlMetricList;
        }
    }
}
