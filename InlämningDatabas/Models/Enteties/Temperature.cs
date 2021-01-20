using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    class Temperature
    {
        public int ID { get; set; }
        public int DateID { get; set; }
        public float TemperatureReading { get; set; }
        public string PositionForReading { get; set; }
        public int Humidity { get; set; }
        public DateTime TimeOfTemperatureReading { get; set; }
        public DateTime DateOfTemperatureReading { get; set; }
    }
}
