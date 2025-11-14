using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PC = StockApi.ExcelManager.PositionColumns;

namespace StockApi
{
    public partial class ExcelForm : Form
    {
        DataTable _positionsDataTable;
        public ExcelForm(DataTable positionsDataTable)
        {
            _positionsDataTable = positionsDataTable;
            InitializeComponent();
        }

        private void ExeclForm_Load(object sender, EventArgs e)
        {
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            string[] lines;
            string[] cells;
            StringBuilder sb = new StringBuilder();

            lines = textBox1.Text.Split("\r\n");
            
            foreach(string line in lines)
            {
                cells = line.Split("\t");
                if(cells.GetLength(0) > 5)
                {
                    sb.Append(cells[4] + "\t"); // Buy/Sell
                    sb.Append(cells[5].Replace("Shares", "").Trim() + "\t"); // Quantity
                    sb.Append(cells[0] + "\t"); // Ticker
                    sb.Append(cells[8].Replace("$", "") + "\t"); // fill Price

                    DataRow tickersDR = _positionsDataTable.AsEnumerable().Where(x => x[(int)PC.Ticker].ToString().Contains(cells[0])).First();
                    string q = tickersDR.ItemArray[1].ToString(); 
                    sb.Append(q + "\r\n"); // Shares Owned
                }
            }
            textBox1.Text += sb.ToString();
        }
    }
}
