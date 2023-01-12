using OpenQA.Selenium;

namespace Flight_delay_analyzer.FlightAware;

public interface IFlightAware
{
    public List<Flight> FlightList { get; }

    public void GetFlights();

    public void GetFlightDelay(string flightNumber);

    public void FilterFlights();

    public List<IWebElement> GetFlightsFromHtml();
}