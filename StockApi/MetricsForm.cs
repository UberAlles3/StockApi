using SqlLayer;
using SqlLayer.SQL_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using YahooLayer;

namespace StockApi
{
    public partial class MetricsForm : Form
    {
        private static StockDownloads _stockDownloads = new StockDownloads("");
        private static Analyze _analyze = new Analyze();
        private string _ticker;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public MetricsForm(string ticker)
        {
            _ticker = ticker;
            InitializeComponent();
        }

        private void MetricsForm_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Lastest");
            comboBox1.Items.Add("Last 2 Months");
            comboBox1.Items.Add("Last 3 Months");
            comboBox1.Items.Add("Last 6 Months");
            comboBox1.Items.Add("Last Year");
            comboBox1.SelectedIndex = 2;

            dataGridView1.Top = 1;
            dataGridView1.Height = panel1.Height - 2;
            dataGridView1.Left = 1;
            dataGridView1.Width = panel1.Width - 2;

            btnCancelMetrics.Visible = false;

            if(_ticker != "")
            {
                comboBox1.SelectedIndex = 3;
                txtTicker.Text = _ticker;
                btnSearch_Click(null, null);
            }
        }

        private void MetricsForm_Paint(object sender, PaintEventArgs e)
        {
            int width = this.Width - 17;
            int height = this.Height - 39;
            Pen greenPen = new Pen(Color.FromArgb(255, 128, 128, 128), 3);
            e.Graphics.DrawRectangle(greenPen, 0, -1, width, height);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<SqlMetric> metrics = new List<SqlMetric>();
            List<SqlMetric> bigMovers = new List<SqlMetric>();
            string ticker = txtTicker.Text;

            txtTicker.Text = ticker = ticker.ToUpper();

            if (ticker.Trim() == "")
                ticker = null;
            else
                ticker = ticker.ToUpper();

            // Get All the metric rows
            SqlCrudOperations sqlCrudOperations = new SqlCrudOperations();

            if (comboBox1.SelectedIndex == 0)
                metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddDays(-1), ticker);
            if (comboBox1.SelectedIndex == 1)
                metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddMonths(-1), ticker);
            if (comboBox1.SelectedIndex == 2)
                metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddMonths(-3), ticker);
            if (comboBox1.SelectedIndex == 3)
                metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddMonths(-5), ticker);
            if (comboBox1.SelectedIndex == 4)
                metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddMonths(-11), ticker);

            metrics = metrics.Where(metrics => Form1.PositionList.Select(x => x.Symbol).Contains(metrics.Ticker)).ToList();

            metrics = metrics.OrderBy(x => x.Ticker).ThenBy(x => x.Year).ThenBy(x => x.Month).ToList();

            bigMovers = GetBigMovers(metrics);

            if (chkBigChanges.Checked == true)
                metrics = bigMovers;

            BindListToMetricGrid(metrics);
            ColorGrid(metrics);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell tickerCell = null;
            string ticker;

            // Ensure the click isn't on the header row
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Access the specific DataGridViewCell
                DataGridViewCell clickedCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // Optional: Do something specific if it's a particular column
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Chart")
                {
                    tickerCell = dataGridView1.Rows[e.RowIndex].Cells[1];
                    // Retrieve the value of the cell
                    ticker = tickerCell.Value?.ToString();

                    MetricsChartForm form = new MetricsChartForm(ticker);
                    form.Owner = this;
                    form.Show();
                }
            }
        }

        private void BindListToMetricGrid(List<SqlMetric> metrics)
        {
            dataGridView1.Columns.Clear();

            var bindingList = new BindingList<SqlMetric>(metrics);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DefaultCellStyle.ForeColor = Form1.TextForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            dataGridView1.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView1.DataSource = source.DataSource;
            
            dataGridView1.Columns.Add("Chart", "Chart");

            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].HeaderText = "Ticker";
            dataGridView1.Columns[1].Width = 60;
            dataGridView1.Columns[2].HeaderText = "Year";
            dataGridView1.Columns[2].Width = 50;
            dataGridView1.Columns[3].HeaderText = "Month";
            dataGridView1.Columns[3].Width = 50;

            dataGridView1.Columns[4].HeaderText = "Price Trend";
            dataGridView1.Columns[4].Width = 55;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[4].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[5].HeaderText = "Earnings Per Share";
            dataGridView1.Columns[5].Width = 55;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[5].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[6].HeaderText = "Target Price";
            dataGridView1.Columns[6].Width = 55;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[6].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[7].HeaderText = "Price Book";
            dataGridView1.Columns[7].Width = 55;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[7].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[8].HeaderText = "Dividend";
            dataGridView1.Columns[8].Width = 60;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[8].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[9].HeaderText = "Profit Margin";
            dataGridView1.Columns[9].Width = 55;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[9].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[10].HeaderText = "Revenue";
            dataGridView1.Columns[10].Width = 60;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[10].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[11].HeaderText = "Profit";
            dataGridView1.Columns[11].Width = 55;
            dataGridView1.Columns[11].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[11].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[12].HeaderText = "Basic EPS";
            dataGridView1.Columns[12].Width = 55;
            dataGridView1.Columns[12].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[12].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[13].HeaderText = "Cash Debt";
            dataGridView1.Columns[13].Width = 55;
            dataGridView1.Columns[13].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[13].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[14].HeaderText = "Peg Ratio";
            dataGridView1.Columns[14].Width = 50;
            dataGridView1.Columns[14].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[14].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[15].HeaderText = "Valuation";
            dataGridView1.Columns[15].Width = 60;
            dataGridView1.Columns[15].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[15].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[16].HeaderText = "Cash Flow";
            dataGridView1.Columns[16].Width = 55;
            dataGridView1.Columns[16].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[16].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[17].HeaderText = "Final Metric";
            dataGridView1.Columns[17].Width = 55;
            dataGridView1.Columns[17].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[17].DefaultCellStyle.Format = "N3";

            dataGridView1.Columns[18].HeaderText = "Update Date";
            dataGridView1.Columns[18].Width = 80;
            dataGridView1.Columns[18].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomRight;
            dataGridView1.Columns[18].DefaultCellStyle.Format = "MM/dd/yyyy";

            dataGridView1.Columns[19].Width = 50;
            dataGridView1.Columns[19].DefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells["Chart"].Value = "Chart";
            }

            dataGridView1.Refresh();
        }

        private List<SqlMetric> GetBigMovers(List<SqlMetric> metrics)
        {
            List<SqlMetric> bigMovers = new List<SqlMetric>();

            string ticker = "";
            SqlMetric first = null;
            SqlMetric last = null;
            bool bigMover = false;
            foreach (SqlMetric r in metrics)
            {
                // Change of ticker 
                if (ticker != r.Ticker)
                {
                    if (first != null && bigMover) // first change of ticker
                    {
                        // Add first and last to Big Movers
                        bigMovers.Add(first);
                        bigMovers.Add(last);
                        bigMover = false;
                    }

                    first = r;
                }
                ticker = r.Ticker;
                last = r;

                // only add first and last metric
                if (comboBox1.SelectedIndex < 3 || first.UpdateDate.AddMonths(4) > DateTime.Now) // 1 month or 3 months or not enough history
                {
                    if (r.FinalMetric > first.FinalMetric * 1.04 || r.FinalMetric < first.FinalMetric * .965)
                    {
                        bigMover = true;
                    }
                }
                else
                {
                    if (r.FinalMetric > first.FinalMetric * 1.07 || r.FinalMetric < first.FinalMetric * .94)
                    {
                        bigMover = true;
                    }
                }
            }

            if (bigMover)
            {
                bigMovers.Add(first);
                bigMovers.Add(last);
            }

            return bigMovers;
        }

        private void ColorGrid(List<SqlMetric> metrics)
        {
            string ticker = "";
            double previous = 0;
            int i = 0;
            Color currentColor = Color.Black;
            Color currentForeColor = Form1.TextForeColor;
            Color altColor = Color.FromArgb(0, 0, 48);
            Color altForeColor = Color.White;
            int finalMetricColumnIndex = 0;

            // find final metric column;
            foreach(DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.HeaderText == "Final Metric") 
                    finalMetricColumnIndex = column.Index;
            }

            foreach (SqlMetric r in metrics)
            {
                if (ticker != r.Ticker)
                {
                    ticker = r.Ticker;
                    previous = r.FinalMetric;
                    if (i > 0)
                    {
                        //                        dataGridView1.Rows[i].Cells[17].Style.BackColor = currentColor;
                        if (currentColor == Color.Black)
                        {
                            currentColor = altColor;
                            currentForeColor = altForeColor;
                        }
                        else
                        {
                            currentColor = Color.Black;
                            currentForeColor = Form1.TextForeColor;
                        }
                    }
                }

                if (i < dataGridView1.Rows.Count - 2)
                    dataGridView1.Rows[i].Cells[finalMetricColumnIndex + 1].Style.BackColor = currentColor;

                if (r.FinalMetric > previous * 1.032)
                {
                    dataGridView1.Rows[i].Cells[finalMetricColumnIndex].Style.ForeColor = Color.Lime;
                }
                else if (r.FinalMetric > previous * 1.01)
                {
                    dataGridView1.Rows[i].Cells[finalMetricColumnIndex].Style.ForeColor = Color.FromArgb(205, 242, 202);
                }
                else if (r.FinalMetric < previous * .97)
                {
                    dataGridView1.Rows[i].Cells[finalMetricColumnIndex].Style.ForeColor = Color.Red;
                }
                else if (r.FinalMetric < previous * .99)
                {
                    dataGridView1.Rows[i].Cells[finalMetricColumnIndex].Style.ForeColor = Color.FromArgb(242, 202, 202);
                }

                i++;
            }
        }
 
        private async void btnRunMetrics_Click(object sender, EventArgs e)
        {
            Metrics metrics = new Metrics();
            txtTickerList.Clear();

            btnCancelMetrics.Visible = true;
            btnRunMetrics.Enabled = false;

            // Allow cancelling DailyMetrics with the Cancel button
            try
            {
                int x = await metrics.DailyGetMetrics(Form1.PositionList, txtTickerList, txtBeginLetter.Text, txtEndLetter.Text, cts.Token);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Metrics cancelled.");
                txtTickerList.Clear();
                cts = new CancellationTokenSource(); //reset
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            btnCancelMetrics.Visible = false;
            btnRunMetrics.Enabled = true;
        }

        private void btnCancelMetrics_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }
    }
}
