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
            string sensor;

            switch (menuChoise)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Var vänlig ange datumet du söker i format åååå-mm-dd.");
                    Console.Write(": ");
                    DateTime chosenDate = DateTime.ParseExact(Console.ReadLine(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                    Console.WriteLine("Vänligen ange om du vill se temperatur för inomhus(inne) eller utomhus(ute)");
                    Console.Write(": ");
                    sensor = Console.ReadLine();
                    Console.WriteLine(SearchDate(chosenDate, sensor));

                    break;
                case 2:
                    Console.Clear();
                    Console.WriteLine("Vänligen ange om du vill sortera temperatur för inomhus(inne) eller utomhus(ute)");
                    sensor = Console.ReadLine();

                    foreach (var item in SortOnTemperature(sensor))
                    {
                        Console.WriteLine($"{item.date}, {item.avgTemp}");
                    }

                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("Vänligen ange om du vill sortera fuktighetsgrad för inomhus(inne) eller utomhus(ute)");
                    sensor = Console.ReadLine();

                    foreach (var item in SortOnHumidity(sensor))
                    {
                        Console.WriteLine($"{item.date}, {item.avgHumidity}");
                    }
                    break;
                case 4:
                    Console.Clear();
                    Console.WriteLine("Vänligen ange om du vill se fuktighetsgrad för inomhus(inne) eller utomhus(ute)");
                    sensor = Console.ReadLine();

                    foreach (var item in SortByMoldrisk(sensor))
                    {
                        Console.WriteLine($"{item.date}, {item.moldrisk}");
                    }
                    break;
                case 5:
                    Console.Clear();
                    Console.WriteLine($"Hösten startade :{StartOfAutumn()}");

                    break;
                case 6:
                    Console.Clear();
                    Console.WriteLine($"Vintern startade :{StartOfWinter()}");

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
            using (var db = new Library.Models.EFContext())
            {
                var query = db.Date
                    .Where(d => d.DateOfTemperature == date)
                    .Select(d => d.ID)
                    .FirstOrDefault();

                var tempList = db.Temperature
                    .Where(t => t.DateID == query)
                    .Where(t => t.PositionForReading == sensor)
                    .Select(t => t.TemperatureReading)
                    .AsEnumerable();

                return tempList.Average();
            }
        }
        private static List<(DateTime date, float avgTemp)> SortOnTemperature(string sensor)
        {
            using (var db = new Library.Models.EFContext())
            {
                var query = db.Temperature
                    .Where(t => t.PositionForReading == sensor)
                    .GroupBy(t => t.Date.DateOfTemperature)
                    .Select(g => new { date = g.Key, avgTemp = g.Average(t => t.TemperatureReading) })
                    .OrderByDescending(x => x.avgTemp).AsEnumerable();

                List<(DateTime, float)> resultSet = new();

                foreach (var item in query)
                {
                    resultSet.Add((item.date, item.avgTemp));
                }
                return resultSet;
            }
        }
        private static List<(DateTime date, double avgHumidity)> SortOnHumidity(string sensor)
        {
            using (var db = new Library.Models.EFContext())
            {
                var query = db.Temperature
                    .Where(t => t.PositionForReading == sensor)
                    .GroupBy(t => t.Date.DateOfTemperature)
                    .Select(g => new { date = g.Key, avgHumidity = g.Average(t => t.Humidity) })
                    .OrderByDescending(x => x.avgHumidity).AsEnumerable();

                List<(DateTime, double)> resultSet = new();

                foreach (var item in query)
                {
                    resultSet.Add((item.date, item.avgHumidity));
                }
                return resultSet;
            }
        }
        private static List<(DateTime date, double moldrisk)> SortByMoldrisk(string sensor)
        {
            using (var db = new Library.Models.EFContext())
            {
                var query = db.Temperature
                    .Where(t => t.PositionForReading == sensor)
                    .GroupBy(t => t.Date.DateOfTemperature)
                    .Select(t => new
                    {
                        avgHumidity = t.Average(t => t.Humidity),
                        avgTemp = t.Average(t => t.TemperatureReading),
                        date = t.Key
                    });

                var moldriskList = query
                    .Where(q => q.avgTemp >= 0 && q.avgHumidity >= 78)
                    .Select(q => new
                    {
                        date = q.date,
                        moldRisk = ((q.avgHumidity - 78) * (q.avgTemp / 15)) / 0.22
                    })
                    .OrderByDescending(q => q.moldRisk);

                List<(DateTime date, double moldrisk)> resultSet = new();

                foreach (var item in moldriskList)
                {
                    resultSet.Add((item.date, item.moldRisk));
                }

                return resultSet;
            }
        }
        private static string StartOfAutumn()
        {
            using (var db = new Library.Models.EFContext())
            {
                var query = db.Temperature
                    .Where(t => t.PositionForReading == "ute")
                    .GroupBy(t => t.Date.DateOfTemperature)
                    .Select(g => new
                    {
                        date = g.Key,
                        avgTemp = g.Average(t => t.TemperatureReading)
                    })
                    .AsEnumerable();

                float compareAutumn = 10;
                int daysCounter = 0;
                string startOfAutumn="";
                foreach (var item in query)
                {
                    if (item.avgTemp <= compareAutumn)
                    {
                        daysCounter++;
                    }
                    else
                    {
                        daysCounter = 0;
                    }

                    if (daysCounter >= 5)
                    {
                        startOfAutumn = $"{item.date}";
                        break;
                    }
                    else
                    {
                        startOfAutumn = "Startdag för höst kunde inte hittas";
                    }
                }
                return startOfAutumn;
            }
        }
        private static string StartOfWinter()
        {
            using (var db = new Library.Models.EFContext())
            {
                var query = db.Temperature
                    .Where(t => t.PositionForReading == "ute")
                    .GroupBy(t => t.Date.DateOfTemperature)
                    .Select(g => new
                    {
                        date = g.Key,
                        avgTemp = g.Average(t => t.TemperatureReading)
                    })
                    .AsEnumerable();

                float compareWinter = 0;
                int daysCounter = 0;
                string startOfWinter = "";
                foreach (var item in query)
                {
                    if (item.avgTemp <= compareWinter)
                    {
                        daysCounter++;
                    }
                    else
                    {
                        daysCounter = 0;
                    }

                    if (daysCounter >= 5)
                    {
                        startOfWinter = $"{item.date}";
                        break;
                    }
                    else
                    {
                        startOfWinter = "Startdag för vinter kunde inte hittas";
                    }
                }
                return startOfWinter;
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
