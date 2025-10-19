using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;
using ServiceStack.OrmLite;
using SqlLayer.Models;

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
        public SqlCashFlow CashFlow { get; set; }
        //public BalanceSheet BalanceSheet { get; set; }
        //public IncomeStatement IncomeStatement { get; set; }

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
            this.CashFlow = new SqlCashFlow();
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
    }
}
