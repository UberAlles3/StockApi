using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StockApi
{
    public partial class OffHighs : Form
    {
        private DataTable _tradesDataTable;

        public OffHighs(DataTable tradesDataTable)
        {
            InitializeComponent();
            _tradesDataTable = tradesDataTable;
        }
    }
}
