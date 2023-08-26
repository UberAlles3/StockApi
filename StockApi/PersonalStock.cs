using CommonClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StockApi
{
    public class PersonalStock
    {
        private static List<PersonalStockData> personalStockDataList = new List<PersonalStockData>();

        public PersonalStock()
        {
            ReadInStockData();
        }

        private void ReadInStockData()
        {
            TextFile TF = new TextFile();
            string[] parts;
            string currentFolder = Environment.CurrentDirectory;

            if (personalStockDataList.Count > 0)
                return;

            TF.OpenFile(currentFolder + @"\Stocks.txt", TextFile.TextFileMode.InputMode);

            foreach (string s in TF)
            {
                parts = s.Split("\t");

                PersonalStockData personalStockData = new PersonalStockData { Ticker = parts[0], SharesOwned = Convert.ToInt32(parts[1]) };
                personalStockDataList.Add(personalStockData);

                Debug.WriteLine(s);
            }
            TF.CloseFile();
        }

        public PersonalStockData GetPersonalDataForTicker(string ticker)
        {
            PersonalStockData personalStockData = personalStockDataList.Find(x => x.Ticker == ticker);

            return personalStockData;
        }

        public class PersonalStockData
        {
            private string ticker = "";
            private float sharesOwned;

            public string Ticker { get => ticker; set => ticker = value; }
            public float SharesOwned { get => sharesOwned; set => sharesOwned = value; }

            public override string ToString()
            {
                return string.Format(
                    $"{Ticker}, {SharesOwned}"
                );
            }
        }
    }
}
