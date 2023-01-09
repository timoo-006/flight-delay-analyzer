using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flight_delay_analyzer
{
    class JSONReadAndWrite
    {
        /// <summary>
        /// Reads the flight code and stores it into a JSON file
        /// </summary>
        /// <returns>deserializedJson</returns>
        public List<JSONInputs> ReadFlightsOutFromJSON(string originAirport, string destinationAirport, DateTime dateOfFlight)
        {
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

        /// <summary>
        /// Stores delayed flights into JSON file
        /// </summary>
        /// <param name="delayedFlights"></param>
        public void StoreFlightsIntoJSON(List<Flight> delayedFlights)
        {
            try
            {
                var delayedFlightsIntoJson = JsonConvert.SerializeObject(delayedFlights);
                File.AppendAllText("delayedFlights.json", delayedFlightsIntoJson);

                if (delayedFlights.Count <= 0)
                {
                    throw new Exception("No flights detected");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AnalyzeResults(List<Flight> flightList)
        {
            foreach (Flight flight in flightList)
            {
                if (flight.delay.Contains("verspätet"))
{
                    int minutesOfDelay = Convert.ToInt32(new string(flight.delay.Where(char.IsDigit).ToArray()).ToString());
                    string delayedFlightNumber = flight.flightNumber;
                }
                if (flight.delay.Contains("verfrüht"))
                {
                    int minutesOfDelay = Convert.ToInt32(new string(flight.delay.Where(char.IsDigit).ToArray()).ToString());
                    string delayedFlightNumber = flight.flightNumber;
                }
            }
        }
    }
}