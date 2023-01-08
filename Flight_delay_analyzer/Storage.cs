using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flight_delay_analyzer
{
    class JSONItemsInput
    {
        public string OriginAirport { get; set; }
        public string DestinationAirport { get; set;}
    }

    class JSONItemsOutput
    {
        public string FlightNumber { get; set; }
        public string Delay { get; set; }
        public DateTime dateTime => DateTime.Today.AddDays(-1);
    }

    class Storage
    {
        public Storage()
        {
        }

        public void ReadFlightsOutFromJSON()
        {
            // Read the readFlight JSON File
            var json = File.ReadAllText("readFlight.json");
            var deserializedJson = JsonConvert.DeserializeObject<JSONItemsInput>(json);
            string originAirport = deserializedJson.OriginAirport;
            string destinationAirport = deserializedJson.DestinationAirport;
        }

        public void StoreFlightsIntoJSON()
        {
            JSONItemsInput jsonObject = new();
            FlightAware flightAware = new FlightAware(jsonObject.OriginAirport, jsonObject.DestinationAirport, FlightAware._driver);
            
            Console.WriteLine(flightAware.DelayedFlights);
        }
    }
}
