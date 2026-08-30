using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockApi
{
    public partial class JointTradeForm : Form
    {
        public JointTradeForm()
        {
            InitializeComponent();
        }

        private void JointTradeForm_Load(object sender, EventArgs e)
        {
            UpdateMetricValueFromPositionsList();
        }

        private static void UpdateMetricValueFromPositionsList()
        {
            var sourceDict = ExcelManager.PositionList.ToDictionary(src => src.Symbol, src => src.TotalMetric);

            // Update the Metric value form the Positions list
            ExcelManager.JointTradeList.ForEach(target =>
            {
                if (sourceDict.TryGetValue(target.Symbol, out var newValue))
                {
                    target.Metric = newValue;
                }
            });

            // Update the Metric value if it's zero
            ExcelManager.JointTradeList.ForEach(x => { if (x.Metric == 0) x.Metric = 1.1; });
        }

        private void btnGetTrades_Click(object sender, EventArgs e)
        {
            // Eliminate any stocks that don't have trades over a year old.

            // Find first trade for each stock
            List<ExcelTrade> firstTrade = ExcelManager.JointTradeList
            .GroupBy(p => p.Symbol)
            .Select(g => g.OrderBy(p => p.TradeDate).First())
            .OrderBy(x => x.Symbol)
            .ToList();

            // Find last trade for each stock
            List<ExcelTrade> lastTrade = ExcelManager.JointTradeList
            .GroupBy(p => p.Symbol)
            .Select(g => g.OrderByDescending(p => p.TradeDate).First())
            .OrderBy(x => x.Symbol)
            .ToList();

            foreach(ExcelPosition jointPosition in ExcelManager.JointPositionList)
            {
                // skip if the first trade isn't over a year old
                ExcelTrade firstTradeRow = firstTrade.Where(x => x.Symbol == jointPosition.Symbol).FirstOrDefault();
                ExcelTrade lastTradeRow = lastTrade.Where(x => x.Symbol == jointPosition.Symbol).FirstOrDefault();

                if (firstTradeRow == null)
                {
                    //txtTickerList.Text += "First trade not found.\r\n";
                    continue;
                }

                if(firstTradeRow.TradeDate > DateTime.Now.AddYears(-1))
                {
                    //txtTickerList.Text += "No trades over a year old.\r\n";
                    continue;
                }

                txtTickerList.Text += jointPosition.Symbol.PadRight(7) + firstTradeRow.Metric.ToString("0.00").PadLeft(4) + firstTradeRow.TradeDate.ToShortDateString().PadLeft(11) + " ";

                // Is the current price 20% down or 35% up from last trade? Metric will adjust.
                if(jointPosition.Price > lastTradeRow.Price * (1.35 * ((lastTradeRow.Metric + 4) / 5)))
                {
                    txtTickerList.Text += jointPosition.Price.ToString().PadLeft(10) + lastTradeRow.Price.ToString().PadLeft(10)
                       + (((jointPosition.Price / lastTradeRow.Price) - 1) * 100).ToString("###").PadLeft(6) + "%     ";

                    if (jointPosition.Quantity < 2)
                        txtTickerList.Text += "Only 1 share. No sale.";
                    else
                        txtTickerList.Text += "Sell " + (jointPosition.Quantity / 5).ToString("##");
                }

                // Is the current price 20% down or 35% up from last trade? Metric will adjust.
                if (jointPosition.Price < lastTradeRow.Price * (.7 * ((lastTradeRow.Metric + 1) / 2)))
                {
                    txtTickerList.Text += jointPosition.Price.ToString().PadLeft(10) + lastTradeRow.Price.ToString().PadLeft(10)
                       + (((jointPosition.Price / lastTradeRow.Price) - 1) * 100).ToString("###").PadLeft(6) + "%     ";

                    txtTickerList.Text += "Buy " + (jointPosition.Quantity / 4).ToString("##");
                }


                txtTickerList.Text += "\r\n";
            }
        }
    }
}
