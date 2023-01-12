using Newtonsoft.Json;
using Flight_delay_analyzer.FlightAware;

namespace Flight_delay_analyzer.Storage
{
    public class JSONReadAndWrite
    {
        public class FlightsAnalyzeProperties
        {
            public string FlightNumber { get; set; }
            public int FlightDelay { get; set; }
        }

        /// <summary>
        /// Reads the flight code and stores it into a JSON file
        /// </summary>
        /// <returns>List<JSONInputs></returns>
        /// <param name="originAirport"></param>
        /// <param name="destinationAirport"></param>
        /// <param name="dateOfFlight"></param>
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
                Console.WriteLine(ex.Message);
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

                if (delayedFlights.Count == 0)
                {
                    throw new Exception("No flights detected");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AnalyzeResults(List<Flight> flightList, string origin, string destination, DateTime dateOfFlight)
        {
            List<FlightsAnalyzeProperties> flightsToAnalyze = new List<FlightsAnalyzeProperties>();
            foreach (Flight flight in flightList)
            {
                FlightsAnalyzeProperties flightAnalyzeObject = new FlightsAnalyzeProperties();
                int minutesOfDelay = 0;
                if (flight.delay.Contains("Verspätung"))
                {
                    int hoursDelayIntoMinutesDelay = 0;
                    if (flight.delay.Contains("Stunde"))
                    {
                        hoursDelayIntoMinutesDelay = Convert.ToInt32(new string(flight.delay.Substring(0, flight.delay.IndexOf("Stunde")).Where(x => char.IsDigit(x)).ToArray()).ToString()) * 60;
                    }
                    if(flight.delay.Contains("Minuten"))
                    {
                        if (!flight.delay.Contains("Stunde"))
                        {
                            minutesOfDelay = Convert.ToInt32(new string(flight.delay.Where(x => char.IsDigit(x)).ToArray()).ToString()) + hoursDelayIntoMinutesDelay;
                        }
                        else
                        {
                            minutesOfDelay = Convert.ToInt32(new string(flight.delay.Substring(flight.delay.IndexOf("Stunde")).Where(x => char.IsDigit(x)).ToArray()).ToString()) + hoursDelayIntoMinutesDelay;
                        }
                    }
                }
                else if (flight.delay.Contains("verfrüht"))
                {
                    //Turn delay to negative number to detect if flight is too late or too early
                    int hoursDelayIntoMinutesDelay = 0;
                    if (flight.delay.Contains("Stunde"))
                    {
                        hoursDelayIntoMinutesDelay = Convert.ToInt32(new string(flight.delay.Substring(0, flight.delay.IndexOf("Stunde")).Where(x => char.IsDigit(x)).ToArray()).ToString()) * -60;
                    }
                    if (flight.delay.Contains("Minuten"))
                    {
                        if (!flight.delay.Contains("Stunde"))
                        {
                            minutesOfDelay = Convert.ToInt32(new string(flight.delay.Where(x => char.IsDigit(x)).ToArray()).ToString()) * -1 - hoursDelayIntoMinutesDelay;
                        }
                        else
                        {
                            minutesOfDelay = Convert.ToInt32(new string(flight.delay.Substring(flight.delay.IndexOf("Stunde")).Where(x => char.IsDigit(x)).ToArray()).ToString()) * -1 - hoursDelayIntoMinutesDelay;
                        }
                    }
                }
                string flightNumber = flight.flightNumber;
                flightAnalyzeObject.FlightDelay = minutesOfDelay;
                flightAnalyzeObject.FlightNumber = flightNumber;
                flightsToAnalyze.Add(flightAnalyzeObject);
            }

            int biggestDelay = flightsToAnalyze.Max(flight => flight.FlightDelay);
            int earliestAnticipatedFlight = flightsToAnalyze.Min(flightList => flightList.FlightDelay);
            
            // Log the results to the console
            Console.WriteLine("\n=============================");
            Console.WriteLine("Results for " + origin + " to " + destination + " on " + dateOfFlight.ToString("dd.MM.yyyy"));
            Console.WriteLine("The biggest delay is: " + biggestDelay + " minutes");
            Console.WriteLine("The earliest anticipated flight is: " + earliestAnticipatedFlight + " minutes");
            Console.WriteLine("=============================");
        }
    }
}