﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockApi
{
    public partial class PerformanceForm : Form
    {
        List<PerformanceItem> _performanceList = null;

        public PerformanceForm(List<PerformanceItem> performanceList)
        {
            InitializeComponent();
            _performanceList = performanceList;
        }

        private void PerformanceForm_Load(object sender, EventArgs e)
        {
            var bindingList = new BindingList<PerformanceItem>(_performanceList);
            dataGridView2.DataSource = new BindingSource(bindingList, null); 

            this.BackColor = dataGridView2.BackgroundColor;
            dataGridView2.DefaultCellStyle.ForeColor = Color.LightSteelBlue;
            dataGridView2.DefaultCellStyle.SelectionForeColor = Color.LightSteelBlue;
            dataGridView2.DefaultCellStyle.BackColor = Color.Black;
            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Black;

            dataGridView2.Columns[0].Width = 80;
            ////dataGridView2.Columns[0].DefaultCellStyle.Format = "MM/dd/yyyy";
            dataGridView2.Columns[1].Width = 50;
            // Quantity
            dataGridView2.Columns[2].Width = 60;
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[2].DefaultCellStyle.Format = "#####";
            // Bought Price
            dataGridView2.Columns[3].Width = 75;
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[3].DefaultCellStyle.Format = "N2";
            // Current Price
            dataGridView2.Columns[4].Width = 75;
            dataGridView2.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[4].DefaultCellStyle.Format = "N2";
            // Profit
            dataGridView2.Columns[5].Width = 75;
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[5].DefaultCellStyle.Format = "N2";
            // Total Profit
            dataGridView2.Columns[6].Width = 75;
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[6].DefaultCellStyle.Format = "N2";
            // DOW
            dataGridView2.Columns[7].Width = 70;
            dataGridView2.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[7].DefaultCellStyle.Format = "N0";

            dataGridView2.Refresh();

            // DOW gain / loss
            decimal dowGain = Convert.ToDecimal((_performanceList[0].DowLevel - _performanceList[19].DowLevel)) / Convert.ToDecimal(_performanceList[0].DowLevel) * 100;
            lblDowGain.Text = dowGain.ToString("N1") + "%";

            // Total Profit
            decimal totProfit = _performanceList.Sum(x => x.TotalProfit);
            lblTotalProfit.Text = totProfit.ToString("0.00");

            // Total Cost
            decimal totCost = _performanceList.Sum(x => (x.Quantity) * x.BuyPrice);
            // Current Worth
            decimal totWorth = _performanceList.Sum(x => (x.Quantity) * x.CurrentPrice);
            // Buy Gain
            decimal buyGain = ((totWorth - totCost) / totCost) * 100M;
            lblPortfolioGain.Text = buyGain.ToString("N1") + "%";

        }
    }
}
