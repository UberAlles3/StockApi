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
        public string Ticker = "";
        public int Period = 0;
        
        public string Url10Day = "https://api.wsj.net/api/kaavio/charts/big.chart?nosettings=1&symb=?ticker?&uf=0&type=2&size=2&style=320&freq=7&entitlementtoken=0c33378313484ba9b46b8e24ded87dd6&time=18&rand=1171500738&compidx=&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=335&width=579&mocktick=1";
        public string Url1Year = "https://api.wsj.net/api/kaavio/charts/big.chart?nosettings=1&symb=?ticker?&uf=0&type=2&size=2&style=320&freq=1&entitlementtoken=0c33378313484ba9b46b8e24ded87dd6&time=8&rand=1763347161&compidx=&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=335&width=579&mocktick=1";
        public string Url3Year = "https://api.wsj.net/api/kaavio/charts/big.chart?nosettings=1&symb=?ticker?&uf=0&type=2&size=2&style=320&freq=2&entitlementtoken=0c33378313484ba9b46b8e24ded87dd6&time=10&rand=933137116&compidx=&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=335&width=579&mocktick=1";
        public string Url5Year = "https://api.wsj.net/api/kaavio/charts/big.chart?nosettings=1&symb=?ticker?&uf=0&type=2&size=2&style=320&freq=2&entitlementtoken=0c33378313484ba9b46b8e24ded87dd6&time=12&rand=933137116&compidx=&ma=0&maval=9&lf=1&lf2=0&lf3=0&height=335&width=579&mocktick=1";
        public string Url = "";

        public string UrlSC10Day = "https://stockcharts.com/c-sc/sc?s=?ticker?&p=D&b=1&g=100&yr=1&mn=0&dy=0&i=p45662470291";
        public string UrlSC1Year = "https://stockcharts.com/c-sc/sc?s=?ticker?&p=D&b=1&g=2&yr=1&mn=0&dy=0&i=p45662470291";
        public string UrlSC3Year = "https://stockcharts.com/c-sc/sc?s=?ticker?&p=D&b=1&g=1&yr=1&mn=0&dy=0&i=p45662470291";
        public string UrlSC5Year = "https://stockcharts.com/c-sc/sc?s=?ticker?&p=D&b=1&g=0&yr=1&mn=0&dy=0&i=p45662470291";



        //https://stockcharts.com/c-sc/sc?s=XOM&p=D&b=1&g=0&yr=1&mn=0&dy=0&i=p45662470291

        // https://stockcharts.com/c-sc/sc?s=VTI&p=D&yr=1&mn=6&dy=0&i=p54662805948&r=1767720960506 moving average

        // https://stockcharts.com/c-sc/sc?s=AAPL&p=D&b=5&g=0&i=t8436358827c&r=1615503207659
        // s = symbol
        // p = ? (maybe D = daily?)
        // b = ?
        // g = ?
        // i = ?
        // r = current time (unix epoch format) with three trailing digits


        public ChartForm(string ticker, int period)
        {
            InitializeComponent();
            Ticker = ticker;
            Period = period;
        }

        private void ChartForm_Load(object sender, EventArgs e)
        {
            if (Period == 0)
            {
                this.Text = Ticker + " - 10 Day Chart";
                Url = Url10Day.Replace("?ticker?", Ticker);
            }
            else if (Period == 1)
            {
                this.Text = Ticker + " - 1 Year Chart";
                Url = Url1Year.Replace("?ticker?", Ticker);
            }
            else if (Period == 2)
            {
                this.Text = Ticker + " - 3 Year Chart";
                Url = Url3Year.Replace("?ticker?", Ticker);
            }
            else if (Period == 3)
            {
                this.Text = Ticker + " - 5 Year Chart";
                Url = Url5Year.Replace("?ticker?", Ticker);
            }

            try
            {
                pictureBox1.Load(Url);
            }
            catch 
            {
                if (Period == 0)
                {
                    Url = UrlSC10Day.Replace("?ticker?", Ticker);
                }
                else if (Period == 1)
                {
                    Url = UrlSC1Year.Replace("?ticker?", Ticker);
                }
                else if (Period == 2)
                {
                    Url = UrlSC3Year.Replace("?ticker?", Ticker);
                }
                else if (Period == 3)
                {
                    Url = UrlSC5Year.Replace("?ticker?", Ticker);
                }

                this.Width = 1000;
                this.Height = 560;

                pictureBox1.Load(Url);
            }
        }
    }
}
