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
            //SetUpDatabase.SetUp();
            //Console.WriteLine("Done and done!");
            FindDateAndTemperature.MenuAndPrint();
        }
    }
    class FindDateAndTemperature
    {
        public static void MenuAndPrint()
        {
            Console.WriteLine("Välkommen till Väder-databasen WeatherDB");
            Console.WriteLine("Vänligen välj vad du önskar göra:");
            Console.WriteLine("1: Se medeltemperatur för ett visst datum");
            Console.WriteLine("2: Sortera datum från varmaste till kallaste dagen");
            Console.WriteLine("3: Sortera datum från torraste till fuktigaste dagen");
            Console.WriteLine("4: Sortera datum från minst till störst mögelrisk");
            Console.WriteLine("5: Hitta första dagen av den meteorologiska hösten");
            Console.WriteLine("6: Hitta första dagen av den meteorologiska vintern");

            int menuChoise = int.Parse(Console.ReadLine());

            switch (menuChoise)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Var vänlig ange datumet du söker i format åååå-mm-dd.");
                    Console.Write(": ");
                    DateTime chosenDate = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    Console.WriteLine("Vänligen ange om du vill se temeratur för inomhus(inne) eller utomhus(ute)");
                    Console.Write(": ");
                    string sensor = Console.ReadLine();
                    Console.WriteLine(SearchDate(chosenDate, sensor));
                    
                    break;
                case 2:
                    Console.Clear();

                    break;
                case 3:
                    Console.Clear();

                    break;
                case 4:
                    Console.Clear();

                    break;
                case 5:
                    Console.Clear();

                    break;
                case 6:
                    Console.Clear();

                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Var vänlig ange siffran som föregår den aktion du vill utföra.");
                    MenuAndPrint();
                    break;
            }
        }
        private static float SearchDate(DateTime date, string sensor)
        {
            List<Library.Models.Temperature> temperatures = new List<Library.Models.Temperature>();
            using (var db = new Library.Models.EFContext())
            {
                var query = (from d in db.Date
                             where d.DateOfTemperature == date
                             select d.ID).FirstOrDefault().ToString();

                int dateID = int.Parse(query);

                var tempList = (from t in db.Temperature
                                where t.DateID == dateID
                                where t.PositionForReading == sensor
                                select t).ToList();
                foreach (var item in tempList)
                {
                    temperatures.Add(item);
                }
            }
            var averageTemperature = temperatures
                .Average(t => t.TemperatureReading);

            return averageTemperature;
        }
        private static void SortOnTemperature()
        {
            using (var db = new Library.Models.EFContext())
            {

            }
        }
        private static void SortOnHumidity()
        {
            using (var db = new Library.Models.EFContext())
            {

            }
        }
        private static void SortByMoldrisk()
        {
            using (var db = new Library.Models.EFContext())
            {

            }
        }
        private static void StartOfAutumn()
        {
            using (var db = new Library.Models.EFContext())
            {

            }
        }
        private static void StartOfWinter()
        {
            using (var db = new Library.Models.EFContext())
            {

            }
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

                    date.DateOfTemperature = DateTime.ParseExact(dateAndTime[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    temp.DateOfTemperatureReading = DateTime.Parse(dateAndTime[0]);
                    temp.TimeOfTemperatureReading = DateTime.Parse(dateAndTime[1]);
                    temp.PositionForReading = tempDetails[1];
                    temp.TemperatureReading = actualTempreading;
                    temp.Humidity = int.Parse(tempDetails[3]);

                    dates.Add(date);
                    temps.Add(temp);
                    Console.WriteLine("Line read done");
                }
            }
        }
        private static void AddToDatabase()
        {
            var q = dates
                .GroupBy(q => q.DateOfTemperature)
                .Select(q => new Library.Models.Date() { DateOfTemperature = q.Key })
                .ToList();

            Console.WriteLine($"{q.Count} vs {dates.Count}");
            List<Library.Models.Date> distinctDates = new List<Library.Models.Date>();
            distinctDates.AddRange(q);

            for (int i = 0; i < distinctDates.Count; i++)
            {
                distinctDates[i].Temperatures = new List<Library.Models.Temperature>();

                var q1 = temps
                    .Where(q => q.DateOfTemperatureReading == distinctDates[i].DateOfTemperature).ToList();

                distinctDates[i].Temperatures.AddRange(q1);

                Console.WriteLine($"Temperaturer tillagt i datun nr {i}");
            }

            using (var db = new Library.Models.EFContext())
            {
                foreach (var item in distinctDates)
                {
                    db.Date.Add(item);
                }

        
                Console.WriteLine("Datum inlagda i databasen");

                foreach (var item in temps)
                {
                    db.Temperature.Add(item);
                }
                db.SaveChanges();
            }
        }
    }
}
