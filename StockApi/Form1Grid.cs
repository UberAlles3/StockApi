using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StockApi
{
    public class Form1Grid
    {
        public static void SetupPriceHistoryGridColumns(DataGridView dataGridView1, Color foreColor, Color backgroundColor)
        {
            dataGridView1.DefaultCellStyle.ForeColor = Form1.TextForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            dataGridView1.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].HeaderText = "Date";
            dataGridView1.Columns[3].Width = 120;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[4].Width = 120;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
        }

        public static void SetupTradeGridColumns(DataGridView dataGridView2, Color foreColor, Color backgroundColor)
        {
            dataGridView2.DefaultCellStyle.ForeColor = Form1.TextForeColor;
            dataGridView2.DefaultCellStyle.SelectionForeColor = foreColor;
            dataGridView2.DefaultCellStyle.BackColor = backgroundColor;
            dataGridView2.DefaultCellStyle.SelectionBackColor = backgroundColor;
            dataGridView2.Columns[0].HeaderText = "Date";
            dataGridView2.Columns[1].Visible = false;
            dataGridView2.Columns[2].HeaderText = "Buy/Sell";
            dataGridView2.Columns[2].Width = 60;
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[3].HeaderText = "Quan.";
            dataGridView2.Columns[3].Width = 60;
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[3].DefaultCellStyle.Format = "#####";
            dataGridView2.Columns[4].Visible = false; // Hide ticker

            dataGridView2.Columns[5].HeaderText = "Price";
            dataGridView2.Columns[5].Width = 77;
            dataGridView2.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[5].DefaultCellStyle.Format = "N2";
            dataGridView2.Columns[6].HeaderText = "Tot. Shares";
            dataGridView2.Columns[6].Width = 68;
            dataGridView2.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView2.Columns[6].DefaultCellStyle.Format = "#####";

            dataGridView2.Columns[7].Visible = false;
            dataGridView2.Columns[8].Visible = false;
            dataGridView2.Columns[9].Visible = false;
        }

        public static void ColorTradeGrid(DataGridView dataGridView2, DataTable TickerTradesDataTable)
        {
            string min;
            string max;
            float previous;
            float current;
            int i = 0;
            // Color trades based on groups of 5 rows, the high and low for the 5 rows
            foreach (DataRow r in TickerTradesDataTable.Rows)
            {
                // Color Buy and Sells
                if (r.ItemArray[2].ToString().Trim().ToLower().Contains("buy"))
                    dataGridView2.Rows[i].Cells[2].Style.ForeColor = Color.Lime;
                else
                    dataGridView2.Rows[i].Cells[2].Style.ForeColor = Color.Yellow;

                int i2 = i;
                if (i % 1 == 0) // every 2nd pass, evaluate
                {
                    // Find Min, Max trade price
                    min = TickerTradesDataTable.Select().Skip(i).Take(5).AsEnumerable()
                        .Min(row => row[5])
                        .ToString();
                    max = TickerTradesDataTable.Select().Skip(i).Take(5).AsEnumerable()
                        .Max(row => row[5])
                        .ToString();

                    for (i2 = i; i2 < i + Math.Min(5, (TickerTradesDataTable.Rows.Count)) && i < TickerTradesDataTable.Rows.Count - Math.Min(4, (TickerTradesDataTable.Rows.Count - 1)); i2++)
                    {
                        if (TickerTradesDataTable.Rows[i2].ItemArray[5].ToString() == max) // High - Green coloring
                        {
                            current = previous = 0;
                            try
                            {
                                if (i2 > 0)
                                    previous = float.Parse(TickerTradesDataTable.Rows[i2 - 1].ItemArray[5].ToString());
                                current = float.Parse(TickerTradesDataTable.Rows[i2].ItemArray[5].ToString());
                            }
                            catch { } // eat the error
                            if (current > previous)
                            {
                                dataGridView2.Rows[i2].Cells[5].Style.ForeColor = Color.Lime;
                                if (i2 > 1 && dataGridView2.Rows[i2 - 1].Cells[5].Style.ForeColor == Color.Lime)
                                    dataGridView2.Rows[i2 - 1].Cells[5].Style.ForeColor = Color.Silver;
                            }
                        }
                        if (TickerTradesDataTable.Rows[i2].ItemArray[5].ToString() == min) // Low - Red coloring
                        {
                            // get previous, if previous < than this low, don't color and leave previous alone
                            current = previous = 0;
                            try
                            {
                                if (i2 > 0)
                                    previous = float.Parse(TickerTradesDataTable.Rows[i2 - 1].ItemArray[5].ToString());
                                else
                                    previous = 10000;
                                current = float.Parse(TickerTradesDataTable.Rows[i2].ItemArray[5].ToString());
                            }
                            catch { } // eat the error
                            if (current < previous)
                            {
                                dataGridView2.Rows[i2].Cells[5].Style.ForeColor = Color.Red;
                                if (i2 > 1 && dataGridView2.Rows[i2 - 1].Cells[5].Style.ForeColor == Color.Red)
                                    dataGridView2.Rows[i2 - 1].Cells[5].Style.ForeColor = Color.Silver;
                            }
                        }
                    }
                }

                i++;
            }
        }

    }
}
