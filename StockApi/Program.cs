using System;
using System.Windows.Forms;
using log4net;
using log4net.Config;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

namespace StockApi
{
    static class Program
    {
        public static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Program));

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
