using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StockApi
{
    public partial class PerformanceForm : Form
    {
        public PerformanceForm()
        {
            InitializeComponent();
        }

        private void PerformanceForm_Load(object sender, EventArgs e)
        {
            this.BackColor = dataGridView2.BackgroundColor;
        }

        public void BindData()
        {


        }
    }
}
