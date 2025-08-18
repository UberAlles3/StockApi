using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockApi
{
    /// <summary>
    /// Daily export of metrics to a csv for import into a spreadsheet
    /// </summary>
    public class MetricsExport
    {
        public async Task<int> DailyGetMetrics()
        {
            // Your code to be executed daily
            MessageBox.Show("Daily function executed!");

            return 0;
        }
    }
}
