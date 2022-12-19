using Flight_delay_analyzer;
using OpenQA.Selenium;

FlightAware flightAware = new FlightAware("KLAX", "KJFK");
flightAware.GetFlights();