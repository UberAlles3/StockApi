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
        //public BalanceSheet BalanceSheet { get; set; }

        #region Pass-Through Properties
        //TODO


        //public string Ticker
        //{
        //    get
        //    {
        //        return this.BalanceSheet.Ticker;
        //    }
        //    set
        //    {
        //        this.BalanceSheet.Ticker = value;
        //        this.IncomeStatement.Ticker = value;
        //        this.CashFlow.Ticker = value;
        //    }
        //}

        //public DateTime StartDate
        //{
        //    get
        //    {
        //        return this.BalanceSheet.StartDate;
        //    }
        //    set
        //    {
        //        this.BalanceSheet.StartDate = value;
        //        this.IncomeStatement.StartDate = value;
        //        this.CashFlow.StartDate = value;
        //    }
        //}

        //public DateTime EndDate
        //{
        //    get
        //    {
        //        return this.BalanceSheet.EndDate;
        //    }
        //    set
        //    {
        //        this.BalanceSheet.EndDate = value;
        //        this.IncomeStatement.EndDate = value;
        //        this.CashFlow.EndDate = value;
        //    }
        //}

        #endregion

        public SqlFinancialStatement()
        {
            //this.IncomeStatement = new IncomeStatement();
            this.SqlCashFlow = new SqlCashFlow();
        }

        public void SaveFinancials()
        {
            Debug.WriteLine("SaveFinancials()");

            var factory = FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlCashFlow>();

                db.Delete<SqlCashFlow>(x => x.Year > DateTime.Now.AddYears(-5).Year);

                //foreach (string key in _statementCache.Keys)
                //{
                SqlCashFlow cashFlow = new SqlCashFlow() { Year = DateTime.Now.Year, EndCashPosition = 111, FreeCashFlow = 222.222, Ticker = "AAPL", OperatingCashFlow = 555, UpdateDate = DateTime.Now.Date};
                db.Insert<SqlCashFlow>(cashFlow);
                //    db.Insert<BalanceSheet>(_statementCache[key].BalanceSheet);
                //    db.Insert<CashFlow>(_statementCache[key].CashFlow);
                //}
            }
        }

        public void SaveIncomeStatements(List<SqlIncomeStatement> sqlIncomeStatements)
        {
            Debug.WriteLine("SaveIncomeStatements()");

            var factory = FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<SqlIncomeStatement>();

                db.Delete<SqlIncomeStatement>(x => x.Year > DateTime.Now.AddYears(-5).Year);

                foreach (SqlIncomeStatement sis in sqlIncomeStatements)
                {
                  db.Insert<SqlIncomeStatement>(sis);
                }
            }
        }

        public List<SqlIncomeStatement> GetIncomeStatements(string ticker)
        {
            List<SqlIncomeStatement> sqlIncomeStatementsList;


            Debug.WriteLine("SaveIncomeStatements()");

            var factory = FinancialStatementFactory();

            using (IDbConnection db = factory.OpenDbConnection())
            {
                sqlIncomeStatementsList = db.Select<SqlIncomeStatement>(x => x.Year > DateTime.Now.Year - 5);
            }

            return sqlIncomeStatementsList;
        }
    }
}
