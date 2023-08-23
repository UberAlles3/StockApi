using CommonClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace StockApi
{
    /// <summary>
    /// 
    /// </summary>
    class Analyze
    {
        static TextFile TF = new TextFile();
        static List<PersonalStockData> personalStockDataList = new List<PersonalStockData>();

        static void ReadInStockData()
        {
            string[] parts;
            string currentFolder = Environment.CurrentDirectory;

            if (personalStockDataList.Count > 0)
                return;
            
            TF.OpenFile(currentFolder + @"\Stocks.txt", TextFile.TextFileMode.InputMode);

            foreach (string s in TF)
            {
                parts = s.Split("\t");

                PersonalStockData personalStockData = new PersonalStockData { Ticker = parts[0], SharesOwned = Convert.ToInt32(parts[1])};
                personalStockDataList.Add(personalStockData);

                Debug.WriteLine(s);
            }
            TF.CloseFile();
        }

        public static PersonalStockData PreFillAnalyzeFormData()
        {
            ReadInStockData();

            PersonalStockData personalStockData = personalStockDataList.Find(x => x.Ticker == StockSummary.Ticker);

            return personalStockData;
        }


        public static AnalyzeResults AnalyzeStockData(int sharesOwned, bool bought, int sharesTraded, int movementTargetPercent)
        {
            // combine trends with
            // one year target
            // EPS
            // Fair Value
            // Estimated Return %
            // Should we read in excel file?
            // Volatility

            // Trend
            float trendMetric = Convert.ToInt16(StockHistory.YearTrend) + Convert.ToInt16(StockHistory.MonthTrend) + Convert.ToInt16(StockHistory.WeekTrend);
            if(trendMetric == 0) // Very downward trend
                trendMetric = .9F;
            else if (trendMetric == 1)
                trendMetric = .95F;
            else if (trendMetric == 2)
                trendMetric = 1.05F;
            else if (trendMetric == 2)
                trendMetric = 1.1F; // Very upward trend

            // One Year Target
            float targetPriceMetric = 1F;
            if (YahooFinance.OneYearTargetColor == Color.Red)
                targetPriceMetric = .9F;
            if (YahooFinance.OneYearTargetColor == Color.Lime)
                targetPriceMetric = 1.1F;

            // EPS
            float epsMetric = 1F;
            if (StockSummary.EPSColor == Color.Red)
                epsMetric = .9F;
            if (StockSummary.EPSColor == Color.Lime)
                epsMetric = 1.1F;

            // Fair Value
            float fairValueMetric = 1F;
            if (StockSummary.FairValueColor == Color.Red)
                fairValueMetric = .9F;
            if (StockSummary.FairValueColor == Color.Lime)
                fairValueMetric = 1.1F;


            float totalMetric = trendMetric * targetPriceMetric * epsMetric * fairValueMetric;

            float buyPrice = StockHistory.HistoricDataToday.Price * ((100F - movementTargetPercent) / 100F);
            float sellPrice = StockHistory.HistoricDataToday.Price * ((100F + movementTargetPercent) / 100F);
            // Apply metrics
            buyPrice = buyPrice * totalMetric;
            sellPrice = sellPrice * totalMetric;


            AnalyzeResults analyzeResults = new AnalyzeResults();
            if (bought)
            {
                analyzeResults.BuyQuantity = Convert.ToInt16(sharesTraded * .8F); // Buy less if you just bought
                analyzeResults.SellQuantity = Convert.ToInt16(sharesTraded * 1.1F); // Sell more if you just bought
                if (analyzeResults.SellQuantity > Convert.ToInt16(sharesOwned / 2.5F))
                    analyzeResults.SellQuantity = Convert.ToInt16(sharesOwned / 2.5F);
            }
            else
            {
                analyzeResults.BuyQuantity = Convert.ToInt16(sharesTraded * 1.2F); // Buy more if you just sold
                analyzeResults.SellQuantity = Convert.ToInt16(sharesTraded * .8F); // Buy less if you just sold
            }

            analyzeResults.BuyPrice = buyPrice;

            return analyzeResults;
        }

        public class PersonalStockData
        {
            private string ticker = "";
            private int sharesOwned;

            public string Ticker { get => ticker; set => ticker = value; }
            public int SharesOwned { get => sharesOwned; set => sharesOwned = value; }

            public override string ToString()
            {
                return string.Format(
                    $"{Ticker}, {SharesOwned}"
                );
            }
        }

        public class AnalyzeResults
        {
            public int BuyQuantity;
            public float BuyPrice;
            public int SellQuantity;
            public float SellPrice;
        }
    }
}

/* TextFile usage docs
private void btnEnumerator_Click(object sender, EventArgs e)
{

}

private void btnReset_Click(object sender, EventArgs e)
{
    TextFile TF = new TextFile();
    TF.OpenFile(@"C:\FTP_Script.txt", TextFile.TextFileMode.InputMode);
    Console.WriteLine(TF.Data);
    TF.MoveNext();
    Console.WriteLine(TF.Data);
    TF.MoveNext();
    Console.WriteLine(TF.Data);
    TF.Reset();
    Console.WriteLine(TF.Data);
    TF.MoveNext();
    Console.WriteLine(TF.Data);
    TF.CloseFile();
}

private void btnFileNotFound_Click(object sender, EventArgs e)
{
    TextFile TF = new TextFile();
    TF.OpenFile(@"C:\XXXXXXXXXXX.txt", TextFile.TextFileMode.InputMode);
    TF.CloseFile();
}

private void button2_Click(object sender, EventArgs e)
{
    TextFile TF = new TextFile();
    TF.OpenFile(@"C:\FTP_Script.txt", TextFile.TextFileMode.InputMode);
    Console.WriteLine(TF.ReadAll());
    TF.CloseFile();
}

private void button1_Click(object sender, EventArgs e)
{
    TextFile TF = new TextFile();
    TF.OpenFile(@"C:\FTP_Script.txt", TextFile.TextFileMode.InputMode);

    while (!TF.EOF)
    {
        Console.WriteLine("{0:00}  {1}", TF.LineNumber, TF.Data);
        TF.MoveNext();
    }

    TF.CloseFile();
}

private void btnOutput_Click(object sender, EventArgs e)
{
    TextFile TF = new TextFile();
    TF.OpenFile(@"C:\FTP_Script2.txt", TextFile.TextFileMode.OutputMode);
    // TF.AutoFlush = true; // Flushes/commits the data to file after every write.
    TF.WriteData("a");
    TF.WriteData("b");
    TF.WriteLine("c");
    TF.Flush(); // Flushes/commits the data to file 
    TF.WriteData("a");
    TF.WriteData("b");
    TF.WriteLine("c");
    TF.CloseFile();
}

private void btnOutAppend_Click(object sender, EventArgs e)
{
    TextFile TF = new TextFile();
    TF.OpenFile(@"C:\FTP_Script2.txt", TextFile.TextFileMode.OutputAppend);
    // TF.AutoFlush = true; // Flushes/commits the data to file after every write.
    TF.WriteLine(DateTime.Now.ToString());
    TF.CloseFile();
}

private void button3_Click(object sender, EventArgs e)
{
    TextFile TF = new TextFile();
    TF.OpenFile(@"C:\FTP_Script2.txt", TextFile.TextFileMode.InputMode);
    Console.WriteLine(TF.ReadAll());
    TF.CloseFile();
}

private void Form1_Load(object sender, EventArgs e)
{

}

private void btnSendMail_Click(object sender, EventArgs e)
{
    MailMessage oMailMessage = new MailMessage();

    // MailMessage.SMTPServerIP = "192.168.15.2";
    //MailMessage.SMTPServerIP = "DADXP";
    oMailMessage.SMTPServerIP = "mail.adelphia.net";
    oMailMessage.DefaultSender = "YOU_DONT_KNOW@adelphia.net";
    oMailMessage.SendMail("test<br>test<b>bold</b>", "TestSubject", "gdrake@adelphia.net");

    Console.WriteLine(oMailMessage.LastError);
}
*/