using CommonClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (personalStockDataList.Count > 0)
                return;
            
            TF.OpenFile(@"E:\Source Code\StockAPI\StockApi\StockApi\bin\Debug\netcoreapp3.1\Stocks.txt", TextFile.TextFileMode.InputMode);

            foreach (string s in TF)
            {
                parts = s.Split("\t");

                PersonalStockData personalStockData = new PersonalStockData { Ticker = parts[0], Shares = Convert.ToInt32(parts[1])};
                personalStockDataList.Add(personalStockData);

                Debug.WriteLine(s);
            }
            TF.CloseFile();
        }

        public static void AnalyzeStockData()
        {
            ReadInStockData();

            PersonalStockData personalStockData = personalStockDataList.Find(x => x.Ticker == StockSummary.Ticker); 


        }


        public class PersonalStockData
        {
            private string ticker = "";
            private int shares;

            public string Ticker { get => ticker; set => ticker = value; }
            public int Shares { get => shares; set => shares = value; }

            public override string ToString()
            {
                return string.Format(
                    $"{Ticker}, {Shares}"
                );
            }
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