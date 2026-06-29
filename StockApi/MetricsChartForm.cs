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
        public MetricsChartForm()
        {
            InitializeComponent();
        }

        private void MetricsChartForm_Load(object sender, EventArgs e)
        {

            //metricList.Add(new MetricsXY() { Month = 11, Value = double.NaN });
            //metricList.Add(new MetricsXY() { Month = 12, Value = 1.15 });
            //metricList.Add(new MetricsXY() { Month = 1, Value = 1.1 });
            //metricList.Add(new MetricsXY() { Month = 2, Value = 1.2 });
            //metricList.Add(new MetricsXY() { Month = 3, Value = 1.23 });
            //metricList.Add(new MetricsXY() { Month = 4, Value = 1.19 });

            CreateChart("XOM");
            this.Text += " for XOM";
        }

        private void CreateChart(string ticker)
        {
            string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string[] labelMonths = new string[6];
            List<SqlMetric> metrics = new List<SqlMetric>();
            List<MetricsXY> metricXYList = new List<MetricsXY>();

            // Get All the metric rows
            SqlCrudOperations sqlCrudOperations = new SqlCrudOperations();
            metrics = sqlCrudOperations.GetMetricList(DateTime.Now.AddMonths(-5), ticker);

            int i = 0;
            foreach (SqlMetric sm in metrics)
            {
                labelMonths[i++] = months[sm.Month - 1];
                metricXYList.Add(new MetricsXY { Month = sm.Month, Value = sm.FinalMetric });
            }

            double low = metricXYList.Min(x => x.Value);
            double high = metricXYList.Max(x => x.Value);

            var bindingList = new BindingList<MetricsXY>(metricXYList);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DataSource = source.DataSource;
            cartesianChart1.AxisX.Add(new LiveCharts.Wpf.Axis
            {
                Title = "Month",
                Labels = labelMonths,
                //MinWidth = 60,
                //MaxWidth = 60,
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
            //var years = (from x in metricList
            //             select new { Year = x.Year }).Distinct();


            //foreach (var year in years)
            //{
            List<double> values = new List<double>();
            for (i = 0; i < 6; i++)
            {
                //double value = 0;
                //var data = from x in metricList
                //           where x.Month.Equals(month)
                //           //orderby x.Month ascending
                //           select new { x.Value, x.Month };
                //if (data.SingleOrDefault() != null)
                //    value = data.SingleOrDefault().Value;
                values.Add(metricXYList[i].Value);

            }
            series.Add(new LineSeries() { Title = "Months", Values = new ChartValues<double>(values), LineSmoothness = 0 });

            //}
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
