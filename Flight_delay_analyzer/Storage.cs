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
    public class JSONInputs
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
        public List<JSONInputs> ReadFlightsOutFromJSON()
        {
            // Read the readFlight JSON File
            try
            {
                var json = File.ReadAllText("readFlight.json");
                if (json == null || json == "")
                {
                    throw new Exception("Couldn't read file");
                }
                var deserializedJson = JsonConvert.DeserializeObject<List<JSONInputs>>(json);
                return deserializedJson;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void StoreFlightsIntoJSON(List<Flight> delayedFlights)
        {
            try
            {
                var delayedFlightsIntoJson = JsonConvert.SerializeObject(delayedFlights);
                File.AppendAllText("delayedFlights.json", delayedFlightsIntoJson);
                if (delayedFlights.Count > 0)
                {
                    Console.WriteLine("No delayed flights detected");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
