using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;


namespace InlämningDatabas
{
    class Program
    {
        static void Main(string[] args)
        {
            SetUpDatabase.SetUp(); 
            Console.WriteLine("Hello World!");
        }
    }
    class SetUpDatabase
    {
        static List<Library.Models.Temperature> temps = new List<Library.Models.Temperature>();
        static List<Library.Models.Date> dates = new List<Library.Models.Date>();
        
        public static void SetUp()
        {
            ReadFromFile();
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

                    dates.Add(date);
                    temps.Add(temp);
                }
            }
        }
        private static void AddToDatabase()
        {
            using (var db = new Library.Models.EFContext())
            {
                for (int i = 0; i < dates.Count; i++)
                {
                    db.Date.Add(dates[i]);
                }

                for (int i = 0; i < temps.Count; i++)
                {
                    db.Temperature.Add(temps[i]);
                }
                db.SaveChanges();
            }
        }
    }
}
