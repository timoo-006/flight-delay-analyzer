using Flight_delay_analyzer;
using OpenQA.Selenium;

FlightAware flightAware = new FlightAware("EGLL", "LSZH");
flightAware.GetFlightData();