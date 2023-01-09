using OpenQA.Selenium;

namespace Flight_delay_analyzer;

public interface IFlightAware
{
    public List<Flight> DelayedFlights { get; }

    public void GetFlights();

    public void GetFlightDelay(string flightNumber);

    public void FilterFlights();

    public List<IWebElement> GetFlightsFromHtml();
}