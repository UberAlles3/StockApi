using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ServiceStack.OrmLite;
using SqlLayer.SQL_Models;

namespace SqlLayer
{
    public class SqlCrudOperations
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

        public SqlCrudOperations()
        {
        }

        //////////////////////////////////////////////////////////////
        ///                         Ticker
        public SqlTicker GetTicker(string ticker)
        {
            SqlTicker sqlTicker = GetTickerList(ticker).FirstOrDefault();

            if (sqlTicker == null) sqlTicker = new SqlTicker() { Id = 0, IsFund = false, IsPreRevenueCompany = false };

            return sqlTicker;
        }

        public List<SqlTicker> GetTickerList(string ticker)
        {
            List<SqlTicker> sqlTickerList;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlTicker>();
                sqlTickerList = db.Select<SqlTicker>(x => x.Ticker == ticker);
            }

            return sqlTickerList;
        }

        //////////////////////////////////////////////////////////////
        ///                         Summary
        public List<SqlSummary> GetSummaryList(string ticker)
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

        public List<SqlSummary> GetAllSummaryList()
        {
            List<SqlSummary> sqlSummaryList;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlSummary>();
                sqlSummaryList = db.Select<SqlSummary>();
            }

            return sqlSummaryList;
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


        //////////////////////////////////////////////////////////////
        ///                     Income Statement
        public List<SqlIncomeStatement> GetIncomeStatementList(string ticker)
        {
            List<SqlIncomeStatement> sqlIncomeStatementList;

            Debug.WriteLine("GetIncomeStatements()");

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlIncomeStatement>();
                sqlIncomeStatementList = db.Select<SqlIncomeStatement>(x => x.Ticker == ticker && x.Year > DateTime.Now.Year - 5);
            }

            return sqlIncomeStatementList;
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

        public void DeleteIncomeStatement(string ticker)
        {
            Debug.WriteLine("DeleteIncomeStatement()");

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlIncomeStatement>();
                db.Delete<SqlIncomeStatement>(x => x.Ticker == ticker);
            }

            return;
        }

        //////////////////////////////////////////////////////////////
        ///                        Statistics
        public List<SqlStatistic> GetStatisticList(string ticker)
        {
            List<SqlStatistic> sqlStatisticList;

            Debug.WriteLine("GetStatistics()");

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlStatistic>();
                sqlStatisticList = db.Select<SqlStatistic>(x => x.Ticker == ticker);
            }

            return sqlStatisticList;
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

        public void DeleteStatistics(string ticker)
        {
            Debug.WriteLine("DeleteStatistics()");

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlStatistic>();
                db.Delete<SqlStatistic>(x => x.Ticker == ticker);
            }

            return;
        }

        //////////////////////////////////////////////////////////////
        ///                        Price History
        public List<SqlPriceHistory> GetPriceHistoryList(string ticker)
        {
            List<SqlPriceHistory> sqlPriceHistoryList;

            Debug.WriteLine("GetPriceHistories()");

            var factory = FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlPriceHistory>();
                sqlPriceHistoryList = db.Select<SqlPriceHistory>(x => x.Ticker == ticker);
            }

            return sqlPriceHistoryList;
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

        //////////////////////////////////////////////////////////////
        ///                       Cash Flow
        public List<SqlCashFlow> GetCashFlowList(string ticker)
        {
            List<SqlCashFlow> sqlCashFlowList;

            Debug.WriteLine("GetCashFlows()");

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlCashFlow>();
                sqlCashFlowList = db.Select<SqlCashFlow>(x => x.Ticker == ticker && x.Year > DateTime.Now.Year - 5);
            }

            return sqlCashFlowList;
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

        //////////////////////////////////////////////////////////////
        ///                         Metrics
        public List<SqlMetric> GetMetricList(DateTime minDate, string ticker)
        {
            List<SqlMetric> sqlMetricList;

            var factory = FinancialStatementFactory();
            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlMetric>();
                if(ticker == null)
                    sqlMetricList = db.Select<SqlMetric>(x => x.Year*100+x.Month >= minDate.Year*100+minDate.Month);
                else
                    sqlMetricList = db.Select<SqlMetric>(x => x.Year * 100 + x.Month >= minDate.Year * 100 + minDate.Month && x.Ticker == ticker);
            }

            return sqlMetricList;
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
     }
}
