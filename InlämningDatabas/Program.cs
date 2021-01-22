using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace InlämningDatabas
{
    class Program
    {
        static void Main(string[] args)
        {
            SetUpDatabase.SetUp();
            Console.WriteLine("Done and done!");
        }
    }
    class SetUpDatabase
    {
        static List<Library.Models.Temperature> temps = new List<Library.Models.Temperature>();
        static List<Library.Models.Date> dates = new List<Library.Models.Date>();

        public static void SetUp()
        {
            ReadFromFile();
            Console.Clear();
            Console.WriteLine("Fil inläst!");
            AddToDatabase();
        }
        private static void ReadFromFile()
        {
            using (StreamReader reader = new StreamReader("TemperaturData.csv"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Library.Models.Date date = new Library.Models.Date();
                    Library.Models.Temperature temp = new Library.Models.Temperature();

                    string[] tempDetails = line.Split(',');
                    string[] dateAndTime = tempDetails[0].Split(' ');
                    string[] temperature = tempDetails[2].Split('.');
                    string tempreading = $"{temperature[0]},{temperature[1]}";
                    float actualTempreading = float.Parse(tempreading);

                    date.DateOfTemperature = DateTime.Parse(dateAndTime[0]);

                    temp.DateOfTemperatureReading = DateTime.Parse(dateAndTime[0]);
                    temp.TimeOfTemperatureReading = DateTime.Parse(dateAndTime[1]);
                    temp.PositionForReading = tempDetails[1];
                    temp.TemperatureReading = actualTempreading;
                    temp.Humidity = int.Parse(tempDetails[3]);

                    if (dates[dates.Count-1].DateOfTemperature != date.DateOfTemperature)
                    {
                        dates.Add(date);
                    }
                    
                    temps.Add(temp);
                    Console.WriteLine("Line read done");
                }
            }
        }
        private static void AddToDatabase()
        {
            for (int i = 0; i < dates.Count; i++)
            {
                dates[i].Temperatures = new List<Library.Models.Temperature>();

                var q1 = temps
                    .Where(q => q.DateOfTemperatureReading == dates[i].DateOfTemperature).ToList();

                foreach (var item in q1)
                {
                    dates[i].Temperatures.Add(item);
                }
                Console.WriteLine($"Temperaturer tillagt i datun nr {i}");
            }

            int datesCounter = 0;
            int tempsCounter = 0;

            using (var db = new Library.Models.EFContext())
            {
                while (datesCounter < dates.Count)
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        db.Date.Add(dates[datesCounter]);
                        datesCounter++;
                    }
                    db.SaveChanges();
                    Console.WriteLine($"{datesCounter} datum inlagda i databasen");
                }

                while (tempsCounter < temps.Count)
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        db.Temperature.Add(temps[tempsCounter]);
                        tempsCounter++;
                    }
                    db.SaveChanges();
                    Console.WriteLine($"{tempsCounter} mätnignar inlagda i databasen");
                }
            }
        }
    }
}
