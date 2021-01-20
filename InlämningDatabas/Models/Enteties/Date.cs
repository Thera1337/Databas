using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Models
{
    class Date
    {
        public int ID { get; set; }
        public DateTime DateOfTemperature { get; set; }
        public List<Temperature> Temperatures { get; set; }
    }
}
