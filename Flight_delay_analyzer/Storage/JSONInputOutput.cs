using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flight_delay_analyzer
{
    public class JSONInputs
    {
        public string OriginAirport { get; set; }
        public string DestinationAirport { get; set; }
        public DateTime dateOfFlight => DateTime.Today.AddDays(-1);
    }
}