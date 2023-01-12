using Flight_delay_analyzer;
using Flight_delay_analyzer.FlightAware;
using Flight_delay_analyzer.Storage;
using static Flight_delay_analyzer.Storage.JSONReadAndWrite;

namespace JSONReadAndWrite_Tests
{
    [TestClass]
    public class JSONReadAndWrite_Tests
    {
        [TestMethod]
        public void JSONReadAndWrite_ReadFlightsOutFromJSON_ReturnsListWithFlights()
        {
            //Arrange
            JSONReadAndWrite jSONReadAndWrite = new();
            string originAirport = "SFO";
            string destinationAirport = "JFK";
            string json = "[{\"originAirport\":\"SFO\",\"destinationAirport\":\"JFK\"},{\"originAirport\":\"SFO\",\"destinationAirport\":\"JFK\"}]";
            File.WriteAllText("readFlight.json", json);
            //Act
            var result = jSONReadAndWrite.ReadFlightsOutFromJSON(originAirport, destinationAirport, DateTime.Now.AddDays(-1));
            //Assert
            Assert.AreEqual(originAirport, result[0].OriginAirport);
            Assert.AreEqual(destinationAirport, result[0].DestinationAirport);
        }

        [TestMethod]
        public void JSONReadAndWrite_StoreFlightsIntoJSON_CheckIfJsonSavesAndIfItSavesTheExpectedOutput()
        {
            //Arrange
            JSONReadAndWrite jSONReadAndWrite = new();
            List<Flight> flights = new List<Flight>();
            Flight flight = new Flight("LX375", "5 minuten verfr�ht");
            flights.Add(flight);
            if (File.Exists("delayedFlights.json")) File.Delete("delayedFlights.json");
            string expectedJson = "[{\"delay\":\"5 minuten verfr�ht\",\"flightNumber\":\"LX375\"}]";
            //Act
            jSONReadAndWrite.StoreFlightsIntoJSON(flights);
            string json = File.ReadAllText("delayedFlights.json");
            //Assert
            Assert.IsTrue(File.Exists("delayedFlights.json"));
            Assert.AreEqual(expectedJson, json);
        }

        [TestMethod]
        public void JSONReadAndWrite_StoreFlightsIntoJSON_ThrowsExceptionListEmpty()
        {
            //Arrange
            JSONReadAndWrite jSONReadAndWrite = new();
            List<Flight> flights = new List<Flight>();
            //Act
            Action action = () => jSONReadAndWrite.StoreFlightsIntoJSON(flights);
            //Assert
            Assert.ThrowsException<Exception>(action);
        }

        [TestMethod]
        public void JSONReadAndWrite_AnalyzeResults_GetMostDelayedFlight()
        {
            //Arrange
            JSONReadAndWrite jsonReadAndWrite = new();
            List<Flight> flights = new List<Flight>();
            Flight flightDelayed = new Flight("LX375", "15 minutes versp�tet");
            Flight flightPunctual = new Flight("AA421", "p�nktlich");
            Flight flightAnticipated = new Flight("AE200", "15 minutes verfr�ht");
            flights.Add(flightDelayed);
            flights.Add(flightPunctual);
            flights.Add(flightAnticipated);
            List<FlightsAnalyzeProperties> flightsToAnalyze = new List<FlightsAnalyzeProperties>()
            {
                new FlightsAnalyzeProperties(){ FlightNumber = flightDelayed.flightNumber, FlightDelay = 15 },
                new FlightsAnalyzeProperties(){ FlightNumber = flightPunctual.flightNumber, FlightDelay = 0 },
                new FlightsAnalyzeProperties(){ FlightNumber = flightAnticipated.flightNumber, FlightDelay = -15 },
            };
            //Act
            jsonReadAndWrite.AnalyzeResults(flights, null, null, DateTime.Now.AddDays(-1));
            int biggestDelay = flightsToAnalyze.Max(flight => flight.FlightDelay);
            //Assert
            Assert.AreEqual(15, biggestDelay);
        }

        [TestMethod]
        public void JSONReadAndWrite_AnalyzeResults_GetEarliestAnticipatedFlight()
        {
            //Arrange
            JSONReadAndWrite jSONReadAndWrite = new();
            List<Flight> flights = new List<Flight>();
            Flight flightDelayed = new Flight("LX375", "15 minutes versp�tet");
            Flight flightPunctual = new Flight("AA421", "p�nktlich");
            Flight flightAnticipated = new Flight("AE200", "15 minutes verfr�ht");
            flights.Add(flightDelayed);
            flights.Add(flightPunctual);
            flights.Add(flightAnticipated);
            List<FlightsAnalyzeProperties> flightsToAnalyze = new List<FlightsAnalyzeProperties>()
            {
                new FlightsAnalyzeProperties(){ FlightNumber = flightDelayed.flightNumber, FlightDelay = 15 },
                new FlightsAnalyzeProperties(){ FlightNumber = flightPunctual.flightNumber, FlightDelay = 0 },
                new FlightsAnalyzeProperties(){ FlightNumber = flightAnticipated.flightNumber, FlightDelay = -15 },
            };
            //Act
            jSONReadAndWrite.AnalyzeResults(flights, null, null, DateTime.Now.AddDays(-1));
            int earliestAnticipatedFlight = flightsToAnalyze.Min(flight => flight.FlightDelay);
            //Assert
            Assert.AreEqual(-15, earliestAnticipatedFlight);
        }
    }
}