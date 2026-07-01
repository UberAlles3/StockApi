using LiveCharts;
using LiveCharts.Wpf;
using SqlLayer;
using SqlLayer.SQL_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;

namespace StockApi
{
    public partial class MetricsChartForm : Form
    {
        string _ticker = "";
        
        public MetricsChartForm(string ticker)
        {
            InitializeComponent();
            _ticker = ticker;
        }

        private void MetricsChartForm_Load(object sender, EventArgs e)
        {

            //metricList.Add(new MetricsXY() { Month = 11, Value = double.NaN });
            //metricList.Add(new MetricsXY() { Month = 12, Value = 1.15 });
            //metricList.Add(new MetricsXY() { Month = 1, Value = 1.1 });
            //metricList.Add(new MetricsXY() { Month = 2, Value = 1.2 });
            //metricList.Add(new MetricsXY() { Month = 3, Value = 1.23 });
            //metricList.Add(new MetricsXY() { Month = 4, Value = 1.19 });

            this.Text += " for " + _ticker;
            CreateChart(_ticker);
        }

        private void CreateChart(string ticker)
        {
            string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Jan", "Feb", "Mar", "Apr", "May", "Jun" };
            int[] monthIndexes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1, 2, 3, 4, 5, 6 };
            string[] labelMonths = new string[6];
            List<SqlMetric> metrics = new List<SqlMetric>();
            List<MetricsXY> metricXYList = new List<MetricsXY>();
            int startMonth = DateTime.Now.AddMonths(-5).Month;

            // Get All the metric rows
            SqlCrudOperations sqlCrudOperations = new SqlCrudOperations();
            metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddMonths(-5), ticker);

            for (int i = 0; i < 6; i++)
            {
                int monthIndex = monthIndexes[startMonth + i - 1];

                // Find the sql metric row that matches the month number. 
                bool found = false; 
                foreach (SqlMetric sm in metrics)
                {
                    if (sm.Month == monthIndex)
                    {
                        metricXYList.Add(new MetricsXY { Month = sm.Month, Value = sm.FinalMetric });
                        found = true;
                        break;
                    }
                }
                if(!found)
                    metricXYList.Add(new MetricsXY { Month = i + 1, Value = double.NaN });

                labelMonths[i] = months[monthIndex - 1];
            }


            double low = metricXYList.Where(x => x.Value > 0).Min(x => x.Value);
            double high = metricXYList.Where(x => x.Value > 0).Max(x => x.Value);

            var bindingList = new BindingList<MetricsXY>(metricXYList);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DataSource = source.DataSource;
            cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Month",
                Labels = labelMonths,
                LabelsRotation = 45
            }); ;
            cartesianChart1.AxisY.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Metric",
                LabelFormatter = value => value.ToString("0.00"),
                MinValue = low - .03,
                MaxValue = high + .03,
                Separator = new Separator
                {
                    StrokeThickness = 1.5,
                    Stroke = System.Windows.Media.Brushes.LightGray, // Line color
                    Step = .04 // Force grid line every .04 units
                }
            });
            cartesianChart1.LegendLocation = LiveCharts.LegendLocation.None;

            cartesianChart1.Series.Clear();
            SeriesCollection series = new SeriesCollection();

            List<double> values = new List<double>();
            for (int i = 0; i < metricXYList.Count; i++)
            {
                values.Add(metricXYList[i].Value);
            }
            series.Add(new LineSeries() { Title = "Months", Values = new ChartValues<double>(values), LineSmoothness = 0 });

            cartesianChart1.Series = series;
        }
    }

    public class MetricsXY
    {
        //public int Year { get; set; }
        public int Month { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return $"Month: {Month}, Value: {Value}";
        }
    }
}
