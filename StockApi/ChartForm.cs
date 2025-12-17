using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StockApi
{
    public partial class ChartForm : Form
    {
        public string Url10Day = "https://api.wsj.net/api/kaavio/charts/big.chart?nosettings=1&symb=?ticker?&uf=0&type=2&size=2&style=320&freq=7&entitlementtoken=0c33378313484ba9b46b8e24ded87dd6&time=18&rand=1171500738&compidx=&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=335&width=579&mocktick=1";
        public string Url1Year = "https://api.wsj.net/api/kaavio/charts/big.chart?nosettings=1&symb=?ticker?&uf=0&type=2&size=2&style=320&freq=1&entitlementtoken=0c33378313484ba9b46b8e24ded87dd6&time=8&rand=1763347161&compidx=&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=335&width=579&mocktick=1";
        public string Url3Year = "https://api.wsj.net/api/kaavio/charts/big.chart?nosettings=1&symb=?ticker?&uf=0&type=2&size=2&style=320&freq=2&entitlementtoken=0c33378313484ba9b46b8e24ded87dd6&time=10&rand=933137116&compidx=&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=335&width=579&mocktick=1";
        public string Url5Year = "https://api.wsj.net/api/kaavio/charts/big.chart?nosettings=1&symb=?ticker?&uf=0&type=2&size=2&style=320&freq=2&entitlementtoken=0c33378313484ba9b46b8e24ded87dd6&time=12&rand=933137116&compidx=&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=335&width=579&mocktick=1";
        public string Url = ""; 

        public ChartForm()
        {
            InitializeComponent();
        }

        private void ChartForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Load(Url);
        }
    }
}
