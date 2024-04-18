using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace StockApi
{
    public partial class WebView : Form
    {
        public bool NavigationComplete = false;
        public string Html;

        public WebView()
        {
            InitializeComponent();
        }

        private void WebView_Load(object sender, EventArgs e)
        {

        }

        public void Navigate(string url)
        {
            NavigationComplete = false;
            webView21.Source = new Uri(url);
            webView21.ZoomFactor = .5f;
        }

        private async void webView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            string html;
            html = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");
            html = Regex.Unescape(html);
            html = html.Remove(0, 1);
            html = html.Remove(html.Length - 1, 1);
            Html = html;
            NavigationComplete = true;
        }
    }
}
